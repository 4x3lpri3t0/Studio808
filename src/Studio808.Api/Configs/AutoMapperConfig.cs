using AutoMapper;
using Studio808.BusinessLogic.Components.UserComponent.Dtos;
using Studio808.BusinessLogic.Components.UserComponent.Entities;

namespace Studio808.Api.Configs
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDto>();
        }
    }
}