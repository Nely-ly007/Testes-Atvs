using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe de dados serializável que representa o estado salvo do jogo.
/// Cada slot de save gera uma instância dessa classe, serializada em JSON e encriptada em disco.
/// </summary>
[Serializable]
public class SaveData
{
    // Nome da cena/fase em que o save foi feito
    public string sceneName;

    // Quantidade de moedas que o jogador tinha no momento do checkpoint (não no momento do save em si)
    public int coinsCollected;

    // IDs únicos das moedas já coletadas até o checkpoint ativo (para não reaparecerem)
    public List<string> collectedCoinIDs = new List<string>();

    // Se o jogador já passou por algum checkpoint nesta fase
    public bool hasPassedCheckpoint;

    // ID do checkpoint ativo (útil se houver mais de um checkpoint por fase no futuro)
    public string activeCheckpointID;

    // Posição central do checkpoint, para o player reaparecer exatamente ali
    public float checkpointPosX;
    public float checkpointPosY;
    public float checkpointPosZ;

    // Se este slot está vazio/nunca foi usado
    public bool isEmpty = true;

    public Vector3 GetCheckpointPosition()
    {
        return new Vector3(checkpointPosX, checkpointPosY, checkpointPosZ);
    }

    public void SetCheckpointPosition(Vector3 pos)
    {
        checkpointPosX = pos.x;
        checkpointPosY = pos.y;
        checkpointPosZ = pos.z;
    }
}
