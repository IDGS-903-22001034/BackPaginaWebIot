namespace AuthAPI.Dtos
{
    public class CotizacionDto
    {
        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; }
        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public int CantidadDispositivos { get; set; }
        public int CantidadAnimales { get; set; }
        public string TipoGanado { get; set; }
        public int Hectareas { get; set; }
        public List<string> FuncionalidadesRequeridas { get; set; }
        public string Comentarios { get; set; }
    }

    public class CotizacionResponseDto
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; }
        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public int CantidadDispositivos { get; set; }
        public int CantidadAnimales { get; set; }
        public string TipoGanado { get; set; }
        public int Hectareas { get; set; }
        public List<string> FuncionalidadesRequeridas { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
        public decimal? PrecioCalculado { get; set; }
        public decimal? PrecioFinal { get; set; }
        public string NotasInternas { get; set; }
        public string UsuarioAsignado { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public bool ClienteCreado { get; set; }
    }

    public class EnviarCotizacionDto
    {
        public int CotizacionId { get; set; }
        public decimal PrecioFinal { get; set; }
        public string NotasAdicionales { get; set; }
        public int ValidezDias { get; set; } = 30;
    }

    public class CrearClienteDesdeeCotizacionDto
    {
        public int CotizacionId { get; set; }
        public string Password { get; set; } // Si es null, se genera automáticamente
    }
}