using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        public int VentaId { get; set; }
        [JsonIgnore]
        public Venta? Venta { get; set; }
        public int ProductoId { get; set; }
        [JsonIgnore]
        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
