namespace Microservicio.Login.Api.Aplicacion
{
    public class UsuarioDto
    {
        public string Id { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioGuid { get; set; }

        // Nuevos campos
        public string PreguntaRecuperacion { get; set; }
        public string RespuestaRecuperacion { get; set; }
    }
}
