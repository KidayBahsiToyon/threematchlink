namespace Game.Services
{
    public interface IGameService
    {
        int CurrentScore { get; }
        int RemainingMoves { get; }
        int TargetScore { get; }
        bool IsGameOver { get; }

        void InitializeGame();
        void RestartLevel();
        void AddMoves(int amount);
        void AddScore(int amount);
    }
}
