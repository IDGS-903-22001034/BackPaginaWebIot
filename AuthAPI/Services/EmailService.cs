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

        public async Task<bool> SendNotificationToAdmin(string subject, string message)
        {
            var adminEmails = _configuration["Email:AdminEmails"].Split(',');
            var tasks = adminEmails.Select(email => SendEmailAsync(email.Trim(), subject, message));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
    }
}