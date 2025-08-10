namespace AuthAPI.Dtos
{
    public class CompraClienteDto
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<ProductoDto> Productos { get; set; }
    }
}
