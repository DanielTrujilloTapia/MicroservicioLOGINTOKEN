using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microservicio.Login.Api.Modelo;

using Microservicio.Login.Api.Persistencia;


namespace Microservicio.Login.Api.Aplicacion
{
    public class RefreshTokenHandler
    {
        public class RenovarTokenRequest : IRequest<LoginResponseDto>
        {
            public string RefreshToken { get; set; }
        }

        public class ManejadorRenovarToken : IRequestHandler<RenovarTokenRequest, LoginResponseDto>
        {
            private readonly ContextoMongo _contexto;
            private readonly IMapper _mapper;
            private readonly JwtSettings _jwtSettings;

            public ManejadorRenovarToken(ContextoMongo contexto, IMapper mapper, IOptions<JwtSettings> jwtSettings)
            {
                _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            }

            public async Task<LoginResponseDto> Handle(RenovarTokenRequest request, CancellationToken cancellationToken)
            {
                // Buscar usuario con refresh token válido
                var usuario = await _contexto.UsuarioCollection
                    .Find(u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiration > DateTime.UtcNow)
                    .FirstOrDefaultAsync(cancellationToken);

                if (usuario == null)
                    throw new UnauthorizedAccessException("Refresh token inválido o expirado.");

                // Generar y asignar nuevo refresh token
                usuario.AsignarNuevoRefreshToken();

                // Actualizar en Mongo
                var filtro = Builders<Usuarioss>.Filter.Eq(u => u.Id, usuario.Id);
                var update = Builders<Usuarioss>.Update
                    .Set(u => u.RefreshToken, usuario.RefreshToken)
                    .Set(u => u.RefreshTokenExpiration, usuario.RefreshTokenExpiration);

                await _contexto.UsuarioCollection.UpdateOneAsync(filtro, update, cancellationToken: cancellationToken);

                // Generar nuevo JWT
                string nuevoToken = usuario.GenerarJwt(
                    claveSecreta: _jwtSettings.SecretKey,
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    minutosExpiracion: 60);

                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                return new LoginResponseDto
                {
                    Usuario = usuarioDto,
                    Token = nuevoToken,
                    RefreshToken = usuario.RefreshToken
                };
            }
        }
    }
}
