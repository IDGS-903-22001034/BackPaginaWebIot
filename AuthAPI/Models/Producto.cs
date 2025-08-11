using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public decimal? PrecioSugerido { get; set; }

        [StringLength(250)]
        public string? Imagen { get; set; }

        public DateTime? FechaRegistro { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<ComponentesProducto>? ComponentesProducto { get; set; }
        [JsonIgnore]
        public ICollection<Comentario>? Comentarios { get; set; }
        [JsonIgnore]
        public ICollection<Manual>? Manuales { get; set; }
        [JsonIgnore]
        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
    }
}
