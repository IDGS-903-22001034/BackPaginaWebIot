using AuthAPI.Data;
using AuthAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ProveedoresController(AppDbContext context)
        {
            _baseDatos = context;
        }

        // GET: api/ListaProveedores
        [HttpGet]
        [Route("ListaProveedores")]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _baseDatos.Proveedores.ToListAsync();
        }

        // GET: api/getProveedor/5
        [HttpGet]
        [Route("getProveedor/{id:int}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _baseDatos.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return proveedor;
        }

        // POST: api/AgregarProveedores
        [HttpPost]
        [Route("AgregarProveedores")]
        public async Task<ActionResult<Proveedor>> AgregarProveedor([FromBody] Proveedor request)
        {
            await _baseDatos.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        // PUT: api/ModificarProveedor/5
        [HttpPut]
        [Route("ModificarProveedor/{id:int}")]
        public async Task<IActionResult> EditarProveedor(int id, [FromBody] Proveedor request)
        {
            var espacioModificar = await _baseDatos.Proveedores.FindAsync(id);

            if (espacioModificar == null)
            {
                return BadRequest("No existe el espacio");
            }

            espacioModificar.NombreEmpresa = request.NombreEmpresa;
            espacioModificar.Contacto = request.Contacto;
            espacioModificar.Telefono = request.Telefono;
            espacioModificar.Email = request.Email;

            try
            {
                await _baseDatos.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE: api/EliminarProveedor/5
        [HttpDelete]
        [Route("EliminarProveedor/{id:int}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedorEliminar = await _baseDatos.Proveedores.FindAsync(id);

            if (proveedorEliminar == null)
            {
                return BadRequest("No existe el producto");
            }

            _baseDatos.Proveedores.Remove(proveedorEliminar);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
