using System.Collections.Generic;
using Game.Board;

namespace Game.Services
{
    public interface ILinkService
    {
        List<Gem> LinkedGems { get; }
        bool IsDragging { get; }
        int MinimumMatchCount { get; }

        int GetCurrentLinkCount();
        bool IsCurrentLinkValid();
        void CancelLink();
    }
}
