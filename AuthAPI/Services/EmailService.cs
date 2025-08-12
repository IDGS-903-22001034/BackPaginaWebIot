using AuthAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AuthAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendCotizacionEmail(Cotizacion cotizacion);
        Task<bool> SendWelcomeEmail(string email, string fullName, string password);
        Task<bool> SendNotificationToAdmin(string subject, string message);
        Task<bool> SendCotizacionNotificationToAdmin(AuthAPI.Dtos.CotizacionDto cotizacion, decimal precioCalculado);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["Email:SmtpHost"];
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            _smtpUser = _configuration["Email:SmtpUser"];
            _smtpPassword = _configuration["Email:SmtpPassword"];
            _fromEmail = _configuration["Email:FromEmail"];
            _fromName = _configuration["Email:FromName"];
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };

                    message.To.Add(to);

                    await client.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        // NUEVO: Método específico para notificaciones de cotización con diseño mejorado
        public async Task<bool> SendCotizacionNotificationToAdmin(AuthAPI.Dtos.CotizacionDto cotizacion, decimal precioCalculado)
        {
            var template = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background: #f8f9fa; }}
        .email-container {{ max-width: 600px; margin: 0 auto; background: white; }}
        .header {{ background: linear-gradient(135deg, #2c5530 0%, #4a7c59 100%); color: white; padding: 25px; text-align: center; }}
        .content {{ padding: 30px; }}
        .client-info {{ background: #f8f9fa; border-left: 4px solid #4a7c59; padding: 20px; margin-bottom: 20px; border-radius: 0 8px 8px 0; }}
        .project-info {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 20px; margin-bottom: 20px; border-radius: 0 8px 8px 0; }}
        .project-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-top: 15px; }}
        .project-card {{ background: white; padding: 12px; border-radius: 6px; border: 1px solid rgba(255,193,7,0.3); text-align: center; }}
        .card-label {{ font-size: 12px; color: #856404; font-weight: 600; text-transform: uppercase; }}
        .card-value {{ font-size: 20px; font-weight: bold; color: #2c5530; margin-top: 5px; }}
        .price-card .card-value {{ color: #28a745; }}
        .features {{ background: #e8f5e8; padding: 15px; border-radius: 8px; margin-bottom: 20px; }}
        .comments {{ background: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #dee2e6; }}
        .action-box {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 20px; border-radius: 8px; text-align: center; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #6c757d; border-top: 1px solid #dee2e6; }}
        .info-row {{ margin: 8px 0; }}
        .label {{ font-weight: 600; color: #2c5530; }}
        h1 {{ margin: 0; font-size: 24px; }}
        h2 {{ margin: 0 0 15px 0; color: #2c5530; font-size: 18px; }}
        h3 {{ margin: 0 0 10px 0; color: #2c5530; font-size: 16px; }}
        p {{ margin: 5px 0; }}
        .subtitle {{ margin: 5px 0 0 0; opacity: 0.9; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>🚨 Nueva Cotización</h1>
            <p class='subtitle'>Requiere revisión inmediata</p>
        </div>
        
        <div class='content'>
            <!-- Información del Cliente -->
            <div class='client-info'>
                <h2>👤 Información del Cliente</h2>
                <div class='info-row'>
                    <span class='label'>Nombre:</span> {cotizacion.NombreCliente}
                </div>
                <div class='info-row'>
                    <span class='label'>Email:</span> {cotizacion.EmailCliente}
                </div>
                <div class='info-row'>
                    <span class='label'>Empresa:</span> {cotizacion.Empresa ?? "No especificada"}
                </div>
                <div class='info-row'>
                    <span class='label'>Teléfono:</span> {cotizacion.Telefono ?? "No proporcionado"}
                </div>
            </div>

            <!-- Detalles del Proyecto -->
            <div class='project-info'>
                <h2>📊 Detalles del Proyecto</h2>
                <div class='project-grid'>
                    <div class='project-card'>
                        <div class='card-label'>Dispositivos</div>
                        <div class='card-value'>{cotizacion.CantidadDispositivos}</div>
                    </div>
                    <div class='project-card'>
                        <div class='card-label'>Animales</div>
                        <div class='card-value'>{cotizacion.CantidadAnimales}</div>
                    </div>
                    <div class='project-card'>
                        <div class='card-label'>Tipo</div>
                        <div class='card-value' style='font-size: 16px;'>{cotizacion.TipoGanado}</div>
                    </div>
                    <div class='project-card'>
                        <div class='card-label'>Hectáreas</div>
                        <div class='card-value'>{cotizacion.Hectareas}</div>
                    </div>
                </div>
                
                <!-- Precio destacado -->
                <div class='project-card price-card' style='margin-top: 15px; grid-column: 1 / -1;'>
                    <div class='card-label'>Precio Estimado</div>
                    <div class='card-value' style='font-size: 24px;'>${precioCalculado:N2}</div>
                </div>
            </div>

            <!-- Funcionalidades -->
            {(!string.IsNullOrEmpty(string.Join(", ", cotizacion.FuncionalidadesRequeridas)) ? $@"
            <div class='features'>
                <h3>⚙️ Funcionalidades Requeridas</h3>
                <p>{string.Join(", ", cotizacion.FuncionalidadesRequeridas)}</p>
            </div>" : "")}

            <!-- Comentarios si existen -->
            {(!string.IsNullOrEmpty(cotizacion.Comentarios) ? $@"
            <div class='comments'>
                <h3>💬 Comentarios del Cliente</h3>
                <p>{cotizacion.Comentarios}</p>
            </div>" : "")}

            <!-- Call to Action -->
            <div class='action-box'>
                <h3 style='margin: 0 0 10px 0;'>⏰ Acción Requerida</h3>
                <p style='margin: 0; font-size: 16px;'>Contactar cliente en las próximas 4 horas laborables</p>
            </div>
        </div>
        
        <div class='footer'>
            <p style='margin: 0 0 10px 0; font-weight: 600;'>🖥️ Accede al sistema administrativo para gestionar esta cotización</p>
            <small style='opacity: 0.8;'>📅 {DateTime.Now:dd/MM/yyyy HH:mm} | Sistema Ganadería IoT</small>
        </div>
    </div>
</body>
</html>";

            var adminEmails = _configuration["Email:AdminEmails"].Split(',');
            var tasks = adminEmails.Select(email => SendEmailAsync(
                email.Trim(),
                "🚨 Nueva Cotización - Acción Requerida",
                template
            ));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Método original (mantener por compatibilidad)
        public async Task<bool> SendNotificationToAdmin(string subject, string message)
        {
            // Template simple para notificaciones generales
            var simpleTemplate = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background: #f5f5f5; }}
        .container {{ max-width: 500px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #2c5530; color: white; padding: 15px; border-radius: 8px; margin-bottom: 20px; text-align: center; }}
        .content {{ color: #333; line-height: 1.6; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; border-top: 1px solid #eee; padding-top: 15px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2 style='margin: 0;'>🔔 Notificación del Sistema</h2>
        </div>
        <div class='content'>
            <p>{message}</p>
        </div>
        <div class='footer'>
            <p>Ganadería IoT - {DateTime.Now:dd/MM/yyyy HH:mm}</p>
        </div>
    </div>
</body>
</html>";

            var adminEmails = _configuration["Email:AdminEmails"].Split(',');
            var tasks = adminEmails.Select(email => SendEmailAsync(email.Trim(), subject, simpleTemplate));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        public async Task<bool> SendCotizacionEmail(Cotizacion cotizacion)
        {
            var template = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #2E7D32; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; background: #f5f5f5; }
        .price-box { background: white; padding: 20px; margin: 20px 0; border-radius: 10px; text-align: center; }
        .price { font-size: 36px; color: #2E7D32; font-weight: bold; }
        .details { background: white; padding: 20px; border-radius: 10px; }
        .footer { text-align: center; padding: 20px; color: #666; }
        .btn { display: inline-block; padding: 15px 30px; background: #2E7D32; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Cotización Pecuadex GPS</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{NAME}</strong>,</p>
            <p>Gracias por su interés en Pecuadex GPS. Basándonos en sus requerimientos, hemos preparado la siguiente cotización personalizada:</p>
            
            <div class='price-box'>
                <p>Precio Total:</p>
                <div class='price'>${PRICE} MXN</div>
                <small>*Válido por 30 días</small>
            </div>
            
            <div class='details'>
                <h3>Detalles de su solicitud:</h3>
                <ul>
                    <li><strong>Dispositivos:</strong> {DEVICES}</li>
                    <li><strong>Animales:</strong> {ANIMALS}</li>
                    <li><strong>Tipo de ganado:</strong> {TYPE}</li>
                    <li><strong>Hectáreas:</strong> {HECTARES}</li>
                </ul>
                
                <h3>Incluye:</h3>
                <ul>
                    <li>✓ Dispositivos GPS de última generación</li>
                    <li>✓ Aplicación móvil iOS y Android</li>
                    <li>✓ Plataforma web completa</li>
                    <li>✓ Soporte técnico 24/7</li>
                    <li>✓ Capacitación inicial</li>
                    <li>✓ Garantía de 2 años</li>
                </ul>
            </div>
            
            <center>
                <a href='mailto:ventas@pecuadex.com?subject=Acepto%20la%20cotización%20{ID}' class='btn'>Aceptar Cotización</a>
            </center>
            
            <p>Si tiene alguna pregunta o desea ajustar algo, no dude en contactarnos.</p>
            
            <p>Saludos cordiales,<br>
            Equipo de Ventas Pecuadex</p>
        </div>
        <div class='footer'>
            <p>© 2024 Pecuadex. Todos los derechos reservados.</p>
            <p>Tel: +52 (477) 123-4567 | Email: ventas@pecuadex.com</p>
        </div>
    </div>
</body>
</html>";

            template = template.Replace("{NAME}", cotizacion.NombreCliente)
                              .Replace("{PRICE}", cotizacion.PrecioFinal?.ToString("N2") ?? "0")
                              .Replace("{DEVICES}", cotizacion.CantidadDispositivos.ToString())
                              .Replace("{ANIMALS}", cotizacion.CantidadAnimales.ToString())
                              .Replace("{TYPE}", cotizacion.TipoGanado)
                              .Replace("{HECTARES}", cotizacion.Hectareas.ToString())
                              .Replace("{ID}", cotizacion.Id.ToString());

            return await SendEmailAsync(
                cotizacion.EmailCliente,
                $"Cotización Pecuadex GPS #{cotizacion.Id}",
                template
            );
        }

        public async Task<bool> SendWelcomeEmail(string email, string fullName, string password)
        {
            var template = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #2E7D32; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; background: #f5f5f5; }
        .credentials { background: white; padding: 20px; margin: 20px 0; border-radius: 10px; border: 2px solid #2E7D32; }
        .btn { display: inline-block; padding: 15px 30px; background: #2E7D32; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido a Pecuadex!</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{NAME}</strong>,</p>
            <p>Su cuenta ha sido creada exitosamente. A continuación encontrará sus credenciales de acceso:</p>
            
            <div class='credentials'>
                <h3>Credenciales de Acceso:</h3>
                <p><strong>URL:</strong> https://app.pecuadex.com</p>
                <p><strong>Email:</strong> {EMAIL}</p>
                <p><strong>Contraseña:</strong> {PASSWORD}</p>
                <p style='color: red;'><small>Por seguridad, le recomendamos cambiar su contraseña en el primer inicio de sesión.</small></p>
            </div>
            
            <center>
                <a href='https://app.pecuadex.com/login' class='btn'>Iniciar Sesión</a>
            </center>
            
            <h3>Primeros pasos:</h3>
            <ol>
                <li>Inicie sesión con las credenciales proporcionadas</li>
                <li>Complete su perfil</li>
                <li>Configure sus dispositivos GPS</li>
                <li>Descargue la aplicación móvil</li>
            </ol>
            
            <p>Si necesita ayuda, nuestro equipo de soporte está disponible 24/7.</p>
            
            <p>Saludos cordiales,<br>
            Equipo Pecuadex</p>
        </div>
    </div>
</body>
</html>";

            template = template.Replace("{NAME}", fullName)
                              .Replace("{EMAIL}", email)
                              .Replace("{PASSWORD}", password);

            return await SendEmailAsync(email, "Bienvenido a Pecuadex - Credenciales de Acceso", template);
        }
    }
}