namespace AuthAPI.Dtos
{
    public class VentaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public IEnumerable<DetalleVentaDto> Detalles { get; set; }
    }
}
