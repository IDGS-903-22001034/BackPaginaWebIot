using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/ListaComentarios
        [HttpGet]
        [Route("ListaComentarios")]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _baseDatos.Comentarios
                .Include(c => c.Producto)
                .ToListAsync();
        }

        // GET: api/getPorProducto/5
        [HttpGet]
        [Route("ObtenerPorProducto/{productoId:int}")]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosPorProducto(int productoId)
        {
            var comentarios = await _baseDatos.Comentarios
                .Where(c => c.ProductoId == productoId)
                .Include(c => c.Producto)
                .ToListAsync();

            return comentarios;
        }

        // POST: api/AgregarComentario
        [HttpPost]
        [Route("AgregarComentario")]
        public async Task<ActionResult<Comentario>> AgregarComentario([FromBody] Comentario comentario)
        {
            _baseDatos.Comentarios.Add(comentario);
            await _baseDatos.SaveChangesAsync();
            return Ok(comentario);
        }

        // PUT: api/ModificarComentario/5
        [HttpPut]
        [Route("ModificarComentario/{id:int}")]
        public async Task<IActionResult> ModificarComentario(int id, [FromBody] Comentario comentario)
        {
            var comentarioExistente = await _baseDatos.Comentarios.FindAsync(id);

            if (comentarioExistente == null)
                return NotFound("Comentario no encontrado");

            comentarioExistente.Descripcion = comentario.Descripcion;
            comentarioExistente.Calificacion = comentario.Calificacion;
            comentarioExistente.Fecha = DateTime.Now;

            await _baseDatos.SaveChangesAsync();
            return Ok(comentarioExistente);
        }

        // DELETE: api/EliminarComentario/5
        [HttpDelete]
        [Route("EliminarComentario/{id:int}")]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            var comentario = await _baseDatos.Comentarios.FindAsync(id);

            if (comentario == null)
                return NotFound("Comentario no encontrado");

            _baseDatos.Comentarios.Remove(comentario);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
