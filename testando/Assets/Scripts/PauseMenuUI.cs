using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o Menu de Pausa (UI Toolkit). Abre/fecha com a tecla P.
/// Espera um UXML com botões: "SaveButton", "LoadButton", "BackToMenuButton".
/// Coloque este script num GameObject persistente (ou em cada fase) com UIDocument.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _root;

    private Button _saveButton;
    private Button _loadButton;
    private Button _backToMenuButton;

    private bool _isPaused = false;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        _saveButton = _root.Q<Button>("SaveButton");
        _loadButton = _root.Q<Button>("LoadButton");
        _backToMenuButton = _root.Q<Button>("BackToMenuButton");

        _root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        _saveButton.clicked += OnSave;
        _loadButton.clicked += OnLoad;
        _backToMenuButton.clicked += OnBackToMenu;
    }

    private void OnDisable()
    {
        _saveButton.clicked -= OnSave;
        _loadButton.clicked -= OnLoad;
        _backToMenuButton.clicked -= OnBackToMenu;
    }

    private void Update()
    {
        // Troque por uma Input Action do seu Input System se preferir, mantendo o padrão do projeto.
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;
        _root.style.display = _isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void ClosePauseMenu()
    {
        _isPaused = false;
        _root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void OnSave()
    {
        SaveSlotsUI.Instance.Open(SaveSlotsUI.SlotMode.Save);
    }

    private void OnLoad()
    {
        SaveSlotsUI.Instance.Open(SaveSlotsUI.SlotMode.Load);
    }

    private void OnBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicial");
    }
}
