namespace QuizAPI.StatisticManager
{
    public interface IStatisticManager
    {
        public Task<IResult> GetLastPlayedGame();
        public Task<IResult> GetAllPlayedGames();
        public Task<IResult> GetGameSessionStats(string gamesessionId);
    }
}
