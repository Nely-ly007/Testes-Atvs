using UnityEngine;
using UnityEngine.UIElements;
using ShooterBoss.Player;
using ShooterBoss.Boss;

namespace ShooterBoss.UI
{
    /// <summary>
    /// Liga os eventos de PlayerHealth/BossHealth aos elementos do UXML.
    /// Estrutura esperada no UXML (ids):
    ///   #lives-container (com N filhos VisualElement classe "life-icon")
    ///   #boss-health-bar (VisualElement, largura = %vida via style.width)
    ///   #end-screen (container, começa com display:none)
    ///   #end-title (Label: "VITÓRIA!" ou "DERROTA")
    ///   #retry-button (Button, só visível na derrota)
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private BossHealth bossHealth;

        private VisualElement _root;
        private VisualElement _livesContainer;
        private VisualElement _bossBar;
        private VisualElement _endScreen;
        private Label _endTitle;
        private Button _retryButton;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _livesContainer = _root.Q<VisualElement>("lives-container");
            _bossBar = _root.Q<VisualElement>("boss-health-bar");
            _endScreen = _root.Q<VisualElement>("end-screen");
            _endTitle = _root.Q<Label>("end-title");
            _retryButton = _root.Q<Button>("retry-button");

            _endScreen.style.display = DisplayStyle.None;
            _retryButton.clicked += Retry;

            playerHealth.OnLivesChanged += UpdateLives;
            playerHealth.OnPlayerDied += () => ShowEnd(false);
            bossHealth.OnHpChanged += UpdateBossBar;
            bossHealth.OnBossDefeated += () => ShowEnd(true);
        }

        private void OnDisable()
        {
            playerHealth.OnLivesChanged -= UpdateLives;
            bossHealth.OnHpChanged -= UpdateBossBar;
            _retryButton.clicked -= Retry;
        }

        private void UpdateLives(int current, int max)
        {
            for (int i = 0; i < _livesContainer.childCount; i++)
                _livesContainer[i].style.visibility = i < current ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateBossBar(int current, int max)
        {
            float pct = max > 0 ? (float)current / max : 0f;
            _bossBar.style.width = Length.Percent(pct * 100f);

            // muda a cor conforme o estado de vida do chefão
            Color c = pct <= 0.2f ? new Color(0.8f, 0.15f, 0.15f)
                    : pct <= 0.5f ? new Color(0.9f, 0.6f, 0.1f)
                    : new Color(0.2f, 0.75f, 0.2f);
            _bossBar.style.backgroundColor = c;
        }

        private void ShowEnd(bool victory)
        {
            Time.timeScale = 0f;
            _endScreen.style.display = DisplayStyle.Flex;
            _endTitle.text = victory ? "VITÓRIA!" : "DERROTA";
            _retryButton.style.display = victory ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
