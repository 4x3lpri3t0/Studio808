using System.ComponentModel.DataAnnotations;

namespace Api.Requests
{
    public class SaveGameStateRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int GamesPlayed { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public long Score { get; set; }

        public SaveGameStateRequest(int gamesPlayed, long score)
        {
            this.GamesPlayed = gamesPlayed;
            this.Score = score;
        }
    }
}