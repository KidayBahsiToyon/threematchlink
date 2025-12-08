using UnityEngine;

namespace Game.Board
{
    [CreateAssetMenu(fileName = "GemDatabase", menuName = "MatchLink/Gem Database")]
    public class GemDatabase : ScriptableObject
    {
        [SerializeField] 
        private GemType[] _types;

        public GemType[] GemTypes => _types;

        public GemType GetRandomGemType()
        {
            if (_types == null || _types.Length == 0)
            {
                Debug.LogError("No gem types configured!");
                return null;
            }
            return _types[Random.Range(0, _types.Length)];
        }

        public int GetTypeIndex(GemType type)
        {
            for (int i = 0; i < _types.Length; i++)
            {
                if (_types[i] == type)
                    return i;
            }
            return -1;
        }
    }
}
