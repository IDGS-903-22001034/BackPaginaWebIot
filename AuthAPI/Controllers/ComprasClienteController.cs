using AuthAPI.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthAPI.Data;
using AuthAPI.Models;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasClienteController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ComprasClienteController(AppDbContext context)
        {
            _baseDatos = context;
        }

        [HttpGet("ListaComprasCliente")]
        public async Task<ActionResult<IEnumerable<CompraClienteDto>>> GetComprasCliente()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var purchases = await _baseDatos.Ventas
                .Where(v => v.UsuarioId == userId)
                .Include(v => v.Detalles)
                    .ThenInclude(dv => dv.Producto)
                .ToListAsync();

            var result = purchases.Select(p => new CompraClienteDto
            {
                Fecha = p.Fecha,
                Total = p.Total,
                Productos = p.Detalles.Select(d => new ProductoDto
                {
                    Nombre = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            });

            return Ok(result);
        }

        [HttpGet("ClienteManualProductos")]
        public async Task<ActionResult<IEnumerable<ManualDto>>> GetProductoManuales()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var productIds = await _baseDatos.Ventas
                .Where(v => v.UsuarioId == userId)
                .SelectMany(v => v.Detalles)
                .Select(dv => dv.ProductoId)
                .Distinct()
                .ToListAsync();

            var manuals = await _baseDatos.Manuales
                .Where(m => productIds.Contains(m.ProductoId))
                .ToListAsync();

            return Ok(manuals.Select(m => new ManualDto
            {
                Titulo = m.Titulo,
                UrlDocumento = m.UrlDocumento
            }));
        }

        [HttpPost("comment")]
        public async Task<IActionResult> PostComentario(ComentarioDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasPurchased = await _baseDatos.Ventas
                .Where(v => v.UsuarioId == userId)
                .SelectMany(v => v.Detalles)
                .AnyAsync(dv => dv.ProductoId == dto.ProductoId);

            if (!hasPurchased)
                return BadRequest("No puedes comentar un producto que no has comprado.");

            var comentario = new Comentario
            {
                ProductoId = dto.ProductoId,
                Descripcion = dto.Descripcion,
                Calificacion = dto.Calificacion,
                Fecha = DateTime.UtcNow
            };

            _baseDatos.Comentarios.Add(comentario);
            await _baseDatos.SaveChangesAsync();

            return Ok("Comentario guardado correctamente.");
        }



    }
}
