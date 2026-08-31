using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Singleton responsável por persistir os dados de save em disco.
/// Cada slot é um arquivo individual em Application.persistentDataPath.
/// O conteúdo é serializado para JSON e encriptado (XOR + Base64) antes de ser salvo.
/// Slot 0 é sempre o autosave.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const int TOTAL_SLOTS = 4; // slot 0 = autosave, 1..3 = slots manuais
    private const string FILE_PREFIX = "save";
    private const string FILE_EXTENSION = ".dat";

    // Chave simples de encriptação XOR. Troque por algo próprio do seu projeto se quiser.
    private const string ENCRYPTION_KEY = "IFRN-GameDev-2026";

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

    private string GetFilePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, FILE_PREFIX + slot + FILE_EXTENSION);
    }

    /// <summary>
    /// Salva o SaveData no slot indicado. Se replicateToAutosave for true (padrão),
    /// também grava uma cópia no slot 0, conforme exigido pelo enunciado.
    /// </summary>
    public void SaveGame(int slot, SaveData data, bool replicateToAutosave = true)
    {
        data.isEmpty = false;

        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json);

        File.WriteAllText(GetFilePath(slot), encrypted);

        if (replicateToAutosave && slot != 0)
        {
            File.WriteAllText(GetFilePath(0), encrypted);
        }

        Debug.Log($"[SaveManager] Jogo salvo no slot {slot}" + (replicateToAutosave && slot != 0 ? " (replicado no autosave)" : ""));
    }

    /// <summary>
    /// Carrega o SaveData do slot indicado. Retorna um SaveData vazio (isEmpty = true) se não existir.
    /// </summary>
    public SaveData LoadGame(int slot)
    {
        string path = GetFilePath(slot);

        if (!File.Exists(path))
        {
            return new SaveData { isEmpty = true };
        }

        try
        {
            string encrypted = File.ReadAllText(path);
            string json = Decrypt(encrypted);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Falha ao carregar slot {slot}: {e.Message}");
            return new SaveData { isEmpty = true };
        }
    }

    /// <summary>
    /// Copia o conteúdo bruto (já encriptado) de um slot para o slot 0 (autosave).
    /// Usado quando o jogador carrega um slot manual: o autosave deve refletir aquele save.
    /// </summary>
    public void ReplicateSlotToAutosave(int slot)
    {
        if (slot == 0) return;

        string path = GetFilePath(slot);
        if (!File.Exists(path)) return;

        string content = File.ReadAllText(path);
        File.WriteAllText(GetFilePath(0), content);
    }

    public bool HasSave(int slot)
    {
        return File.Exists(GetFilePath(slot));
    }

    public void DeleteSave(int slot)
    {
        string path = GetFilePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    // --- Encriptação simples XOR + Base64 ---
    // Suficiente para impedir edição casual do save em um editor de texto.

    private string Encrypt(string plainText)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            char key = ENCRYPTION_KEY[i % ENCRYPTION_KEY.Length];
            result.Append((char)(c ^ key));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(result.ToString());
        return Convert.ToBase64String(bytes);
    }

    private string Decrypt(string encryptedText)
    {
        byte[] bytes = Convert.FromBase64String(encryptedText);
        string xored = Encoding.UTF8.GetString(bytes);

        StringBuilder result = new StringBuilder();
        for (int i = 0; i < xored.Length; i++)
        {
            char c = xored[i];
            char key = ENCRYPTION_KEY[i % ENCRYPTION_KEY.Length];
            result.Append((char)(c ^ key));
        }

        return result.ToString();
    }
}
