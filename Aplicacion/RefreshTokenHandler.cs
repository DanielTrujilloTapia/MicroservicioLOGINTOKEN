using AutoMapper;
using MediatR;
using Microservicio.Login.Api.Persistencia;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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
            private readonly TokenService _tokenService;
            private readonly IMediator _mediator;


            public ManejadorRenovarToken(ContextoMongo contexto, IMapper mapper, IOptions<JwtSettings> jwtSettings, TokenService tokenService, IMediator mediator)
            {
                _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
                _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task<LoginResponseDto> Handle(RenovarTokenRequest request, CancellationToken cancellationToken)
            {
                // Buscar usuario con ese refresh token
                var usuario = await _contexto.UsuarioCollection.Find(u => u.RefreshToken == request.RefreshToken).FirstOrDefaultAsync(cancellationToken);

                // Si no existe el usuario o ya expiró el token → ejecutar logout
                if (usuario == null || usuario.RefreshTokenExpiration <= DateTime.UtcNow)
                {
                    if (usuario != null){await _mediator.Send(new Logout.CerrarSesion { UsuarioId = usuario.Id }, cancellationToken);}
                    throw new UnauthorizedAccessException("El refresh token ha expirado o no es válido. Inicie sesión nuevamente.");
                }

                // ⚠️ No se genera un nuevo refresh token, se mantiene el actual

                // Solo se genera un nuevo JWT
                string nuevoToken = _tokenService.CrearTokenJwtParaAutizacion(usuarioId: usuario.Id,nombreUsuario: usuario.Usuario,claveSecreta: _jwtSettings.SecretKey,issuer: _jwtSettings.Issuer,audience: _jwtSettings.Audience,minutosExpiracion: 10);
                var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                return new LoginResponseDto{Usuario = usuarioDto,Token = nuevoToken,RefreshToken = usuario.RefreshToken};
            }

        }
    }
}