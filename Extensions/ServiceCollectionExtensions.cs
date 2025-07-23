using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microservicio.Login.Api.Aplicacion;

using Microservicio.Login.Api.Persistencia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservicio.Login.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services,
           IConfiguration configuration)
        {
            // Configuración de JWT
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Validaciones con FluentValidation
            services.AddControllers()
                .AddFluentValidation(cfg =>
                    cfg.RegisterValidatorsFromAssemblyContaining<NuevoUsuario>());

            // MongoDB Context
            services.AddSingleton(sp =>
            {
                var connectionString = configuration["MongoDatabase:ConnectionString"];
                var databaseName = configuration["MongoDatabase:DatabaseName"];
                return new ContextoMongo(connectionString, databaseName);
            });

            // MediatR v12+
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(NuevoUsuario.Manejador).Assembly));

            // AutoMapper
            services.AddAutoMapper(cfg =>
                cfg.AddMaps(typeof(ConsultaUsuario).Assembly));

            return services;
        }
    }
}
