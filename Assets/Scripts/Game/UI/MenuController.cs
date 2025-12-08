using Game.Board;
using Game.Constants;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class MenuController : MonoBehaviour
    {
        [Header("Sliders")] 
        [SerializeField]
        private Slider _widthSlider;

        [SerializeField] 
        private Slider _heightSlider;

        [SerializeField] 
        private Slider _movesSlider;

        [SerializeField] 
        private Slider _targetGemsSlider;

        [Header("Value Labels")]
        [SerializeField]
        private TextMeshProUGUI _widthLabel;

        [SerializeField] 
        private TextMeshProUGUI _heightLabel;

        [SerializeField] 
        private TextMeshProUGUI _movesLabel;

        [SerializeField] 
        private TextMeshProUGUI _targetGemsLabel;

        [Header("Gem Selection")]
        [SerializeField]
        private GemDatabase _gemDatabase;
        
        [SerializeField]
        private RectTransform _gemButtonsContainer;
        
        [SerializeField]
        private Button _gemButtonPrefab;

        [Header("Buttons")] 
        [SerializeField] 
        private Button _playButton;
                
        private Color _selectedColor = Color.green;
        private Color _normalColor = Color.white;

        private int _selectedGemIndex = 0;
        private Button[] _gemButtons;
        private const float SelectedGemScale = 1.2f;

        private void Start()
        {
            LoadDefaults();
            SetupListeners();
            CreateGemButtons();
        }

        private void LoadDefaults()
        {
            var config = LevelConfiguration.Instance;

            _widthSlider.value = config.Width;
            _heightSlider.value = config.Height;
            _movesSlider.value = config.MoveLimit;
            _targetGemsSlider.value = config.TargetGemCount;

            _selectedGemIndex = GameSettings.TargetGemTypeIndex >= 0 ? GameSettings.TargetGemTypeIndex : 0;

            UpdateLabels();
        }

        private void SetupListeners()
        {
            _widthSlider.onValueChanged.AddListener(_ => UpdateLabels());
            _heightSlider.onValueChanged.AddListener(_ => UpdateLabels());
            _movesSlider.onValueChanged.AddListener(_ => UpdateLabels());
            _targetGemsSlider.onValueChanged.AddListener(_ => UpdateLabels());

            _playButton.onClick.AddListener(OnPlayClicked);
        }

        private void CreateGemButtons()
        {
            if (_gemDatabase == null || _gemButtonPrefab == null || _gemButtonsContainer == null)
                return;

            foreach (Transform child in _gemButtonsContainer)
            {
                Destroy(child.gameObject);
            }

            _gemButtons = new Button[_gemDatabase.GemTypes.Length];

            for (int i = 0; i < _gemDatabase.GemTypes.Length; i++)
            {
                var gemType = _gemDatabase.GemTypes[i];
                var btn = Instantiate(_gemButtonPrefab, _gemButtonsContainer);
                _gemButtons[i] = btn;

                var icon = btn.GetComponentInChildren<Image>();
                if (icon != null && gemType.sprite != null)
                {
                    icon.sprite = gemType.sprite;
                }

                int index = i;
                btn.onClick.AddListener(() => SelectGem(index));
            }

            SelectGem(_selectedGemIndex);
        }

        private void SelectGem(int index)
        {
            _selectedGemIndex = index;

            for (int i = 0; i < _gemButtons.Length; i++)
            {
                var colors = _gemButtons[i].colors;
                colors.normalColor = i == index ? _selectedColor : _normalColor;
                colors.selectedColor = i == index ? _selectedColor : _normalColor;
                _gemButtons[i].colors = colors;
                
                _gemButtons[i].transform.localScale = i == index ? Vector3.one * SelectedGemScale : Vector3.one;
            }
        }

        private void UpdateLabels()
        {
            _widthLabel.text = $"Width: {(int)_widthSlider.value}";
            _heightLabel.text = $"Height: {(int)_heightSlider.value}";
            _movesLabel.text = $"Moves: {(int)_movesSlider.value}";
            _targetGemsLabel.text = $"Target: {(int)_targetGemsSlider.value}";
        }

        private void OnPlayClicked()
        {
            SaveSettings();
            SceneManager.LoadScene(SceneNames.GameScene);
        }

        private void SaveSettings()
        {
            GameSettings.BoardWidth = (int)_widthSlider.value;
            GameSettings.BoardHeight = (int)_heightSlider.value;
            GameSettings.MoveLimit = (int)_movesSlider.value;
            GameSettings.TargetGemCount = (int)_targetGemsSlider.value;
            GameSettings.TargetGemTypeIndex = _selectedGemIndex;
            GameSettings.IsConfigured = true;
        }

        private void OnDestroy()
        {
            _widthSlider.onValueChanged.RemoveAllListeners();
            _heightSlider.onValueChanged.RemoveAllListeners();
            _movesSlider.onValueChanged.RemoveAllListeners();
            _targetGemsSlider.onValueChanged.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
        }
    }
}
