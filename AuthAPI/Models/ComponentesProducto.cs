using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Models
{
    public class ComponentesProducto
    {

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int PiezaId { get; set; }
        public Pieza? Pieza { get; set; }

        public int CantidadRequerida { get; set; }
    }
}
