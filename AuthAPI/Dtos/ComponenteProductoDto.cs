namespace AuthAPI.Dtos
{
    public class ComponenteProductoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int PiezaId { get; set; }
        public int CantidadRequerida { get; set; }
        public PiezaDto Pieza { get; set; }
    }
}
