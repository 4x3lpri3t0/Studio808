using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BusinessLogic.Base.Dtos;

namespace BusinessLogic.Components.UserComponent.Dtos
{
    public class FriendsDto : BaseDto
    {
        [JsonPropertyName("friends")]
        public HashSet<Guid> Friends { get; set; }
    }
}