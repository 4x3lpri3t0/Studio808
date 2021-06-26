using AutoMapper;
using Studio808.BusinessLogic.Base.Dtos;
using Studio808.DataAccess.Entities;

namespace Studio808.BusinessLogic.Helpers
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
