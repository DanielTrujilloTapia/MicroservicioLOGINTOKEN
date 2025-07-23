using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microservicio.Login.Api.Modelo;
using Microservicio.Login.Api.Persistencia;

namespace Microservicio.Login.Api.Aplicacion
{
    public class LoginUsuario
    {
        public class EjecutaLogin : IRequest<LoginResponseDto>
        {
            public string Usuario { get; set; }
            public string Password { get; set; }
        }

        public class Manejador : IRequestHandler<EjecutaLogin, LoginResponseDto>
        {
            private readonly ContextoMongo _contexto;
            private readonly IMapper _mapper;
            private readonly JwtSettings _jwtSettings;

            public Manejador(ContextoMongo contexto, IMapper mapper, IOptions<JwtSettings> jwtOptions)
            {
                _contexto = contexto;
                _mapper = mapper;
                _jwtSettings = jwtOptions.Value;
            }

            public async Task<LoginResponseDto> Handle(EjecutaLogin request, CancellationToken cancellationToken)
            {
                var usuario = await _contexto.UsuarioCollection
                    .Find(x => x.Usuario == request.Usuario)
                    .FirstOrDefaultAsync();

                if (usuario == null)
                    throw new KeyNotFoundException("El usuario no existe");

                if (usuario.Password != request.Password)
                    throw new AuthenticationException("La contraseña es incorrecta");

                // Generar nuevo refresh token
                usuario.AsignarNuevoRefreshToken();

                // Guardar el refresh token en MongoDB
                var filtro = Builders<Usuarioss>.Filter.Eq(u => u.Id, usuario.Id);
                var update = Builders<Usuarioss>.Update
                    .Set(u => u.RefreshToken, usuario.RefreshToken)
                    .Set(u => u.RefreshTokenExpiration, usuario.RefreshTokenExpiration);

                await _contexto.UsuarioCollection.UpdateOneAsync(filtro, update, cancellationToken: cancellationToken);

                // Generar JWT
                var token = usuario.GenerarJwt(
                    claveSecreta: _jwtSettings.SecretKey,
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    minutosExpiracion: 60 // Puedes ajustar este valor
                );

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                return new LoginResponseDto
                {
                    Usuario = usuarioDto,
                    Token = token,
                    RefreshToken = usuario.RefreshToken
                };
            }
        }
    }
}
