namespace AuthAPI.Dtos
{
    public class ComentarioDto
    {
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public string Descripcion { get; set; }
        public int Calificacion { get; set; }
    }

    public class ComentarioResponseDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCliente { get; set; }
    }
}