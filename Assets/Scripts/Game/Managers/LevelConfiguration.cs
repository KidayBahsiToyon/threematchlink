using Core.Utils;
using UnityEngine;

namespace Game.Managers
{
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "MatchLink/Level Configuration")]
    public class LevelConfiguration : SingletonScriptableObject<LevelConfiguration>
    {
        [Header("Board Settings")]
        public int Width = 8;
        public int Height = 8;
        public int CellSize = 1;
        
        [Header("Game Rules")]
        public int MoveLimit = 10;
        public int TargetGemCount = 15;
        public int MinMatchCount = 3;
        
        [Header("Scoring")]
        public int ScorePerGem = 10;
        
        [System.NonSerialized] 
        public int TargetGemTypeIndex = -1;
    }
}
