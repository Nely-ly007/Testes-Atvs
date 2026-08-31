using UnityEngine;

/// <summary>
/// Moeda coletável. Cada moeda em cena precisa de um coinID único
/// (defina manualmente no Inspector, ex: "fase1_moeda_01") para que o
/// sistema de save saiba exatamente quais moedas esconder ao recarregar.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Tooltip("ID único desta moeda dentro da fase. Ex: fase1_moeda_01")]
    public string coinID;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (string.IsNullOrEmpty(coinID))
        {
            Debug.LogWarning($"[Coin] {name} não tem coinID definido!", this);
        }

        GameEventChannel.RaiseCoinCollected(coinID);
        gameObject.SetActive(false);
    }
}
