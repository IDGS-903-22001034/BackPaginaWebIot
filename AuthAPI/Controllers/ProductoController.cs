using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ProductoController(AppDbContext context)
        {
            _baseDatos = context;
        }
        // GET: api/ListaProductos
        [HttpGet]
        [Route("ListaProductos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _baseDatos.Productos
                .Include(p => p.ComponentesProducto)
                .Include(p => p.Comentarios)
                .Include(p => p.Manuales)
                .Include(p => p.DetallesVenta)
                .ToListAsync();
        }

        // POST: api/AgregarProducto
        [HttpPost]
        [Route("AgregarProducto")]
        public async Task<ActionResult<Producto>> AgregarProductos([FromBody] Producto producto)
        {
            _baseDatos.Productos.Add(producto);
            await _baseDatos.SaveChangesAsync();
            return Ok(producto);
        }


        // PUT: api/ModificarProducto/5
        [HttpPut]
        [Route("ModificarProducto/{id:int}")]
        public async Task<IActionResult> ModificarProducto(int id, [FromBody] Producto producto)
        {
            var productoExistente = await _baseDatos.Productos.FindAsync(id);

            if (productoExistente == null)
                return NotFound("Producto no encontrado");

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.PrecioSugerido = producto.PrecioSugerido;
            productoExistente.Imagen = producto.Imagen;

            await _baseDatos.SaveChangesAsync();
            return Ok(producto);
        }

        // DELETE: api/EliminarProducto/5
        [HttpDelete]
        [Route("EliminarProducto/{id:int}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _baseDatos.Productos.FindAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            _baseDatos.Productos.Remove(producto);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
