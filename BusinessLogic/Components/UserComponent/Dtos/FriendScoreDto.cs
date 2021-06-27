using System;
using System.Text.Json.Serialization;
using BusinessLogic.Base.Dtos;

namespace BusinessLogic.Components.UserComponent.Dtos
{
    public class FriendScoreDto : BaseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("highscore")]
        public long Score { get; set; }
    }
}
