using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class DetalleCompra
    {

        public int? Id { get; set; }

        public int? CompraId { get; set; }
        [JsonIgnore]
        public Compra? Compra { get; set; }

        public int? MovimientosPiezaId { get; set; }
        [JsonIgnore]
        public MovimientosPieza? MovimientosPieza { get; set; }

        [StringLength(50)]
        public string? Presentacion { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
        [NotMapped]
        public int PiezaId { get; set; }
    }
}
