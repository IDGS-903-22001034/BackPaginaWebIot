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

        [HttpPost]
        [Route("AgregarProductoConManual")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AgregarProductoConManual(
    [FromForm] string nombre,
    [FromForm] string descripcion,
    [FromForm] decimal precioSugerido,
    [FromForm] string imagen,
    [FromForm] string? tituloManual,
    [FromForm] IFormFile? archivoManual)
        {
            var producto = new Producto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                PrecioSugerido = precioSugerido,
                Imagen = imagen
            };

            _baseDatos.Productos.Add(producto);
            await _baseDatos.SaveChangesAsync();

            if (archivoManual != null && archivoManual.Length > 0 && !string.IsNullOrEmpty(tituloManual))
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "manuales");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var fileName = Path.GetFileName(archivoManual.FileName);
                var rutaCompleta = Path.Combine(carpeta, fileName);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivoManual.CopyToAsync(stream);
                }

                var manual = new Manual
                {
                    ProductoId = producto.Id,
                    Titulo = tituloManual,
                    UrlDocumento = $"/manuales/{fileName}"
                };

                _baseDatos.Manuales.Add(manual);
                await _baseDatos.SaveChangesAsync();
            }

            return Ok(producto);
        }



        [HttpPut]
        [Route("ModificarProductoConManual/{id:int}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ModificarProductoConManual(
    int id,
    [FromForm] string nombre,
    [FromForm] string descripcion,
    [FromForm] decimal precioSugerido,
    [FromForm] string imagen,
    [FromForm] string? tituloManual,
    [FromForm] IFormFile? archivoManual)
        {
            var productoExistente = await _baseDatos.Productos
                .Include(p => p.Manuales)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoExistente == null)
                return NotFound("Producto no encontrado");

            productoExistente.Nombre = nombre;
            productoExistente.Descripcion = descripcion;
            productoExistente.PrecioSugerido = precioSugerido;
            productoExistente.Imagen = imagen;

            if (archivoManual != null && archivoManual.Length > 0 && !string.IsNullOrEmpty(tituloManual))
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "manuales");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var fileName = Path.GetFileName(archivoManual.FileName);
                var rutaCompleta = Path.Combine(carpeta, fileName);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivoManual.CopyToAsync(stream);
                }
                var manualExistente = productoExistente.Manuales.FirstOrDefault();

                if (manualExistente != null)
                {
                    manualExistente.Titulo = tituloManual;
                    manualExistente.UrlDocumento = $"/manuales/{fileName}";
                }
                else
                {
                    var manual = new Manual
                    {
                        ProductoId = productoExistente.Id,
                        Titulo = tituloManual,
                        UrlDocumento = $"/manuales/{fileName}"
                    };
                    _baseDatos.Manuales.Add(manual);
                }
            }
            else if (!string.IsNullOrEmpty(tituloManual) && productoExistente.Manuales.Any())
            {
                var manualExistente = productoExistente.Manuales.First();
                manualExistente.Titulo = tituloManual;
            }

            await _baseDatos.SaveChangesAsync();
            return Ok(productoExistente);
        }

        // DELETE: api/EliminarProducto/5
        [HttpDelete]
        [Route("EliminarProductoConManual/{id:int}")]
        public async Task<IActionResult> EliminarProductoConManual(int id)
        {
            var producto = await _baseDatos.Productos
                .Include(p => p.Manuales)
                .Include(p => p.ComponentesProducto)
                .Include(p => p.Comentarios)
                .Include(p => p.DetallesVenta)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            if (producto.Manuales.Any())
            {
                foreach (var manual in producto.Manuales)
                {
                    if (!string.IsNullOrEmpty(manual.UrlDocumento))
                    {
                        var rutaArchivo = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            manual.UrlDocumento.TrimStart('/'));

                        if (System.IO.File.Exists(rutaArchivo))
                        {
                            System.IO.File.Delete(rutaArchivo);
                        }
                    }
                    _baseDatos.Manuales.Remove(manual);
                }
            }

            _baseDatos.Productos.Remove(producto);
            await _baseDatos.SaveChangesAsync();

            return Ok(new
            {
                message = "Producto y manual asociado eliminados correctamente",
                productoId = id
            });
        }
    }
}
