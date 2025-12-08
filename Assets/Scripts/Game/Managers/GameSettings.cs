using static Game.Constants.GameConstants;

namespace Game.Managers
{
    public static class GameSettings
    {
        public static bool IsConfigured;
        
        public static int BoardWidth = DEFAULT_WIDTH;
        public static int BoardHeight = DEFAULT_HEIGHT;
        public static int MoveLimit = DEFAULT_MOVES;
        public static int TargetGemCount = DEFAULT_TARGET;
        public static int MinMatchCount = DEFAULT_MIN_MATCH;
        public static int TargetGemTypeIndex = -1;

        public static int GetWidth() => IsConfigured ? BoardWidth : LevelConfiguration.Instance.Width;
        public static int GetHeight() => IsConfigured ? BoardHeight : LevelConfiguration.Instance.Height;
        public static int GetMoveLimit() => IsConfigured ? MoveLimit : LevelConfiguration.Instance.MoveLimit;
        public static int GetTargetGemCount() => IsConfigured ? TargetGemCount : LevelConfiguration.Instance.TargetGemCount;
        public static int GetMinMatchCount() => IsConfigured ? MinMatchCount : LevelConfiguration.Instance.MinMatchCount;
        public static int GetScorePerGem() => LevelConfiguration.Instance.ScorePerGem;
        public static int GetCellSize() => LevelConfiguration.Instance.CellSize;
    }
}
