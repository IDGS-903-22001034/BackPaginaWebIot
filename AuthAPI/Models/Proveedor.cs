using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Proveedor
    {
        public int? Id { get; set; }

        [StringLength(100)]
        public string? NombreEmpresa { get; set; }

        [StringLength(100)]
        public string? Contacto { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [JsonIgnore]
        public ICollection<Compra>? Compras { get; set; }
    }
}
