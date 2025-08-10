using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Compra
    {
        public int? Id { get; set; }

        public int? ProveedorId { get; set; }
        [JsonIgnore]
        public Proveedor? Proveedor { get; set; }

        public DateTime? Fecha { get; set; } = DateTime.Now;

        public ICollection<DetalleCompra> Detalles { get; set; }
    }
}
