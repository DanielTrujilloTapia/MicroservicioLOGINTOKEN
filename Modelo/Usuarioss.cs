using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Microservicio.Login.Api.Modelo
{
    public class Usuarioss
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("usuario")]
        public string Usuario { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [BsonElement("usuarioGuid")]
        public string UsuarioGuid { get; set; }

        [BsonElement("preguntaRecuperacion")]
        public string PreguntaRecuperacion { get; set; }

        [BsonElement("respuestaRecuperacion")]
        public string RespuestaRecuperacion { get; set; }

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiration")]
        public DateTime RefreshTokenExpiration { get; set; }

        // --------------------------
        // MÉTODOS DE TOKEN
        // --------------------------

        public string GenerarJwt(string claveSecreta, string issuer, string audience, int minutosExpiracion = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(claveSecreta);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Id),
                new Claim(ClaimTypes.Name, Usuario)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutosExpiracion),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerarRefreshToken(int tamañoBytes = 32)
        {
            var randomBytes = new byte[tamañoBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public void AsignarNuevoRefreshToken()
        {
            RefreshToken = GenerarRefreshToken();
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(1); // Por ejemplo, 1 día de duración
        }
    }
}
