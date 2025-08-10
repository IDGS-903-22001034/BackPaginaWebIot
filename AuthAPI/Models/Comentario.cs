using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        // Cambiar de ProductoId a VentaId
        public int VentaId { get; set; }
        [JsonIgnore]
        public Venta? Venta { get; set; }

        // Agregar ProductoId para saber sobre qué producto específico es el comentario
        public int ProductoId { get; set; }
        [JsonIgnore]
        public Producto? Producto { get; set; }

        public string Descripcion { get; set; }

        [Range(1, 5)]
        public int Calificacion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}