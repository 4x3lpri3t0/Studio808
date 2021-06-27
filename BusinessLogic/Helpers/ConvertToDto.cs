using AutoMapper;
using BusinessLogic.Base.Dtos;
using Data.Access.Entities;

namespace BusinessLogic.Helpers
{
    public static class ConvertToDto
    {
        public static IMapper Mapper = null;

        public static TDto ToDto<TDto>(this BaseEntity obj) where TDto : BaseDto
        {
            if (Mapper == null)
            {
                throw new System.Exception("Error when trying to init AutoMapper Helper");
            }
            return Mapper.Map<BaseEntity, TDto>(obj);
        }
    }
}
