namespace QuizAPI.StatisticManager
{
    public interface IStatisticManager
    {
        public Task<IResult> GetLastPlayedGame();
        public Task<IResult> GetAllPlayedGames();
    }
}
