using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton central do jogo. Mantém o estado da fase atual (moedas coletadas,
/// checkpoint ativo, IDs de moedas já pegas) e conversa com o SaveManager.
/// Escuta o GameEventChannel para reagir a moedas/checkpoint/vitória (Observer pattern).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado da fase atual (runtime)")]
    public int coinsCollected = 0;
    public bool hasPassedCheckpoint = false;
    public string activeCheckpointID = "";
    public Vector3 checkpointPosition;
    public List<string> collectedCoinIDs = new List<string>();

    // Guarda o próximo slot a ser carregado assim que a cena terminar de carregar.
    // Usado no fluxo: LoadGame(slot) -> SceneManager.LoadScene -> OnSceneLoaded -> posiciona player.
    private SaveData _pendingLoadData = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEventChannel.OnCoinCollected += HandleCoinCollected;
        GameEventChannel.OnCheckpointReached += HandleCheckpointReached;
        GameEventChannel.OnVictoryReached += HandleVictoryReached;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameEventChannel.OnCoinCollected -= HandleCoinCollected;
        GameEventChannel.OnCheckpointReached -= HandleCheckpointReached;
        GameEventChannel.OnVictoryReached -= HandleVictoryReached;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- Reações a eventos ---

    private void HandleCoinCollected(string coinID)
    {
        if (collectedCoinIDs.Contains(coinID)) return;

        collectedCoinIDs.Add(coinID);
        coinsCollected++;

        // Aqui você chama sua UI de moedas, ex:
        // CoinHUD.Instance.UpdateCoinText(coinsCollected);
    }

    private void HandleCheckpointReached(string checkpointID, Vector3 position)
    {
        hasPassedCheckpoint = true;
        activeCheckpointID = checkpointID;
        checkpointPosition = position;

        // Ao passar no checkpoint, as moedas coletadas ATÉ AGORA não devem reaparecer.
        // collectedCoinIDs já reflete isso, pois só guardamos moedas realmente coletadas.

        AutoSave();
    }

    private void HandleVictoryReached()
    {
        AutoSave();
        // Dispare aqui sua UI de vitória (moedas coletadas / total, aguardar input, etc.)
    }

    // --- Início de fase ---

    /// <summary>
    /// Chame isso ao iniciar uma fase do zero (Novo Jogo ou avançar de fase).
    /// Reseta o contador de moedas, conforme exigido no enunciado.
    /// </summary>
    public void ResetPhaseState()
    {
        coinsCollected = 0;
        hasPassedCheckpoint = false;
        activeCheckpointID = "";
        checkpointPosition = Vector3.zero;
        collectedCoinIDs.Clear();
    }

    // --- Integração com o SaveManager ---

    public void AutoSave()
    {
        SaveGameToSlot(0, replicateToAutosave: false);
    }

    public void SaveGameToSlot(int slot, bool replicateToAutosave = true)
    {
        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            coinsCollected = coinsCollected,
            collectedCoinIDs = new List<string>(collectedCoinIDs),
            hasPassedCheckpoint = hasPassedCheckpoint,
            activeCheckpointID = activeCheckpointID,
            isEmpty = false
        };
        data.SetCheckpointPosition(checkpointPosition);

        SaveManager.Instance.SaveGame(slot, data, replicateToAutosave);
    }

    /// <summary>
    /// Carrega o slot indicado. Força o recarregamento da cena mesmo se ela já estiver ativa.
    /// </summary>
    public void LoadGameFromSlot(int slot)
    {
        SaveData data = SaveManager.Instance.LoadGame(slot);

        if (data.isEmpty)
        {
            Debug.LogWarning($"[GameManager] Slot {slot} está vazio.");
            return;
        }

        // Slot manual carregado -> replica no autosave (slot 0), conforme enunciado
        if (slot != 0)
        {
            SaveManager.Instance.ReplicateSlotToAutosave(slot);
        }

        _pendingLoadData = data;

        // Força reload mesmo se a cena já estiver carregada
        SceneManager.LoadScene(data.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_pendingLoadData == null) return;

        SaveData data = _pendingLoadData;
        _pendingLoadData = null;

        coinsCollected = data.coinsCollected;
        hasPassedCheckpoint = data.hasPassedCheckpoint;
        activeCheckpointID = data.activeCheckpointID;
        checkpointPosition = data.GetCheckpointPosition();
        collectedCoinIDs = new List<string>(data.collectedCoinIDs);

        // Posiciona o player e esconde moedas já coletadas.
        // PlayerController e as moedas devem escutar isto ou o GameManager pode buscá-los diretamente:
        PlaceOrHidePostLoad();
    }

    private void PlaceOrHidePostLoad()
    {
        // Posiciona o player no checkpoint (se houver) ou deixa no spawn padrão da cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && hasPassedCheckpoint)
        {
            player.transform.position = checkpointPosition;
        }

        // Esconde moedas cujo ID já está em collectedCoinIDs
        Coin[] coinsInScene = FindObjectsByType<Coin>(FindObjectsSortMode.None);
        foreach (Coin coin in coinsInScene)
        {
            if (collectedCoinIDs.Contains(coin.coinID))
            {
                coin.gameObject.SetActive(false);
            }
        }
    }

    public bool AutosaveExists()
    {
        return SaveManager.Instance.HasSave(0);
    }
}
