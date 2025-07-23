using AutoMapper;
using MediatR;
using MongoDB.Driver;
using Microservicio.Login.Api.Modelo;
using Microservicio.Login.Api.Persistencia;

namespace Microservicio.Login.Api.Aplicacion
{
    public class ConsultaPorUsuario
    {
        public class Ejecuta : IRequest<UsuarioDto>
        {
            public string Usuario { get; set; } // Parámetro de búsqueda
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioDto>
        {
            private readonly ContextoMongo _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoMongo contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }

            public async Task<UsuarioDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // 1. Búsqueda CASE-SENSITIVE exacta
                var usuario = await _contexto.UsuarioCollection
                    .Find(x => x.Usuario == request.Usuario) // Sin ToLower()
                    .FirstOrDefaultAsync();

                // 2. Validación estricta
                if (usuario == null)
                    throw new KeyNotFoundException("Usuario no encontrado o el nombre de usuario no esta escrito correctamente"); // Excepción específica

                return _mapper.Map<UsuarioDto>(usuario);
            }
        }
    }
}