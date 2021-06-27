using System.Text.Json.Serialization;
using BusinessLogic.Base.Dtos;

namespace BusinessLogic.Components.UserComponent.Dtos
{
    public class GameStateDto : BaseDto
    {
        [JsonPropertyName("gamesPlayed")]
        public int GamesPlayed { get; set; }

        [JsonPropertyName("score")]
        public long Score { get; set; }
    }
}
