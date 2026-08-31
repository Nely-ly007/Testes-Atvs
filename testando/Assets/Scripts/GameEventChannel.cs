using System;
using UnityEngine;

/// <summary>
/// Canal de eventos estático (padrão Observer) para desacoplar Player, Moedas,
/// Checkpoints e Zona de Vitória do GameManager.
/// </summary>
public static class GameEventChannel
{
    // Disparado quando uma moeda é coletada. Parâmetro: ID único da moeda.
    public static event Action<string> OnCoinCollected;

    // Disparado quando o player entra na área de checkpoint. Parâmetros: ID do checkpoint, posição central.
    public static event Action<string, Vector3> OnCheckpointReached;

    // Disparado quando o player entra na zona de vitória.
    public static event Action OnVictoryReached;

    public static void RaiseCoinCollected(string coinID)
    {
        OnCoinCollected?.Invoke(coinID);
    }

    public static void RaiseCheckpointReached(string checkpointID, Vector3 position)
    {
        OnCheckpointReached?.Invoke(checkpointID, position);
    }

    public static void RaiseVictoryReached()
    {
        OnVictoryReached?.Invoke();
    }
}
