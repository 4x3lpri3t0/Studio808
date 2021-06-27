namespace Data.Access.Entities
{
    public class GameState : BaseEntity
    {
        public int GamesPlayed { get; set; }
        public long Score { get; set; }

        public GameState(int gamesPlayed, long score)
        {
            GamesPlayed = gamesPlayed;
            Score = score;
        }
    }
}
