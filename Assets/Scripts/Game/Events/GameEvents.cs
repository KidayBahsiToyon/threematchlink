using System.Collections.Generic;
using Core.Events;
using Game.Board;

namespace Game.Events
{
    public struct MatchCompletedEvent : IEvent
    {
        public List<Gem> MatchedGems;
        public int GemCount;
        public GemType GemType;
        public bool IsTargetType;

        public MatchCompletedEvent(List<Gem> gems, bool isTarget)
        {
            MatchedGems = gems;
            GemCount = gems.Count;
            GemType = gems.Count > 0 ? gems[0].Type : null;
            IsTargetType = isTarget;
        }
    }

    public struct LinkUpdatedEvent : IEvent
    {
        public List<Gem> LinkedGems;
        public int Count;
        public bool IsValid;
        public bool IsTargetType;

        public LinkUpdatedEvent(List<Gem> gems, int minMatch, bool isTarget)
        {
            LinkedGems = gems;
            Count = gems.Count;
            IsValid = gems.Count >= minMatch;
            IsTargetType = isTarget;
        }
    }

    public struct LinkCancelledEvent : IEvent { }

    public struct ScoreChangedEvent : IEvent
    {
        public int CurrentScore;
        public int TargetScore;
        public float Progress;

        public ScoreChangedEvent(int current, int target)
        {
            CurrentScore = current;
            TargetScore = target;
            Progress = target > 0 ? (float)current / target : 0f;
        }
    }

    public struct TargetProgressEvent : IEvent
    {
        public int CurrentCount;
        public int TargetCount;
        public GemType TargetType;
        public float Progress;

        public TargetProgressEvent(int current, int target, GemType type)
        {
            CurrentCount = current;
            TargetCount = target;
            TargetType = type;
            Progress = target > 0 ? (float)current / target : 0f;
        }
    }

    public struct MatchScoredEvent : IEvent
    {
        public int GemCount;
        public int ScoreEarned;
        public bool WasTargetGem;

        public MatchScoredEvent(int gems, int score, bool wasTarget)
        {
            GemCount = gems;
            ScoreEarned = score;
            WasTargetGem = wasTarget;
        }
    }

    public struct MovesChangedEvent : IEvent
    {
        public int RemainingMoves;
        public int TotalMoves;

        public MovesChangedEvent(int remaining, int total)
        {
            RemainingMoves = remaining;
            TotalMoves = total;
        }
    }

    public struct GameOverEvent : IEvent
    {
        public bool IsWin;
        public int FinalScore;
        public int GemsCollected;
        public int TargetGems;

        public GameOverEvent(bool win, int score, int collected, int target)
        {
            IsWin = win;
            FinalScore = score;
            GemsCollected = collected;
            TargetGems = target;
        }
    }

    public struct GameInitializedEvent : IEvent
    {
        public int TargetGemCount;
        public int MoveLimit;
        public int BoardWidth;
        public int BoardHeight;
        public GemType TargetGemType;

        public GameInitializedEvent(int targetCount, int moves, int w, int h, GemType targetType)
        {
            TargetGemCount = targetCount;
            MoveLimit = moves;
            BoardWidth = w;
            BoardHeight = h;
            TargetGemType = targetType;
        }
    }

    public struct RestartGameEvent : IEvent { }

    public struct BoardProcessingEvent : IEvent
    {
        public bool IsProcessing;
        public BoardProcessingEvent(bool processing) => IsProcessing = processing;
    }

    public struct GemsDestroyedEvent : IEvent
    {
        public int Count;
        public GemType Type;

        public GemsDestroyedEvent(int count, GemType type)
        {
            Count = count;
            Type = type;
        }
    }

    public struct BoardRefillCompleteEvent : IEvent { }
    
    public struct GoToMenuEvent : IEvent { }
    
}
