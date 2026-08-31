using UnityEngine;

/// <summary>
/// Área de checkpoint. Ao ser tocada pelo player, dispara o evento de checkpoint
/// com a posição CENTRAL do checkpoint (não a posição onde o player encostou),
/// conforme exigido: o player deve reaparecer no centro do checkpoint mesmo que
/// tenha passado só de raspão pela borda do trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("ID único deste checkpoint dentro da fase. Ex: fase1_checkpoint_01")]
    public string checkpointID;

    [Tooltip("Ponto exato onde o player deve reaparecer. Se vazio, usa a posição deste objeto.")]
    public Transform spawnPoint;

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

        Vector3 centerPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        GameEventChannel.RaiseCheckpointReached(checkpointID, centerPosition);
    }
}
