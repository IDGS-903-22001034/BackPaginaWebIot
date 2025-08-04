namespace AuthAPI.Dtos
{
    public class MovimientosPiezaDto
    {
        public int Id { get; set; }
        public int PiezaId { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoPromedio { get; set; }
        public decimal ValorDebe { get; set; }
        public decimal ValorHaber { get; set; }
        public decimal SaldoValor { get; set; }
        public float Existencias { get; set; }
        public PiezaDto Pieza { get; set; }
    }
}
