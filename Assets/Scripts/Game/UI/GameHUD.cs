using Core.Events;
using Game.Board;
using Game.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _scoreText;

        [SerializeField] 
        private TextMeshProUGUI _movesText;

        [SerializeField] 
        private TextMeshProUGUI _targetText;

        [SerializeField] 
        private Image _targetGemIcon;

        [SerializeField] 
        private Slider _progressBar;

        [SerializeField] 
        private TextMeshProUGUI _progressText;

        private EventBinding<ScoreChangedEvent> _scoreBinding;
        private EventBinding<MovesChangedEvent> _movesBinding;
        private EventBinding<TargetProgressEvent> _progressBinding;
        private EventBinding<LinkUpdatedEvent> _linkBinding;
        private EventBinding<LinkCancelledEvent> _cancelBinding;
        private EventBinding<GameInitializedEvent> _initBinding;

        private void OnEnable()
        {
            _scoreBinding = new EventBinding<ScoreChangedEvent>(OnScoreChanged);
            _movesBinding = new EventBinding<MovesChangedEvent>(OnMovesChanged);
            _progressBinding = new EventBinding<TargetProgressEvent>(OnProgressChanged);
            _initBinding = new EventBinding<GameInitializedEvent>(OnGameInit);

            EventBus<ScoreChangedEvent>.Register(_scoreBinding);
            EventBus<MovesChangedEvent>.Register(_movesBinding);
            EventBus<TargetProgressEvent>.Register(_progressBinding);
            EventBus<GameInitializedEvent>.Register(_initBinding);
        }

        private void OnDisable()
        {
            EventBus<ScoreChangedEvent>.Deregister(_scoreBinding);
            EventBus<MovesChangedEvent>.Deregister(_movesBinding);
            EventBus<TargetProgressEvent>.Deregister(_progressBinding);
            EventBus<LinkUpdatedEvent>.Deregister(_linkBinding);
            EventBus<LinkCancelledEvent>.Deregister(_cancelBinding);
            EventBus<GameInitializedEvent>.Deregister(_initBinding);
        }

        private void OnGameInit(GameInitializedEvent e)
        {
            if (_targetGemIcon && e.TargetGemType != null)
            {
                _targetGemIcon.sprite = e.TargetGemType.sprite;
            }

            _targetText.text = $"Collect: 0/{e.TargetGemCount}";
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            _scoreText.text = $"Score: {e.CurrentScore}";
        }

        private void OnMovesChanged(MovesChangedEvent e)
        {
            _movesText.text = $"Moves: {e.RemainingMoves}";
        }

        private void OnProgressChanged(TargetProgressEvent e)
        {
            _targetText.text = $"Collect: {e.CurrentCount}/{e.TargetCount}";

            _progressBar.value = e.Progress;

            _progressText.text = $"{Mathf.RoundToInt(e.Progress * 100)}%";

            if (_targetGemIcon && e.TargetType != null)
            {
                _targetGemIcon.sprite = e.TargetType.sprite;
            }
        }
    }
}

