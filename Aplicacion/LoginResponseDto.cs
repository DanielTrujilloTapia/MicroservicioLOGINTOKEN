namespace Microservicio.Login.Api.Aplicacion
{
    public class LoginResponseDto
    {
        public UsuarioDto Usuario { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; } // ✅ Agregado
    }
}
