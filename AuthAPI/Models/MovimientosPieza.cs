using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class MovimientosPieza
    {
        public int Id { get; set; }

        public int PiezaId { get; set; }
        [JsonIgnore]
        public Pieza? Pieza { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;


        [StringLength(20)]
        public string TipoMovimiento { get; set; } // Entrada o Salida

        public int Cantidad { get; set; }

        public decimal? CostoUnitario { get; set; }

        public decimal CostoPromedio { get; set; }

        public decimal ValorDebe { get; set; }
        public decimal ValorHaber { get; set; }
        public decimal SaldoValor { get; set; }
        public float Existencias { get; set; }
        [JsonIgnore]
        public DetalleCompra? DetalleCompra { get; set; }
    }
}
