using AuthAPI.Data;
using AuthAPI.Dtos;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ComentariosController(AppDbContext context)
        {
            _baseDatos = context;
        }

        // GET: api/Comentarios/ListaComentarios
        [HttpGet]
        [Route("ListaComentarios")]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentarios()
        {
            var comentarios = await _baseDatos.Comentarios
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Usuario)
                .Include(c => c.Producto)
                .Select(c => new ComentarioResponseDto
                {
                    Id = c.Id,
                    VentaId = c.VentaId,
                    ProductoId = c.ProductoId,
                    NombreProducto = c.Producto.Nombre,
                    Descripcion = c.Descripcion,
                    Calificacion = c.Calificacion,
                    Fecha = c.Fecha,
                    NombreCliente = c.Venta.Usuario.FullName
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // GET: api/Comentarios/ObtenerPorProducto/5
        [HttpGet]
        [Route("ObtenerPorProducto/{productoId:int}")]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosPorProducto(int productoId)
        {
            var comentarios = await _baseDatos.Comentarios
                .Where(c => c.ProductoId == productoId)
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Usuario)
                .Include(c => c.Producto)
                .Select(c => new ComentarioResponseDto
                {
                    Id = c.Id,
                    VentaId = c.VentaId,
                    ProductoId = c.ProductoId,
                    NombreProducto = c.Producto.Nombre,
                    Descripcion = c.Descripcion,
                    Calificacion = c.Calificacion,
                    Fecha = c.Fecha,
                    NombreCliente = c.Venta.Usuario.FullName
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // GET: api/Comentarios/ObtenerPorVenta/5
        [HttpGet]
        [Route("ObtenerPorVenta/{ventaId:int}")]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentariosPorVenta(int ventaId)
        {
            var comentarios = await _baseDatos.Comentarios
                .Where(c => c.VentaId == ventaId)
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Usuario)
                .Include(c => c.Producto)
                .Select(c => new ComentarioResponseDto
                {
                    Id = c.Id,
                    VentaId = c.VentaId,
                    ProductoId = c.ProductoId,
                    NombreProducto = c.Producto.Nombre,
                    Descripcion = c.Descripcion,
                    Calificacion = c.Calificacion,
                    Fecha = c.Fecha,
                    NombreCliente = c.Venta.Usuario.FullName
                })
                .ToListAsync();

            return Ok(comentarios);
        }

        // POST: api/Comentarios/AgregarComentario
        [HttpPost]
        [Route("AgregarComentario")]
        [Authorize]
        public async Task<ActionResult<Comentario>> AgregarComentario([FromBody] ComentarioDto comentarioDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verificar que la venta pertenece al usuario autenticado
            var venta = await _baseDatos.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == comentarioDto.VentaId && v.UsuarioId == userId);

            if (venta == null)
            {
                return BadRequest("No se encontró la venta o no tienes permisos para comentar sobre ella.");
            }

            // Verificar que el producto está en esa venta
            var detalleVenta = venta.Detalles.FirstOrDefault(d => d.ProductoId == comentarioDto.ProductoId);
            if (detalleVenta == null)
            {
                return BadRequest("El producto no está incluido en esta venta.");
            }

            // Verificar que no hay comentario previo para este producto en esta venta
            var comentarioExistente = await _baseDatos.Comentarios
                .FirstOrDefaultAsync(c => c.VentaId == comentarioDto.VentaId && c.ProductoId == comentarioDto.ProductoId);

            if (comentarioExistente != null)
            {
                return BadRequest("Ya existe un comentario para este producto en esta venta.");
            }

            var comentario = new Comentario
            {
                VentaId = comentarioDto.VentaId,
                ProductoId = comentarioDto.ProductoId,
                Descripcion = comentarioDto.Descripcion,
                Calificacion = comentarioDto.Calificacion,
                Fecha = DateTime.Now
            };

            _baseDatos.Comentarios.Add(comentario);
            await _baseDatos.SaveChangesAsync();

            return Ok(comentario);
        }

        // PUT: api/Comentarios/ModificarComentario/5
        [HttpPut]
        [Route("ModificarComentario/{id:int}")]
        [Authorize]
        public async Task<IActionResult> ModificarComentario(int id, [FromBody] ComentarioDto comentarioDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comentarioExistente = await _baseDatos.Comentarios
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comentarioExistente == null)
                return NotFound("Comentario no encontrado");

            // Verificar que el comentario pertenece al usuario autenticado
            if (comentarioExistente.Venta.UsuarioId != userId)
                return StatusCode(403, "No tienes permisos para modificar este comentario");

            comentarioExistente.Descripcion = comentarioDto.Descripcion;
            comentarioExistente.Calificacion = comentarioDto.Calificacion;
            comentarioExistente.Fecha = DateTime.Now;

            await _baseDatos.SaveChangesAsync();
            return Ok(comentarioExistente);
        }

        // DELETE: api/Comentarios/EliminarComentario/5
        [HttpDelete]
        [Route("EliminarComentario/{id:int}")]
        [Authorize]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comentario = await _baseDatos.Comentarios
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comentario == null)
                return NotFound("Comentario no encontrado");

            // Verificar que el comentario pertenece al usuario autenticado o es admin
            var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            bool isAdmin = userRoles.Contains("Admin");

            if (comentario.Venta.UsuarioId != userId && !isAdmin)
                return StatusCode(403, "No tienes permisos para eliminar este comentario");

            _baseDatos.Comentarios.Remove(comentario);
            await _baseDatos.SaveChangesAsync();
            return Ok("Comentario eliminado exitosamente");
        }

        // GET: api/Comentarios/MisComentarios
        [HttpGet]
        [Route("MisComentarios")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetMisComentarios()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comentarios = await _baseDatos.Comentarios
                .Where(c => c.Venta.UsuarioId == userId)
                .Include(c => c.Venta)
                    .ThenInclude(v => v.Usuario)
                .Include(c => c.Producto)
                .Select(c => new ComentarioResponseDto
                {
                    Id = c.Id,
                    VentaId = c.VentaId,
                    ProductoId = c.ProductoId,
                    NombreProducto = c.Producto.Nombre,
                    Descripcion = c.Descripcion,
                    Calificacion = c.Calificacion,
                    Fecha = c.Fecha,
                    NombreCliente = c.Venta.Usuario.FullName
                })
                .ToListAsync();

            return Ok(comentarios);
        }
    }
}