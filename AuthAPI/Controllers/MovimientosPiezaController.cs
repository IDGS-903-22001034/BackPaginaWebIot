using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosPiezaController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public MovimientosPiezaController(AppDbContext context)
        {
            _baseDatos = context;
        }

        // GET: api/ListaMovimientosPieza
        [HttpGet]
        [Route("ListaMovimientosPieza")]
        public async Task<ActionResult<IEnumerable<MovimientosPieza>>> GetMovimientosPieza()
        {
            return await _baseDatos.MovimientosPieza
               .Include(m => m.Pieza)
               .ToListAsync();
        }

        // POST: api/AgregarMovimentoPieza
        [HttpPost]
        [Route("AgregarMovimentoPieza")]
        public async Task<ActionResult<MovimientosPieza>> AgregarMovimientoPieza([FromBody] MovimientosPieza movimiento)
        {
            var pieza = await _baseDatos.Piezas.FindAsync(movimiento.PiezaId);
            if (pieza == null)
                return BadRequest($"La pieza con ID {movimiento.PiezaId} no existe.");

            var ultimoMovimiento = await _baseDatos.MovimientosPieza
                .Where(m => m.PiezaId == movimiento.PiezaId)
                .OrderByDescending(m => m.Fecha)
                .FirstOrDefaultAsync();

            decimal costoPromedioAnterior = ultimoMovimiento?.CostoPromedio ?? 0;
            float existenciasAnterior = ultimoMovimiento?.Existencias ?? 0;
            decimal saldoAnterior = ultimoMovimiento?.SaldoValor ?? 0;

            if (movimiento.TipoMovimiento == "Entrada")
            {
                movimiento.ValorDebe = movimiento.CostoUnitario.GetValueOrDefault() * movimiento.Cantidad;
                movimiento.ValorHaber = 0;
                movimiento.SaldoValor = saldoAnterior + movimiento.ValorDebe;

                movimiento.Existencias = existenciasAnterior + movimiento.Cantidad;
                movimiento.CostoPromedio = movimiento.SaldoValor / (decimal)movimiento.Existencias;
            }
            else if (movimiento.TipoMovimiento == "Salida")
            {
                if (movimiento.Cantidad > existenciasAnterior)
                {
                    return BadRequest($"No hay existencias suficientes. Actualmente disponibles: {existenciasAnterior} unidades.");
                }

                movimiento.ValorDebe = 0;
                movimiento.ValorHaber = costoPromedioAnterior * movimiento.Cantidad;
                movimiento.SaldoValor = saldoAnterior - movimiento.ValorHaber;

                movimiento.Existencias = existenciasAnterior - movimiento.Cantidad;
                movimiento.CostoPromedio = costoPromedioAnterior;
                movimiento.CostoUnitario = costoPromedioAnterior;
            }
            else
            {
                return BadRequest("TipoMovimiento debe ser 'Entrada' o 'Salida'.");
            }

            _baseDatos.MovimientosPieza.Add(movimiento);
            await _baseDatos.SaveChangesAsync();

            return Ok(movimiento);
        }

    }
}
