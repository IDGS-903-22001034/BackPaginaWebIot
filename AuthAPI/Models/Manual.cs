using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Manual
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        [JsonIgnore]
        public Producto? Producto { get; set; }

        [StringLength(100)]
        public string Titulo { get; set; }

        [StringLength(250)]
        public string UrlDocumento { get; set; }
    }
}
