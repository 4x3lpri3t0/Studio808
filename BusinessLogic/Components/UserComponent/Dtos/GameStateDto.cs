using System.Text.Json.Serialization;
using BusinessLogic.Base.Dtos;

namespace BusinessLogic.Components.UserComponent.Dtos
{
    public class GameStateDto : BaseDto
    {
        public int GamesPlayed { get; set; }

        public long Score { get; set; }
    }
}
