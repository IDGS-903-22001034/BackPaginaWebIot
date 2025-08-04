using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public VentasController(AppDbContext context)
        {
            _baseDatos = context;
        }

        // GET: api/ListaVentas
        [HttpGet]
        [Route("ListaVentas")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _baseDatos.Ventas
                .Include(v => v.Detalles)
                .Include(v => v.Detalles)
                .Include(v => v.Usuario)
                .ToListAsync();
        }
        // POST: api/Ventas/AgregarVenta
        [HttpPost]
        [Route("AgregarVenta")]
        public async Task<ActionResult<Venta>> AgregarVenta([FromBody] Venta venta)
        {
            if (venta == null || venta.Detalles == null || !venta.Detalles.Any())
                return BadRequest("La venta debe tener al menos un detalle.");

            decimal totalVenta = 0;

            foreach (var detalle in venta.Detalles)
            {
                    var producto = await _baseDatos.Productos
                        .Include(p => p.ComponentesProducto)
                        .ThenInclude(cp => cp.Pieza)
                    .FirstOrDefaultAsync(p => p.Id == detalle.ProductoId);

                if (producto == null)
                    return BadRequest($"Producto con ID {detalle.ProductoId} no existe.");

                foreach (var componente in producto.ComponentesProducto)
                {
                    var piezaId = componente.PiezaId;
                    var cantidadRequeridaTotal = componente.CantidadRequerida * detalle.Cantidad;

                    var ultimoMov = await _baseDatos.MovimientosPieza
                        .Where(m => m.PiezaId == piezaId)
                        .OrderByDescending(m => m.Fecha)
                        .FirstOrDefaultAsync();

                    if (ultimoMov == null || ultimoMov.Existencias < cantidadRequeridaTotal)
                    {
                        return BadRequest($"Stock insuficiente de la pieza '{componente.Pieza.Nombre}' para el producto '{producto.Nombre}'.");
                    }


                    var nuevaExistencia = ultimoMov.Existencias - cantidadRequeridaTotal;
                    var nuevoSaldo = ultimoMov.SaldoValor - (cantidadRequeridaTotal * ultimoMov.CostoPromedio);

                    var movimiento = new MovimientosPieza
                    {
                        PiezaId = piezaId,
                        TipoMovimiento = "Salida",
                        Cantidad = cantidadRequeridaTotal,
                        CostoUnitario = ultimoMov.CostoPromedio,
                        CostoPromedio = ultimoMov.CostoPromedio,
                        ValorDebe = 0,
                        ValorHaber = cantidadRequeridaTotal * ultimoMov.CostoPromedio,
                        SaldoValor = nuevoSaldo,
                        Existencias = nuevaExistencia
                    };

                    _baseDatos.MovimientosPieza.Add(movimiento);
                }

                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                totalVenta += detalle.Subtotal;
            }

            venta.Total = totalVenta;

            _baseDatos.Ventas.Add(venta);
            await _baseDatos.SaveChangesAsync();

            return Ok(venta);
        }
    }
}
