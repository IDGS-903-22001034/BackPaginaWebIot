using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentesProductoController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ComponentesProductoController(AppDbContext context)
        {
            _baseDatos = context;
        }
        // GET: api/ListaComponentesProductos
        [HttpGet]
        [Route("ListaComponentesProductos")]
        public async Task<ActionResult<IEnumerable<ComponentesProducto>>> GetComponentesProductos()
        {
            return await _baseDatos.ComponentesProducto
                .Include(cp => cp.Producto)
                .Include(p => p.Pieza)
                .ToListAsync();

        }

        // POST: api/AgregarComponentesProducto
        [HttpPost]
        [Route("AgregarComponentesProducto")]
        public async Task<ActionResult<ComponentesProducto>> AgregarComponentesProducto([FromBody] ComponentesProducto componentesProducto)
        {
            _baseDatos.ComponentesProducto.Add(componentesProducto);
            await _baseDatos.SaveChangesAsync();
            return Ok(componentesProducto);
        }

        // PUT: api/ModificarComentario/5
        [HttpPut]
        [Route("ModificarComponentesProducto/{id:int}")]
        public async Task<IActionResult> ModificarComponentesProducto(int id, [FromBody] ComponentesProducto componentesProducto)
        {
            var componenteProductoExistente = await _baseDatos.ComponentesProducto.FindAsync(id);

            if (componenteProductoExistente == null)
                return NotFound("Receta no encontrada");

            componenteProductoExistente.CantidadRequerida = componentesProducto.CantidadRequerida;

            await _baseDatos.SaveChangesAsync();
            return Ok(componenteProductoExistente);
        }

        // DELETE: api/EliminarComponentesProducto/5
        [HttpDelete]
        [Route("EliminarComponentesProducto/{id:int}")]
        public async Task<IActionResult> EliminarComponentesProducto(int id)
        {
            var componentesProducto = await _baseDatos.ComponentesProducto.FindAsync(id);

            if (componentesProducto == null)
                return NotFound("Receta no encontrado");

            _baseDatos.ComponentesProducto.Remove(componentesProducto);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
