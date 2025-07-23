using MediatR;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microservicio.Login.Api.Modelo;
using Microservicio.Login.Api.Persistencia;

namespace Microservicio.Login.Api.Aplicacion
{
    public class Logout
    {
        public class CerrarSesion : IRequest<Unit>
        {
            public string UsuarioId { get; set; }
        }

        public class ManejadorCerrarSesion : IRequestHandler<CerrarSesion, Unit>
        {
            private readonly ContextoMongo _contexto;

            public ManejadorCerrarSesion(ContextoMongo contexto)
            {
                _contexto = contexto;
            }

            public async Task<Unit> Handle(CerrarSesion request, CancellationToken cancellationToken)
            {
                var filtro = Builders<Usuarioss>.Filter.Eq(u => u.Id, request.UsuarioId);
                var update = Builders<Usuarioss>.Update
                    .Set(u => u.RefreshToken, null)
                    .Set(u => u.RefreshTokenExpiration, DateTime.MinValue);

                var resultado = await _contexto.UsuarioCollection.UpdateOneAsync(filtro, update, cancellationToken: cancellationToken);

                if (resultado.MatchedCount == 0)
                    throw new Exception("Usuario no encontrado.");

                return Unit.Value;
            }
        }
    }
}
