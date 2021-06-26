using System;
using System.Text.Json.Serialization;
using Studio808.BusinessLogic.Base.Dtos;

namespace Studio808.BusinessLogic.Components.UserComponent.Dtos
{
    public class UserDto : BaseDto
    {
        [JsonPropertyName("userid")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
