using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o Menu Inicial (UI Toolkit). Espera um UXML com botões:
/// "ContinueButton", "NewGameButton", "LoadGameButton", "QuitButton".
/// ContinueButton só aparece se existir autosave (slot 0).
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Tooltip("Nome da cena da primeira fase, usada em Novo Jogo.")]
    public string firstPhaseSceneName = "Fase1";

    private UIDocument _document;
    private VisualElement _root;

    private Button _continueButton;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _quitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        _continueButton = _root.Q<Button>("ContinueButton");
        _newGameButton = _root.Q<Button>("NewGameButton");
        _loadGameButton = _root.Q<Button>("LoadGameButton");
        _quitButton = _root.Q<Button>("QuitButton");
    }

    private void OnEnable()
    {
        _newGameButton.clicked += OnNewGame;
        _loadGameButton.clicked += OnLoadGame;
        _quitButton.clicked += OnQuit;

        if (_continueButton != null)
        {
            _continueButton.clicked += OnContinue;
            // Só mostra "Continuar Jogo" se o autosave (slot 0) existir
            bool hasAutosave = SaveManager.Instance != null && SaveManager.Instance.HasSave(0);
            _continueButton.style.display = hasAutosave ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void OnDisable()
    {
        _newGameButton.clicked -= OnNewGame;
        _loadGameButton.clicked -= OnLoadGame;
        _quitButton.clicked -= OnQuit;

        if (_continueButton != null)
        {
            _continueButton.clicked -= OnContinue;
        }
    }

    private void OnNewGame()
    {
        GameManager.Instance.ResetPhaseState();
        SceneManager.LoadScene(firstPhaseSceneName);
    }

    private void OnContinue()
    {
        GameManager.Instance.LoadGameFromSlot(0);
    }

    private void OnLoadGame()
    {
        // Abre a tela de slots em modo "Load".
        SaveSlotsUI.Instance.Open(SaveSlotsUI.SlotMode.Load);
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
