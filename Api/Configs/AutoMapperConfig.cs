using AutoMapper;
using BusinessLogic.Components.UserComponent.Dtos;
using Data.Access.Entities;

namespace Api.Configs
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDto>();
            CreateMap<GameState, GameStateDto>();
        }
    }
}