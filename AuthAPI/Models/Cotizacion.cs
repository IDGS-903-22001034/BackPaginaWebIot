using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? NombreCliente { get; set; }

        [Required]
        [EmailAddress]
        public string? EmailCliente { get; set; }

        [Required]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Empresa { get; set; }

        public int CantidadDispositivos { get; set; }
        public int CantidadAnimales { get; set; }

        [StringLength(50)]
        public string? TipoGanado { get; set; }

        public int Hectareas { get; set; }

        public string? FuncionalidadesRequeridas { get; set; } // JSON string

        [StringLength(500)]
        public string? Comentarios { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Enviada, Aceptada, Rechazada, Convertida

        public decimal? PrecioCalculado { get; set; }
        public decimal? PrecioFinal { get; set; }

        [StringLength(1000)]
        public string? NotasInternas { get; set; }

        public string? UsuarioAsignadoId { get; set; }
        public AppUser? UsuarioAsignado { get; set; }

        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        public bool ClienteCreado { get; set; } = false;
        public string? ClienteId { get; set; }
        public AppUser? Cliente { get; set; }

        public int? VentaId { get; set; }
        public Venta? Venta { get; set; }
    }
}