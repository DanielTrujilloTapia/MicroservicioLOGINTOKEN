using FluentValidation;
using MediatR;
using Microservicio.Login.Api.Modelo;
using Microservicio.Login.Api.Persistencia;
using MongoDB.Driver;

namespace Microservicio.Login.Api.Aplicacion
{
    public class ActualizarUsuario
    {
        // 📌 DTO de Request que implementa IRequest
        public class EjecutaActualizar : IRequest<Unit>
        {
            public string UsuarioGuid { get; set; }
            public string Usuario { get; set; }
            public string Password { get; set; }
            public string PreguntaRecuperacion { get; set; }
            public string RespuestaRecuperacion { get; set; }
        }

        // 📌 Validación de FluentValidation
        public class EjecutaValidacion : AbstractValidator<EjecutaActualizar>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.UsuarioGuid).NotEmpty().WithMessage("El GUID del login es obligatorio");
                RuleFor(x => x.Usuario).NotEmpty().WithMessage("El nombre de usuario es obligatorio");
                RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es obligatoria");
                RuleFor(x => x.PreguntaRecuperacion).NotEmpty().WithMessage("La pregunta de recuperación es obligatoria");
                RuleFor(x => x.RespuestaRecuperacion).NotEmpty().WithMessage("La respuesta de recuperación es obligatoria");
            }
        }

        // 📌 Manejador de MediatR
        public class Manejador : IRequestHandler<EjecutaActualizar, Unit>
        {
            private readonly ContextoMongo _context;

            public Manejador(ContextoMongo context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(EjecutaActualizar request, CancellationToken cancellationToken)
            {
                var usuarioExistente = await _context.UsuarioCollection
                    .Find(x => x.UsuarioGuid == request.UsuarioGuid)
                    .FirstOrDefaultAsync();

                if (usuarioExistente == null)
                    throw new KeyNotFoundException("No se encontró el usuario especificado");

                var usuarioActualizado = new Modelo.Usuarioss
                {
                    Id = usuarioExistente.Id, // Mantener el mismo ID
                    UsuarioGuid = request.UsuarioGuid, // Mantener el mismo GUID
                    Usuario = request.Usuario,
                    Password = request.Password,
                    PreguntaRecuperacion = request.PreguntaRecuperacion,
                    RespuestaRecuperacion = request.RespuestaRecuperacion,
                    FechaRegistro = usuarioExistente.FechaRegistro // Mantener la fecha original
                };

                await _context.UsuarioCollection.ReplaceOneAsync(
                    x => x.UsuarioGuid == request.UsuarioGuid,
                    usuarioActualizado,
                    new ReplaceOptions { IsUpsert = false },
                    cancellationToken);

                return Unit.Value;
            }
        }
    }
}