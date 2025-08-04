using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PiezaController : ControllerBase
    {
        private readonly AppDbContext _baseDatos;

        public PiezaController(AppDbContext context)
        {
            _baseDatos = context;
        }

        //Método GET ListaPiezas
        [HttpGet]
        [Route("ListaPiezas")]
        public async Task<IActionResult> ListaPiezas()
        {
            var listaPiezas = await _baseDatos.Piezas.ToListAsync();
            return Ok(listaPiezas);
        }

        // GET: api/getPieza/5
        [HttpGet]
        [Route("getPieza/{id:int}")]
        public async Task<ActionResult<Pieza>> GetPieza(int id)
        {
            var pieza = await _baseDatos.Piezas.FindAsync(id);

            if (pieza == null)
            {
                return NotFound();
            }

            return pieza;
        }

        // POST: api/AgregarPiezas
        [HttpPost]
        [Route("AgregarPiezas")]
        public async Task<ActionResult<Pieza>> AgregarPiezas([FromBody] Pieza request)
        {
            await _baseDatos.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        // PUT: api/ModificarPieza/5
        [HttpPut]
        [Route("ModificarPieza/{id:int}")]
        public async Task<IActionResult> ModificarPieza(int id, [FromBody] Pieza request)
        {
            var piezaModificar = await _baseDatos.Piezas.FindAsync(id);

            if (piezaModificar == null)
            {
                return BadRequest("No existe el pieza");
            }

            piezaModificar.Nombre = request.Nombre;
            piezaModificar.UnidadMedida = request.UnidadMedida;
            piezaModificar.Descripcion = request.Descripcion;

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

        // DELETE: api/EliminarPieza/5
        [HttpDelete]
        [Route("EliminarPieza/{id:int}")]
        public async Task<IActionResult> EliminarPieza(int id)
        {
            var piezaEliminar = await _baseDatos.Piezas.FindAsync(id);

            if (piezaEliminar == null)
            {
                return BadRequest("No existe la pieza");
            }

            _baseDatos.Piezas.Remove(piezaEliminar);
            await _baseDatos.SaveChangesAsync();
            return Ok();
        }
    }
}
