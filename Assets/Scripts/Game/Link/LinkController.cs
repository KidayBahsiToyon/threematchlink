using System.Collections.Generic;
using Core.Events;
using Core.ServiceLocator;
using Game.Board;
using Game.Events;
using Game.Managers;
using Game.Services;
using UnityEngine;

namespace Game.Link
{
    public class LinkController : MonoBehaviour, ILinkService
    {
        [SerializeField] 
        private LinkLine _line;
        [SerializeField] 
        private Camera _cam;
        [SerializeField] 
        private bool _diagonal = true;
        [SerializeField] 
        private LayerMask _gemLayer = -1;

        private IBoardService _board;
        private IGameService _game;
        private List<Gem> _chain = new();
        private GemType _type;
        private int _min;
        private bool _drag;

        public List<Gem> LinkedGems => _chain;
        public bool IsDragging => _drag;
        public int MinimumMatchCount => _min;

        private void Start()
        {
            var loc = ServiceLocator.ForSceneOf(this);
            _board = loc.Get<IBoardService>();
            _game = loc.Get<IGameService>();
            _min = GameSettings.GetMinMatchCount();
            if (!_cam)
            {
                _cam = Camera.main;
            }
        }

        private void Update()
        {
            if (_board == null || _board.IsProcessing || _game.IsGameOver) return;

            if (Input.GetMouseButtonDown(0))
            {
                TryStart();
            }
            else if (Input.GetMouseButton(0) && _drag)
            {
                TryContinue(); UpdatePointer();
            }
            else if (Input.GetMouseButtonUp(0) && _drag)
            {
                End();
            }
        }

        private void TryStart()
        {
            var gem = GetGemAt();
            if (gem && !gem.IsMoving)
            {
                _drag = true;
                _type = gem.Type;
                _chain.Clear();
                Add(gem);
            }
        }

        private void TryContinue()
        {
            var gem = GetGemAt();
            if (!gem || gem.IsMoving)
            {
                return;
            }

            // backtrack
            if (_chain.Count >= 2 && gem == _chain[^2])
            {
                _chain.RemoveAt(_chain.Count - 1);
                _line.UpdateLine(_chain);
                RaiseUpdate();
                return;
            }

            if (_chain.Contains(gem))
            {
                return;
            }
            if (gem.Type == _type && Adjacent(gem))
            {
                Add(gem);
            }
        }

        private void Add(Gem gem)
        {
            _chain.Add(gem);
            gem.PlaySelectAnimation();
            _line.UpdateLine(_chain);
            RaiseUpdate();
        }

        private void RaiseUpdate()
        {
            bool target = (_game is GameManager gm) && gm.IsTargetGemType(_type);
            EventBus<LinkUpdatedEvent>.Raise(new LinkUpdatedEvent(_chain, _min, target));
        }

        private void End()
        {
            _drag = false;
            _line.ClearLine();

            if (_chain.Count >= _min)
            {
                var matched = new List<Gem>(_chain);
                bool target = (_game is GameManager gm) && gm.IsTargetGemType(_type);
                EventBus<MatchCompletedEvent>.Raise(new MatchCompletedEvent(matched, target));
                _board.DestroyGems(matched);
            }
            else
            {
                EventBus<LinkCancelledEvent>.Raise(new LinkCancelledEvent());
            }

            _chain.Clear();
            _type = null;
        }

        private bool Adjacent(Gem gem)
        {
            if (_chain.Count == 0)
            {
                return true;
            }
            var last = _chain[^1];
            int dx = Mathf.Abs(gem.X - last.X), dy = Mathf.Abs(gem.Y - last.Y);
            return _diagonal ? (dx <= 1 && dy <= 1 && dx + dy > 0) : (dx + dy == 1);
        }

        private Gem GetGemAt()
        {
            var pos = _cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;

            var hit = Physics2D.Raycast(pos, Vector2.zero, 0f, _gemLayer);
            if (hit.collider)
            {
                return hit.collider.GetComponent<Gem>();
            }

            var grid = _board.WorldToGridPosition(pos);
            return _board.GetGemAt(grid.x, grid.y);
        }

        private void UpdatePointer()
        {
            if (_chain.Count == 0) return;
            var pos = _cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            _line.SetPointerPosition(pos);
        }

        // ILinkService
        public void CancelLink()
        {
            _drag = false;
            _chain.Clear();
            _type = null;
            _line.ClearLine();
            EventBus<LinkCancelledEvent>.Raise(new LinkCancelledEvent());
        }

        public int GetCurrentLinkCount() => _chain.Count;
        public bool IsCurrentLinkValid() => _chain.Count >= _min;
    }
}
