using System.Collections.Generic;
using System.Text.Json.Serialization;
using BusinessLogic.Base.Dtos;

namespace BusinessLogic.Components.UserComponent.Dtos
{
    public class FriendScoresDto : BaseDto
    {
        [JsonPropertyName("friends")]
        public List<FriendScoreDto> FriendScores { get; set; }
    }
}
