using System.Collections.Generic;
using Game.Board;
using UnityEngine;

namespace Game.Services
{
    public interface IBoardService
    {
        int Width { get; }
        int Height { get; }
        bool IsProcessing { get; }

        void InitializeBoard();
        void DestroyGems(List<Gem> gems);
        Gem GetGemAt(int x, int y);
        bool IsValidPosition(int x, int y);
        Vector2Int WorldToGridPosition(Vector3 worldPos);
        Vector3 GetWorldPosCenter(int x, int y);
        List<Gem> GetGemsOfType(GemType type);
        bool AreGemsMoving();
    }
}
