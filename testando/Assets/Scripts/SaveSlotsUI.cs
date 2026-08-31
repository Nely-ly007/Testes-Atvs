using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Tela de seleção de slots de save (UI Toolkit), reutilizada tanto para
/// "Salvar Jogo" quanto para "Carregar Jogo" — o modo é definido em Open().
/// Espera um UXML com botões: "Slot1Button", "Slot2Button", "Slot3Button", "CloseButton".
/// Coloque este script num GameObject persistente (ex: em _Boot) com UIDocument,
/// já que precisa ser acessível tanto do Menu Inicial quanto do Pause Menu.
/// </summary>
public class SaveSlotsUI : MonoBehaviour
{
    public static SaveSlotsUI Instance { get; private set; }

    public enum SlotMode { Save, Load }

    private UIDocument _document;
    private VisualElement _root;

    private Button _slot1Button;
    private Button _slot2Button;
    private Button _slot3Button;
    private Button _closeButton;

    private SlotMode _currentMode;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        _slot1Button = _root.Q<Button>("Slot1Button");
        _slot2Button = _root.Q<Button>("Slot2Button");
        _slot3Button = _root.Q<Button>("Slot3Button");
        _closeButton = _root.Q<Button>("CloseButton");

        _slot1Button.clicked += () => OnSlotClicked(1);
        _slot2Button.clicked += () => OnSlotClicked(2);
        _slot3Button.clicked += () => OnSlotClicked(3);
        _closeButton.clicked += Close;

        _root.style.display = DisplayStyle.None;
    }

    public void Open(SlotMode mode)
    {
        _currentMode = mode;
        RefreshSlotLabels();
        _root.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        _root.style.display = DisplayStyle.None;
    }

    private void RefreshSlotLabels()
    {
        UpdateSlotButtonText(_slot1Button, 1);
        UpdateSlotButtonText(_slot2Button, 2);
        UpdateSlotButtonText(_slot3Button, 3);
    }

    private void UpdateSlotButtonText(Button button, int slot)
    {
        bool hasSave = SaveManager.Instance.HasSave(slot);
        string status = hasSave ? "Ocupado" : "Vazio";
        button.text = $"Slot {slot} ({status})";
    }

    private void OnSlotClicked(int slot)
    {
        if (_currentMode == SlotMode.Save)
        {
            GameManager.Instance.SaveGameToSlot(slot);
        }
        else
        {
            GameManager.Instance.LoadGameFromSlot(slot);
        }

        Close();
    }
}