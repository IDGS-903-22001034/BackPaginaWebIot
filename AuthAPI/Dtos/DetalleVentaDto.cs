namespace AuthAPI.Dtos
{
    public class DetalleVentaDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public ProductoDto Producto { get; set; }
    }
}
