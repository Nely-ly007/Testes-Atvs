using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Controla a tela de vitória (UI Toolkit). Mostra "moedas coletadas / total" e
/// aguarda o jogador apertar uma tecla para avançar para a próxima fase.
/// Coloque este script num GameObject com UIDocument na cena da fase, com o
/// UXML contendo um Label "VictoryLabel" e um Label "ContinueLabel" (ex: "Aperte ESPAÇO para continuar").
/// </summary>
public class VictoryUI : MonoBehaviour
{
    public static VictoryUI Instance { get; private set; }

    [Tooltip("Nome da cena da próxima fase. Deixe vazio se for a última fase.")]
    public string nextSceneName;

    private UIDocument _document;
    private VisualElement _root;
    private Label _victoryLabel;
    private bool _waitingForInput = false;

    private void Awake()
    {
        Instance = this;
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;
        _victoryLabel = _root.Q<Label>("VictoryLabel");

        // Começa escondida
        _root.style.display = DisplayStyle.None;
    }

    public void ShowVictory(int coinsCollected, int totalCoins)
    {
        _victoryLabel.text = $"Fase concluída!\nMoedas: {coinsCollected} / {totalCoins}\n\nAperte ESPAÇO para continuar";
        _root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        _waitingForInput = true;
    }

    private void Update()
    {
        if (!_waitingForInput) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _waitingForInput = false;
            Time.timeScale = 1f;
            GoToNextPhase();
        }
    }

    private void GoToNextPhase()
    {
        GameManager.Instance.ResetPhaseState();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene("MenuInicial");
        }
    }
}