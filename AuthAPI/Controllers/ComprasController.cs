using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {

        private readonly AppDbContext _baseDatos;

        public ComprasController(AppDbContext context)
        {
            _baseDatos = context;
        }

        // GET: api/ListaCompras
        [HttpGet]
        [Route("ListaCompras")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            return await _baseDatos.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.MovimientosPieza)
                        .ThenInclude(m => m.Pieza)
                .ToListAsync();
        }

        // POST: api/AgregarCompra
        [HttpPost]
        [Route("AgregarCompra")]
        public async Task<ActionResult<Compra>> AgregarCompra([FromBody] Compra compra)
        {
            if (compra == null || compra.Detalles == null || !compra.Detalles.Any())
                return BadRequest("Compra inválida: debe tener al menos un detalle");

            var proveedorExiste = await _baseDatos.Proveedores.AnyAsync(p => p.Id == compra.ProveedorId);
            if (!proveedorExiste)
                return BadRequest($"El proveedor con ID {compra.ProveedorId} no existe.");

            var detalles = compra.Detalles.ToList();
            compra.Detalles = null;

            _baseDatos.Compras.Add(compra);
            await _baseDatos.SaveChangesAsync(); 

            foreach (var detalle in detalles)
            {
                if (detalle.PiezaId == 0)
                    return BadRequest("Debe incluir piezaId en cada detalle.");

                var ultMovimiento = await _baseDatos.MovimientosPieza
                    .Where(m => m.PiezaId == detalle.PiezaId)
                    .OrderByDescending(m => m.Fecha)
                    .FirstOrDefaultAsync();

                var nuevaExistencia = (ultMovimiento?.Existencias ?? 0) + detalle.Cantidad;
                var nuevoSaldo = (ultMovimiento?.SaldoValor ?? 0) + detalle.PrecioTotal;
                var nuevoPromedio = nuevoSaldo / (decimal)nuevaExistencia;

                var movimiento = new MovimientosPieza
                {
                    PiezaId = detalle.PiezaId,
                    TipoMovimiento = "Entrada",
                    Cantidad = detalle.Cantidad,
                    CostoUnitario = detalle.PrecioTotal / detalle.Cantidad,
                    CostoPromedio = nuevoPromedio,
                    ValorDebe = detalle.PrecioTotal,
                    ValorHaber = 0,
                    SaldoValor = nuevoSaldo,
                    Existencias = nuevaExistencia
                };

                _baseDatos.MovimientosPieza.Add(movimiento);
                await _baseDatos.SaveChangesAsync(); 

                detalle.MovimientosPiezaId = movimiento.Id;
                detalle.CompraId = compra.Id;
                _baseDatos.DetalleCompra.Add(detalle);
            }

            await _baseDatos.SaveChangesAsync();

            return Ok(compra);
        }



    }
}
