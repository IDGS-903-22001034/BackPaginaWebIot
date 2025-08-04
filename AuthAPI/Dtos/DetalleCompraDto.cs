namespace AuthAPI.Dtos
{
    public class DetalleCompraDto
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public int MovimientosPiezaId { get; set; }
        public string Presentacion { get; set; }
        public float Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
        public MovimientosPiezaDto MovimientoPieza { get; set; }
    }
}
