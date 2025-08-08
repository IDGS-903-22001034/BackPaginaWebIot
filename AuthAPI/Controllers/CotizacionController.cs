using AuthAPI.Data;
using AuthAPI.Dtos;
using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ICotizacionService _cotizacionService;
        private readonly UserManager<AppUser> _userManager;

        public CotizacionController(
            AppDbContext context,
            IEmailService emailService,
            ICotizacionService cotizacionService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _cotizacionService = cotizacionService;
            _userManager = userManager;
        }

        // POST: api/cotizacion/solicitar
        [HttpPost("solicitar")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarCotizacion([FromBody] CotizacionDto dto)
        {
            try
            {
                // Verificar si ya existe una cotización pendiente para este email
                var existente = await _context.Cotizaciones
                    .FirstOrDefaultAsync(c => c.EmailCliente == dto.EmailCliente &&
                                            c.Estado == "Pendiente");

                if (existente != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ya existe una cotización pendiente para este email"
                    });
                }

                // Calcular precio automático
                var precioCalculado = _cotizacionService.CalcularPrecio(dto);

                // Crear nueva cotización
                var cotizacion = new Cotizacion
                {
                    NombreCliente = dto.NombreCliente,
                    EmailCliente = dto.EmailCliente,
                    Telefono = dto.Telefono,
                    Empresa = dto.Empresa,
                    CantidadDispositivos = dto.CantidadDispositivos,
                    CantidadAnimales = dto.CantidadAnimales,
                    TipoGanado = dto.TipoGanado,
                    Hectareas = dto.Hectareas,
                    FuncionalidadesRequeridas = JsonSerializer.Serialize(dto.FuncionalidadesRequeridas),
                    Comentarios = dto.Comentarios,
                    PrecioCalculado = precioCalculado,
                    Estado = "Pendiente",
                    ClienteId = null,
                    ClienteCreado = false,
                    FechaSolicitud = DateTime.Now
                };


                _context.Cotizaciones.Add(cotizacion);
                await _context.SaveChangesAsync();

                // Notificar a administradores
                await _emailService.SendNotificationToAdmin(
                    "Nueva Cotización Recibida",
                    $"Se ha recibido una nueva cotización de {dto.NombreCliente} ({dto.EmailCliente}). " +
                    $"Dispositivos: {dto.CantidadDispositivos}, Precio estimado: ${precioCalculado:N2}"
                );

                return Ok(new
                {
                    success = true,
                    cotizacionId = cotizacion.Id,
                    precioEstimado = precioCalculado,
                    message = "Cotización recibida exitosamente. Un ejecutivo le contactará pronto."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al procesar la cotización",
                    error = ex.Message
                });
            }
        }

        // GET: api/cotizacion/listar
        [HttpGet("listar")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ListarCotizaciones(
            [FromQuery] string estado = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Cotizaciones.AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(c => c.Estado == estado);
            }

            var total = await query.CountAsync();

            var cotizaciones = await query
                .OrderByDescending(c => c.FechaSolicitud)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CotizacionResponseDto
                {
                    Id = c.Id,
                    NombreCliente = c.NombreCliente,
                    EmailCliente = c.EmailCliente,
                    Telefono = c.Telefono,
                    Empresa = c.Empresa,
                    CantidadDispositivos = c.CantidadDispositivos,
                    CantidadAnimales = c.CantidadAnimales,
                    TipoGanado = c.TipoGanado,
                    Hectareas = c.Hectareas,
                    FuncionalidadesRequeridas = JsonSerializer.Deserialize<List<string>>(
                        c.FuncionalidadesRequeridas,
                        (JsonSerializerOptions?)null
                    ),
                    Comentarios = c.Comentarios,
                    FechaSolicitud = c.FechaSolicitud,
                    Estado = c.Estado,
                    PrecioCalculado = c.PrecioCalculado,
                    PrecioFinal = c.PrecioFinal,
                    NotasInternas = c.NotasInternas,
                    UsuarioAsignado = c.UsuarioAsignado != null ? c.UsuarioAsignado.FullName : null,
                    FechaEnvio = c.FechaEnvio,
                    FechaRespuesta = c.FechaRespuesta,
                    ClienteCreado = c.ClienteCreado
                })
                .ToListAsync();

            return Ok(new
            {
                cotizaciones,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        // GET: api/cotizacion/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ObtenerCotizacion(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.UsuarioAsignado)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cotizacion == null)
            {
                return NotFound(new { message = "Cotización no encontrada" });
            }

            var dto = new CotizacionResponseDto
            {
                Id = cotizacion.Id,
                NombreCliente = cotizacion.NombreCliente,
                EmailCliente = cotizacion.EmailCliente,
                Telefono = cotizacion.Telefono,
                Empresa = cotizacion.Empresa,
                CantidadDispositivos = cotizacion.CantidadDispositivos,
                CantidadAnimales = cotizacion.CantidadAnimales,
                TipoGanado = cotizacion.TipoGanado,
                Hectareas = cotizacion.Hectareas,
                FuncionalidadesRequeridas = JsonSerializer.Deserialize<List<string>>(cotizacion.FuncionalidadesRequeridas),
                Comentarios = cotizacion.Comentarios,
                FechaSolicitud = cotizacion.FechaSolicitud,
                Estado = cotizacion.Estado,
                PrecioCalculado = cotizacion.PrecioCalculado,
                PrecioFinal = cotizacion.PrecioFinal,
                NotasInternas = cotizacion.NotasInternas,
                UsuarioAsignado = cotizacion.UsuarioAsignado?.FullName,
                FechaEnvio = cotizacion.FechaEnvio,
                FechaRespuesta = cotizacion.FechaRespuesta,
                ClienteCreado = cotizacion.ClienteCreado
            };

            return Ok(dto);
        }

        // POST: api/cotizacion/enviar
        [HttpPost("enviar")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> EnviarCotizacion([FromBody] EnviarCotizacionDto dto)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(dto.CotizacionId);

            if (cotizacion == null)
            {
                return NotFound(new { message = "Cotización no encontrada" });
            }

            if (cotizacion.Estado != "Pendiente")
            {
                return BadRequest(new { message = "Esta cotización ya fue procesada" });
            }

            // Actualizar cotización
            cotizacion.PrecioFinal = dto.PrecioFinal;
            cotizacion.NotasInternas = dto.NotasAdicionales;
            cotizacion.Estado = "Enviada";
            cotizacion.FechaEnvio = DateTime.Now;
            cotizacion.UsuarioAsignadoId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await _context.SaveChangesAsync();

            // Enviar email con la cotización
            var emailEnviado = await _emailService.SendCotizacionEmail(cotizacion);

            if (!emailEnviado)
            {
                return StatusCode(500, new { message = "Error al enviar el email" });
            }

            return Ok(new
            {
                success = true,
                message = "Cotización enviada exitosamente"
            });
        }

        // POST: api/cotizacion/{id}/aceptar
        [HttpPost("{id}/aceptar")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> AceptarCotizacion(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null)
            {
                return NotFound(new { message = "Cotización no encontrada" });
            }

            if (cotizacion.Estado != "Enviada")
            {
                return BadRequest(new { message = "La cotización debe estar en estado 'Enviada'" });
            }

            cotizacion.Estado = "Aceptada";
            cotizacion.FechaRespuesta = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cotización marcada como aceptada"
            });
        }

        // POST: api/cotizacion/{id}/crear-cliente
        [HttpPost("{id}/crear-cliente")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CrearClienteDesdeCotizacion(int id, [FromBody] CrearClienteDesdeeCotizacionDto dto)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null)
            {
                return NotFound(new { message = "Cotización no encontrada" });
            }

            if (cotizacion.ClienteCreado)
            {
                return BadRequest(new { message = "Ya se creó un cliente para esta cotización" });
            }

            if (cotizacion.Estado != "Aceptada")
            {
                return BadRequest(new { message = "La cotización debe estar aceptada" });
            }

            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(cotizacion.EmailCliente);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Ya existe un usuario con este email" });
            }

            // Generar contraseña si no se proporciona
            var password = string.IsNullOrEmpty(dto.Password)
                ? _cotizacionService.GenerarPasswordAleatorio()
                : dto.Password;

            // Crear usuario
            var user = new AppUser
            {
                UserName = cotizacion.EmailCliente,
                Email = cotizacion.EmailCliente,
                FullName = cotizacion.NombreCliente,
                PhoneNumber = cotizacion.Telefono,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Error al crear el usuario",
                    errors = result.Errors
                });
            }

            // Asignar rol de Cliente
            await _userManager.AddToRoleAsync(user, "Client");

            // Actualizar cotización
            cotizacion.ClienteCreado = true;
            cotizacion.ClienteId = user.Id;
            cotizacion.Estado = "Convertida";

            // Crear venta automática
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                UsuarioId = user.Id,
                Total = cotizacion.PrecioFinal ?? cotizacion.PrecioCalculado ?? 0,
                Estado = "Pendiente",
                Detalles = new List<DetalleVenta>
                {
                    new DetalleVenta
                    {
                        ProductoId = 1, // ID del producto GPS principal
                        Cantidad = cotizacion.CantidadDispositivos,
                        PrecioUnitario = (cotizacion.PrecioFinal ?? cotizacion.PrecioCalculado ?? 0) / cotizacion.CantidadDispositivos,
                        Subtotal = cotizacion.PrecioFinal ?? cotizacion.PrecioCalculado ?? 0
                    }
                }
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            cotizacion.VentaId = venta.Id;
            await _context.SaveChangesAsync();

            // Enviar email de bienvenida con credenciales
            await _emailService.SendWelcomeEmail(user.Email, user.FullName, password);

            return Ok(new
            {
                success = true,
                message = "Cliente creado exitosamente",
                userId = user.Id,
                ventaId = venta.Id,
                email = user.Email
            });
        }

        // PUT: api/cotizacion/{id}/actualizar-estado
        [HttpPut("{id}/actualizar-estado")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoDto dto)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null)
            {
                return NotFound(new { message = "Cotización no encontrada" });
            }

            cotizacion.Estado = dto.Estado;
            cotizacion.NotasInternas = dto.Notas;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Estado actualizado"
            });
        }

        // GET: api/cotizacion/estadisticas
        [HttpGet("estadisticas")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            var stats = new
            {
                Total = await _context.Cotizaciones.CountAsync(),
                Pendientes = await _context.Cotizaciones.CountAsync(c => c.Estado == "Pendiente"),
                Enviadas = await _context.Cotizaciones.CountAsync(c => c.Estado == "Enviada"),
                Aceptadas = await _context.Cotizaciones.CountAsync(c => c.Estado == "Aceptada"),
                Convertidas = await _context.Cotizaciones.CountAsync(c => c.Estado == "Convertida"),
                Rechazadas = await _context.Cotizaciones.CountAsync(c => c.Estado == "Rechazada"),
                ValorTotal = await _context.Cotizaciones
                    .Where(c => c.Estado == "Convertida")
                    .SumAsync(c => c.PrecioFinal ?? c.PrecioCalculado ?? 0),
                TasaConversion = await CalcularTasaConversion()
            };

            return Ok(stats);
        }

        private async Task<double> CalcularTasaConversion()
        {
            var total = await _context.Cotizaciones.CountAsync();
            if (total == 0) return 0;

            var convertidas = await _context.Cotizaciones.CountAsync(c => c.Estado == "Convertida");
            return (double)convertidas / total * 100;
        }
    }

    public class ActualizarEstadoDto
    {
        public string Estado { get; set; }
        public string Notas { get; set; }
    }
}