using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using ShooterBoss.Core;

namespace ShooterBoss.UI
{
    /// <summary>
    /// Único script da cena Resultado. Lê MatchResult.PlayerWon (setado pelo
    /// GameFlowController antes de trocar de cena) e mostra VITÓRIA ou DERROTA.
    /// Por enunciado, o botão de tentar de novo só aparece na derrota.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ResultScreenController : MonoBehaviour
    {
        private Button _retryButton;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var title = root.Q<Label>("result-title");
            var subtitle = root.Q<Label>("result-subtitle");
            _retryButton = root.Q<Button>("retry-button");

            bool victory = MatchResult.PlayerWon;

            title.text = victory ? "VITÓRIA!" : "DERROTA";
            title.style.color = victory ? new Color(0.3f, 0.9f, 0.3f) : new Color(0.9f, 0.3f, 0.3f);
            subtitle.text = victory ? "Você derrotou o chefão." : "O chefão foi mais forte dessa vez.";

            _retryButton.style.display = victory ? DisplayStyle.None : DisplayStyle.Flex;
            _retryButton.clicked += Retry;
        }

        private void OnDisable()
        {
            if (_retryButton != null) _retryButton.clicked -= Retry;
        }

        private void Retry()
        {
            Time.timeScale = 1f; // defensivo, caso algo mais tenha pausado o jogo
            SceneManager.LoadScene(SceneNames.Boot, LoadSceneMode.Single);
        }
    }
}
