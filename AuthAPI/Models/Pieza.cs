using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Pieza
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string UnidadMedida { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<MovimientosPieza>? MovimientosPieza { get; set; }
        [JsonIgnore]
        public ICollection<ComponentesProducto>? Componentes { get; set; }
    }
}
