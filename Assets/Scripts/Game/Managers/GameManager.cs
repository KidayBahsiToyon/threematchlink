using Core.Events;
using Game.Board;
using Game.Events;
using Game.Services;
using UnityEngine;
using static Game.Constants.GameConstants;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour, IGameService
    {
        [SerializeField] 
        private GemDatabase _gemDatabase;
        
        private int _score, _moves, _collected;
        private int _targetCount, _moveLimit, _scorePerGem;
        private bool _gameOver;
        private GemType _targetType;

        EventBinding<MatchCompletedEvent> _matchBinding;
        EventBinding<LinkCancelledEvent> _cancelBinding;
        EventBinding<RestartGameEvent> _restartBinding;

        public int CurrentScore => _score;
        public int RemainingMoves => _moves;
        public int TargetScore => _targetCount;
        public bool IsGameOver => _gameOver;
        public GemType TargetGemType => _targetType;

        private void OnEnable()
        {
            _matchBinding = new EventBinding<MatchCompletedEvent>(OnMatch);
            _cancelBinding = new EventBinding<LinkCancelledEvent>(_ => { });
            _restartBinding = new EventBinding<RestartGameEvent>(_ => InitializeGame());

            EventBus<MatchCompletedEvent>.Register(_matchBinding);
            EventBus<LinkCancelledEvent>.Register(_cancelBinding);
            EventBus<RestartGameEvent>.Register(_restartBinding);
        }

        private void OnDisable()
        {
            EventBus<MatchCompletedEvent>.Deregister(_matchBinding);
            EventBus<LinkCancelledEvent>.Deregister(_cancelBinding);
            EventBus<RestartGameEvent>.Deregister(_restartBinding);
        }

        private void Start() => InitializeGame();

        public void InitializeGame()
        {
            _moveLimit = GameSettings.GetMoveLimit();
            _targetCount = GameSettings.GetTargetGemCount();
            _scorePerGem = GameSettings.GetScorePerGem();
            
            _score = _collected = 0;
            _moves = _moveLimit;
            _gameOver = false;

            PickTargetGem();

            EventBus<MovesChangedEvent>.Raise(new MovesChangedEvent(_moves, _moveLimit));
            EventBus<TargetProgressEvent>.Raise(new TargetProgressEvent(_collected, _targetCount, _targetType));
            EventBus<GameInitializedEvent>.Raise(new GameInitializedEvent(
                _targetCount, _moveLimit, GameSettings.GetWidth(), GameSettings.GetHeight(), _targetType));
        }

        private void PickTargetGem()
        {
            if (_gemDatabase == null || _gemDatabase.GemTypes.Length == 0) return;
            
            int idx = GameSettings.TargetGemTypeIndex;
            if (idx < 0 || idx >= _gemDatabase.GemTypes.Length)
                idx = Random.Range(0, _gemDatabase.GemTypes.Length);
            
            _targetType = _gemDatabase.GemTypes[idx];
        }

        private void OnMatch(MatchCompletedEvent e)
        {
            if (_gameOver) return;

            bool isTarget = e.GemType == _targetType;
            int earned = CalcScore(e.GemCount, isTarget);
            
            _score += earned;
            _moves--;
            
            if (isTarget)
            {
                _collected += e.GemCount;
                EventBus<TargetProgressEvent>.Raise(new TargetProgressEvent(_collected, _targetCount, _targetType));
            }

            EventBus<ScoreChangedEvent>.Raise(new ScoreChangedEvent(_score, _targetCount));
            EventBus<MatchScoredEvent>.Raise(new MatchScoredEvent(e.GemCount, earned, isTarget));
            EventBus<MovesChangedEvent>.Raise(new MovesChangedEvent(_moves, _moveLimit));
            
            CheckGameEnd();
        }

        private int CalcScore(int gems, bool isTarget)
        {
            int score = gems * _scorePerGem;
            int bonusGems = Mathf.Max(0, gems - BONUS_THRESHOLD);
            score += bonusGems * _scorePerGem * BONUS_MULTIPLIER;
            if (isTarget) score += gems * _scorePerGem;
            return score;
        }

        private void CheckGameEnd()
        {
            if (_collected >= _targetCount)
            {
                _gameOver = true;
                EventBus<GameOverEvent>.Raise(new GameOverEvent(true, _score, _collected, _targetCount));
            }
            else if (_moves <= 0)
            {
                _gameOver = true;
                EventBus<GameOverEvent>.Raise(new GameOverEvent(false, _score, _collected, _targetCount));
            }
        }

        public void RestartLevel() => EventBus<RestartGameEvent>.Raise(new RestartGameEvent());
        
        public void AddMoves(int n)
        {
            _moves += n;
            EventBus<MovesChangedEvent>.Raise(new MovesChangedEvent(_moves, _moveLimit));
        }

        public void AddScore(int n)
        {
            _score += n;
            EventBus<ScoreChangedEvent>.Raise(new ScoreChangedEvent(_score, _targetCount));
            CheckGameEnd();
        }
        
        public bool IsTargetGemType(GemType t) => t == _targetType;
    }
}
