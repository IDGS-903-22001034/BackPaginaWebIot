namespace AuthAPI.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Contenido { get; set; } // HTML template
        public bool Activo { get; set; } = true;
    }
}