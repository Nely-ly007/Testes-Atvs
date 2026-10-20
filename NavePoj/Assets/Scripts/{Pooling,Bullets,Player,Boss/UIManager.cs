using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using ShooterBoss.Core;

namespace ShooterBoss.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _livesContainer;
        private VisualElement _bossBar;

        private void Start()
        {
            StartCoroutine(InitializeUI());
        }

        private IEnumerator InitializeUI()
        {
            // Aguarda até que a instância de GameplayRefs esteja disponível
            while (GameplayRefs.Instance == null)
            {
                yield return null;
            }

            // Aguarda os componentes internos de saúde serem atribuídos, se necessário
            while (GameplayRefs.Instance.PlayerHealth == null || GameplayRefs.Instance.BossHealth == null)
            {
                yield return null;
            }

            _root = GetComponent<UIDocument>().rootVisualElement;
            _livesContainer = _root.Q<VisualElement>("lives-container");
            _bossBar = _root.Q<VisualElement>("boss-health-bar");

            var playerHealth = GameplayRefs.Instance.PlayerHealth;
            var bossHealth = GameplayRefs.Instance.BossHealth;

            // Inscreve nos eventos
            playerHealth.OnLivesChanged += UpdateLives;
            bossHealth.OnHpChanged += UpdateBossBar;

            // Sincroniza estado inicial
            UpdateLives(playerHealth.CurrentLives, playerHealth.MaxLives);
            UpdateBossBar(bossHealth.CurrentHp, bossHealth.MaxHp);
        }

        private void OnDisable()
        {
            if (GameplayRefs.Instance == null) return;

            if (GameplayRefs.Instance.PlayerHealth != null)
                GameplayRefs.Instance.PlayerHealth.OnLivesChanged -= UpdateLives;

            if (GameplayRefs.Instance.BossHealth != null)
                GameplayRefs.Instance.BossHealth.OnHpChanged -= UpdateBossBar;
        }

        private void UpdateLives(int current, int max)
        {
            if (_livesContainer == null) return;
            for (int i = 0; i < _livesContainer.childCount; i++)
                _livesContainer[i].style.visibility = i < current ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateBossBar(int current, int max)
        {
            if (_bossBar == null) return;
            float pct = max > 0 ? (float)current / max : 0f;
            _bossBar.style.width = Length.Percent(pct * 100f);

            Color c = pct <= 0.2f ? new Color(0.8f, 0.15f, 0.15f)
                    : pct <= 0.5f ? new Color(0.9f, 0.6f, 0.1f)
                    : new Color(0.2f, 0.75f, 0.2f);
            _bossBar.style.backgroundColor = c;
        }
    }
}