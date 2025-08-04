namespace AuthAPI.Dtos
{
    public class PiezaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal CostoPromedio { get; set; }
        public float Existencias { get; set; }
    }
}
