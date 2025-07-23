using AutoMapper;
using MediatR;
using MongoDB.Driver;
using Microservicio.Login.Api.Modelo;
using Microservicio.Login.Api.Persistencia;

namespace Microservicio.Login.Api.Aplicacion
{
    public class ConsultaUsuario
    {
        public class ListaUsuario : IRequest<List<UsuarioDto>> { }

        public class Manejador : IRequestHandler<ListaUsuario, List<UsuarioDto>>
        {
            private readonly ContextoMongo _contexto;
            private readonly IMapper _mapper;

            public Manejador(ContextoMongo contexto, IMapper mapper)
            {
                _contexto = contexto;
                _mapper = mapper;
            }

            public async Task<List<UsuarioDto>> Handle(ListaUsuario request, CancellationToken cancellationToken)
            {
                var usuarios = await _contexto.UsuarioCollection.Find(_ => true).ToListAsync();
                return _mapper.Map<List<UsuarioDto>>(usuarios);
            }
        }
    }
}
