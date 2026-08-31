using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// HUD simples que mostra "Moedas: X" durante o gameplay.
/// Escuta o GameEventChannel diretamente (Observer pattern), então não precisa
/// de referência ao GameManager nem de Update() a cada frame.
/// Coloque este script num GameObject com UIDocument em cada cena de fase.
/// Espera um UXML com um Label chamado "CoinCountLabel".
/// </summary>
public class CoinHUD : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _root;
    private Label _coinCountLabel;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;
        _coinCountLabel = _root.Q<Label>("CoinCountLabel");
    }

    private void Start()
    {
        // Ao iniciar a fase, mostra o valor atual (útil ao carregar um save/checkpoint,
        // onde o GameManager já populou coinsCollected antes desta cena carregar o HUD)
        UpdateLabel(GameManager.Instance.coinsCollected);
    }

    private void OnEnable()
    {
        GameEventChannel.OnCoinCollected += HandleCoinCollected;
    }

    private void OnDisable()
    {
        GameEventChannel.OnCoinCollected -= HandleCoinCollected;
    }

    private void HandleCoinCollected(string coinID)
    {
        UpdateLabel(GameManager.Instance.coinsCollected);
    }

    private void UpdateLabel(int count)
    {
        if (_coinCountLabel != null)
        {
            _coinCountLabel.text = $"Moedas: {count}";
        }
    }
}
