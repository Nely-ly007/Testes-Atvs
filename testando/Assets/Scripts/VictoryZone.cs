using UnityEngine;

/// <summary>
/// Área de vitória da fase. Ao ser tocada pelo player, dispara o evento de vitória
/// (que já dispara o autosave dentro do GameManager) e informa a UI de vitória.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class VictoryZone : MonoBehaviour
{
    [Tooltip("Total de moedas existentes nesta fase, para exibir X/Total na tela de vitória.")]
    public int totalCoinsInPhase;

    private bool _alreadyTriggered = false;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_alreadyTriggered) return;
        if (!other.CompareTag("Player")) return;

        _alreadyTriggered = true;

        GameEventChannel.RaiseVictoryReached();

        if (VictoryUI.Instance != null)
        {
            VictoryUI.Instance.ShowVictory(GameManager.Instance.coinsCollected, totalCoinsInPhase);
        }
    }
}
