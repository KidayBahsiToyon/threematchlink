using System.Collections;
using Core.Events;
using Game.Constants;
using Game.Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] 
        private GameObject _panel;
        
        [SerializeField] 
        private CanvasGroup _canvasGroup;
        
        [Header("Result")]
        [SerializeField] 
        private TextMeshProUGUI _titleText;
        
        [SerializeField] 
        private TextMeshProUGUI _infoText;
        
        [SerializeField] 
        private TextMeshProUGUI _scoreText;
        
        [Header("Buttons")]
        [SerializeField] 
        private Button _restartButton;
        
        [SerializeField] 
        private Button _menuButton;
        
        [SerializeField] 
        private float _fadeInDuration = 0.3f;
        
        private EventBinding<GameOverEvent> _gameOverBinding;

        private void OnEnable()
        {
            _gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
            EventBus<GameOverEvent>.Register(_gameOverBinding);
        }

        private void OnDisable()
        {
            EventBus<GameOverEvent>.Deregister(_gameOverBinding);
        }

        private void Start()
        {
            Hide();
            SetupButtons();
        }

        private void SetupButtons()
        {
            _restartButton.onClick.AddListener(OnRestartClicked);
            _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnGameOver(GameOverEvent e)
        {
            Show(e.IsWin, e.FinalScore, e.GemsCollected, e.TargetGems);
        }

        public void Show(bool isWin, int score, int collected, int target)
        {
            _panel.SetActive(true);
            
            _titleText.text = isWin ? "YOU WIN!" : "GAME OVER";
            _infoText.text = isWin ? $"You collected {collected} gems!" : $"Only {collected}/{target} gems collected";
            _scoreText.text = $"Score: {score}";
            
            _canvasGroup.alpha = 0;
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = elapsed / _fadeInDuration;
                yield return null;
            }
            _canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        private void OnRestartClicked()
        {
            Hide();
            EventBus<RestartGameEvent>.Raise(new RestartGameEvent());
        }

        private void OnMenuClicked()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}

