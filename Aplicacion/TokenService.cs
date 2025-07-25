﻿// En la carpeta Aplicacion o Services
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Microservicio.Login.Api.Aplicacion
{
    public class TokenService
    {
        public string CrearTokenJwtParaAutizacion(string usuarioId, string nombreUsuario, string claveSecreta, string issuer, string audience, int minutosExpiracion = 10)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(claveSecreta);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Name, nombreUsuario)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),Expires = DateTime.UtcNow.AddMinutes(minutosExpiracion),Issuer = issuer,Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string CrearNuevoRefreshtokenParaElUsuario(int tamañoBytes = 32)
        {
            var randomBytes = new byte[tamañoBytes];
            using (var rng = RandomNumberGenerator.Create()){rng.GetBytes(randomBytes);}
            return Convert.ToBase64String(randomBytes);
        }

        
    }
}