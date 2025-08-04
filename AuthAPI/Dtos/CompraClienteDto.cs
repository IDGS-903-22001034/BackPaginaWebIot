namespace AuthAPI.Dtos
{
    public class CompraClienteDto
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<ProductoDto> Productos { get; set; }
    }
}
