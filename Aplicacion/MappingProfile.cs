using AutoMapper;


namespace Microservicio.Login.Api.Aplicacion
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Modelo.Usuarioss, UsuarioDto>();
        }
    }
}
