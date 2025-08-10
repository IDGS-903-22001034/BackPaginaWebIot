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
                    productoId = d.Producto.Id,
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

        [HttpPost("comentario")]
        public async Task<IActionResult> PostComentario(ComentarioDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verificar que la venta pertenece al usuario
            var venta = await _baseDatos.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == dto.VentaId && v.UsuarioId == userId);

            if (venta == null)
                return BadRequest("No se encontró la venta o no tienes permisos para comentar sobre ella.");

            // Verificar que el producto está en esa venta
            var detalleVenta = venta.Detalles.FirstOrDefault(d => d.ProductoId == dto.ProductoId);
            if (detalleVenta == null)
                return BadRequest("El producto no está incluido en esta venta.");

            // Verificar que no hay comentario previo para este producto en esta venta
            var comentarioExistente = await _baseDatos.Comentarios
                .FirstOrDefaultAsync(c => c.VentaId == dto.VentaId && c.ProductoId == dto.ProductoId);

            if (comentarioExistente != null)
                return BadRequest("Ya existe un comentario para este producto en esta venta.");

            var comentario = new Comentario
            {
                VentaId = dto.VentaId,
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
