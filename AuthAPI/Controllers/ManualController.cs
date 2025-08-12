using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public ManualController(AppDbContext context)
        {
            _baseDatos = context;
        }
        // GET: api/ListaManuales
        [HttpGet]
        [Route("ListaManuales")]
        public async Task<ActionResult<IEnumerable<Manual>>> GetManuales()
        {
            return await _baseDatos.Manuales
                .Include(c => c.Producto)
                .ToListAsync();
        }
        

        // POST: api/AgregarManual
        [HttpPost]
        [Route("AgregarManual")]
        public async Task<ActionResult<Comentario>> AgregarManual([FromBody] Manual manual)
        {
            _baseDatos.Manuales.Add(manual);
            await _baseDatos.SaveChangesAsync();
            return Ok(manual);
        }

        

        // PUT: api/ModificarManual/5
        [HttpPut]
        [Route("ModificarManual/{id:int}")]
        public async Task<IActionResult> ModificarManual(int id, [FromBody] Manual manual)
        {
            var manualExistente = await _baseDatos.Manuales.FindAsync(id);

            if (manualExistente == null)
                return NotFound("Manual no encontrado");

            manualExistente.Titulo = manual.Titulo;
            manualExistente.UrlDocumento = manual.UrlDocumento;

            await _baseDatos.SaveChangesAsync();
            return Ok(manualExistente);
        }

        // DELETE: api/EliminarManual/5
        [HttpDelete]
        [Route("EliminarManual/{id:int}")]
        public async Task<IActionResult> EliminarManual(int id)
        {
            var manual = await _baseDatos.Manuales.FindAsync(id);

            if (manual == null)
                return NotFound("Manual no encontrado");

            _baseDatos.Manuales.Remove(manual);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
