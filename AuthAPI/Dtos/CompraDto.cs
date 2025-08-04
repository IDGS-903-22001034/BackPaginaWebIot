namespace AuthAPI.Dtos
{
    public class CompraDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; }
        public ProveedorDto Proveedor { get; set; }
        public List<DetalleCompraDto> Detalles { get; set; }
    }
}
