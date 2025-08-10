using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Completada";

        public ICollection<DetalleVenta> Detalles { get; set; }

        // Agregar navegación a comentarios
        [JsonIgnore]
        public ICollection<Comentario>? Comentarios { get; set; }

        public string? UsuarioId { get; set; }
        [JsonIgnore]
        public AppUser? Usuario { get; set; }
    }
}