using AuthAPI.Dtos;

namespace AuthAPI.Services
{
    public interface ICotizacionService
    {
        decimal CalcularPrecio(CotizacionDto cotizacion);
        string GenerarPasswordAleatorio();
    }

    public class CotizacionService : ICotizacionService
    {
        public decimal CalcularPrecio(CotizacionDto cotizacion)
        {
            decimal precioBase = 2500; // Precio por dispositivo
            decimal total = precioBase * cotizacion.CantidadDispositivos;

            // Descuentos por volumen
            if (cotizacion.CantidadDispositivos > 50)
            {
                total *= 0.80m; // 20% descuento
            }
            else if (cotizacion.CantidadDispositivos > 20)
            {
                total *= 0.85m; // 15% descuento
            }
            else if (cotizacion.CantidadDispositivos > 10)
            {
                total *= 0.90m; // 10% descuento
            }

            // Costos adicionales por funcionalidades
            if (cotizacion.FuncionalidadesRequeridas?.Contains("alertas-avanzadas") == true)
            {
                total += 500 * cotizacion.CantidadDispositivos;
            }

            if (cotizacion.FuncionalidadesRequeridas?.Contains("analytics") == true)
            {
                total += 800 * cotizacion.CantidadDispositivos;
            }

            if (cotizacion.FuncionalidadesRequeridas?.Contains("integracion-erp") == true)
            {
                total += 1500;
            }

            if (cotizacion.FuncionalidadesRequeridas?.Contains("multi-usuario") == true)
            {
                total += 1000;
            }

            return Math.Round(total, 2);
        }

        public string GenerarPasswordAleatorio()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}