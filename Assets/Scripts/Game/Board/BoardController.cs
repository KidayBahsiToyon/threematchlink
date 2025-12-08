using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Game.CameraHelper;
using Game.Events;
using Game.Managers;
using Game.Services;
using UnityEngine;
using static Game.Constants.GameConstants;

namespace Game.Board
{
    public class BoardController : MonoBehaviour, IBoardService
    {
        [SerializeField] 
        private GemDatabase _gemDatabase;
        [SerializeField] 
        private CameraScaler _cameraScaler;
        [SerializeField] 
        private GameObject _tilePrefab;
        [SerializeField] 
        private Gem _gemPrefab;
        
        [SerializeField] 
        private float _fallSpeed = ANIM_DURATION;
        [SerializeField] 
        private float _fallDelay = 0.05f;
        [SerializeField] 
        private float _spawnDelay = 0.1f;

        private Gem[,] _grid;
        private Vector3 _origin;
        private bool _processing;
        private int _w, _h, _cell;
        
        EventBinding<RestartGameEvent> _restartBinding;

        public int Width => _w;
        public int Height => _h;
        public bool IsProcessing => _processing;

        void OnEnable()
        {
            _restartBinding = new EventBinding<RestartGameEvent>(_ => Restart());
            EventBus<RestartGameEvent>.Register(_restartBinding);
        }

        private void OnDisable() => EventBus<RestartGameEvent>.Deregister(_restartBinding);

        private void Start()
        {
            LoadSettings();
            InitializeBoard();
        }

        private void LoadSettings()
        {
            _w = GameSettings.GetWidth();
            _h = GameSettings.GetHeight();
            _cell = GameSettings.GetCellSize();
        }

        private void Restart()
        {
            ClearBoard();
            LoadSettings();
            InitializeBoard();
        }

        private void ClearBoard()
        {
            if (_grid != null)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                    for (int y = 0; y < _grid.GetLength(1); y++)
                        if (_grid[x, y]) Destroy(_grid[x, y].gameObject);
            }
            
            var bg = GameObject.Find("Background");
            if (bg) Destroy(bg);
        }

        public void InitializeBoard()
        {
            _origin = new Vector3(-_w * _cell / 2f, -_h * _cell / 2f, 0);
            _grid = new Gem[_w, _h];
            
            CreateTiles();
            _cameraScaler.AdjustCamera(_w, _h, _cell);
            StartCoroutine(SpawnAllGems());
        }

        private void CreateTiles()
        {
            if (GameObject.Find("Background")) return;
            
            var parent = new GameObject("Background");
            for (int x = 0; x < _w; x++)
                for (int y = 0; y < _h; y++)
                {
                    var tile = Instantiate(_tilePrefab, GetWorldPosCenter(x, y), Quaternion.identity, parent.transform);
                    tile.name = $"Tile_{x}_{y}";
                }
        }

        private IEnumerator SpawnAllGems()
        {
            _processing = true;
            EventBus<BoardProcessingEvent>.Raise(new BoardProcessingEvent(true));
            
            for (int y = 0; y < _h; y++)
            {
                for (int x = 0; x < _w; x++)
                    SpawnGem(x, y, true);
                yield return new WaitForSeconds(_spawnDelay);
            }
            
            _processing = false;
            EventBus<BoardProcessingEvent>.Raise(new BoardProcessingEvent(false));
        }

        private void SpawnGem(int x, int y, bool animate)
        {
            var gem = Instantiate(_gemPrefab, GetWorldPosCenter(x, y), Quaternion.identity);
            gem.Initialize(_gemDatabase.GetRandomGemType(), x, y);
            gem.name = $"Gem_{x}_{y}";
            _grid[x, y] = gem;
            
            if (animate) gem.PlaySpawnAnimation();
        }

        public void DestroyGems(List<Gem> gems)
        {
            if (_processing || gems == null || gems.Count == 0) return;
            StartCoroutine(DestroySequence(gems));
        }

        private IEnumerator DestroySequence(List<Gem> gems)
        {
            _processing = true;
            EventBus<BoardProcessingEvent>.Raise(new BoardProcessingEvent(true));

            var type = gems[0].Type;
            int count = gems.Count;

            foreach (var g in gems)
            {
                _grid[g.X, g.Y] = null;
                g.PlayDestroyAnimation();
            }

            yield return new WaitForSeconds(DESTROY_ANIM_WAIT);

            foreach (var g in gems)
                Destroy(g.gameObject);

            EventBus<GemsDestroyedEvent>.Raise(new GemsDestroyedEvent(count, type));

            yield return ApplyGravity();
            yield return RefillEmpty();

            EventBus<BoardRefillCompleteEvent>.Raise(new BoardRefillCompleteEvent());
            _processing = false;
            EventBus<BoardProcessingEvent>.Raise(new BoardProcessingEvent(false));
        }

        private IEnumerator ApplyGravity()
        {
            bool moved;
            do
            {
                moved = false;
                for (int x = 0; x < _w; x++)
                {
                    for (int y = 0; y < _h - 1; y++)
                    {
                        if (_grid[x, y] != null) continue;
                        
                        for (int above = y + 1; above < _h; above++)
                        {
                            if (_grid[x, above] == null) continue;
                            
                            var gem = _grid[x, above];
                            _grid[x, y] = gem;
                            _grid[x, above] = null;
                            gem.SetGridPosition(x, y);
                            gem.Fall(GetWorldPosCenter(x, y), _fallSpeed + (above - y) * _fallDelay);
                            moved = true;
                            break;
                        }
                    }
                }
                if (moved) yield return new WaitForSeconds(_fallSpeed);
            } while (moved);
        }

        private IEnumerator RefillEmpty()
        {
            for (int x = 0; x < _w; x++)
            {
                int offset = 0;
                for (int y = _h - 1; y >= 0; y--)
                {
                    if (_grid[x, y] != null) continue;
                    
                    var spawnPos = GetWorldPosCenter(x, _h + offset);
                    var gem = Instantiate(_gemPrefab, spawnPos, Quaternion.identity);
                    gem.Initialize(_gemDatabase.GetRandomGemType(), x, y);
                    gem.name = $"Gem_{x}_{y}";
                    _grid[x, y] = gem;
                    
                    gem.Fall(GetWorldPosCenter(x, y), _fallSpeed + (_h + offset - y) * _fallDelay);
                    offset++;
                }
            }
            yield return new WaitForSeconds(_fallSpeed + _h * _fallDelay);
        }

        public Gem GetGemAt(int x, int y) => IsValidPosition(x, y) ? _grid[x, y] : null;
        public bool IsValidPosition(int x, int y) => x >= 0 && x < _w && y >= 0 && y < _h;
        
        public Vector2Int WorldToGridPosition(Vector3 world)
        {
            var local = world - _origin;
            return new Vector2Int(Mathf.FloorToInt(local.x / _cell), Mathf.FloorToInt(local.y / _cell));
        }
        
        public Vector3 GetWorldPosCenter(int x, int y) => 
            new Vector3(x + CELL_CENTER_OFFSET, y + CELL_CENTER_OFFSET) * _cell + _origin;

        public List<Gem> GetGemsOfType(GemType type)
        {
            var result = new List<Gem>();
            for (int x = 0; x < _w; x++)
            {
                for (int y = 0; y < _h; y++)
                {
                    if (_grid[x, y] && _grid[x, y].Type == type)
                    {
                        result.Add(_grid[x, y]);
                    }
                }
            }
            return result;
        }

        public bool AreGemsMoving()
        {
            for (int x = 0; x < _w; x++)
            {
                for (int y = 0; y < _h; y++)
                {
                    if (_grid[x, y] && _grid[x, y].IsMoving)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
