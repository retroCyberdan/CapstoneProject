using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string saveName;
    public string saveDate;
    public int currentLevel;
    public float playerHealth;
    public Vector3 playerPosition;
    public List<string> collectedItems;
    public Dictionary<string, bool> checkpoints;

    public SaveData()
    {
        saveName = "Nuova Partita";
        saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        currentLevel = 1;
        playerHealth = 100f;
        playerPosition = Vector3.zero;
        collectedItems = new List<string>();
        checkpoints = new Dictionary<string, bool>();
    }
}

public class SaveManager : MonoBehaviour
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "Saves");

    public static void SaveGame(SaveData data, int slotIndex)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string filePath = Path.Combine(savePath, $"save_{slotIndex}.json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Gioco salvato nello slot {slotIndex}");
    }

    public static SaveData LoadGame(int slotIndex)
    {
        string filePath = Path.Combine(savePath, $"save_{slotIndex}.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Gioco caricato dallo slot {slotIndex}");
            return data;
        }
        else
        {
            Debug.LogWarning($"Salvataggio {slotIndex} non trovato");
            return null;
        }
    }

    public static bool SaveExists(int slotIndex)
    {
        string filePath = Path.Combine(savePath, $"save_{slotIndex}.json");
        return File.Exists(filePath);
    }

    public static void DeleteSave(int slotIndex)
    {
        string filePath = Path.Combine(savePath, $"save_{slotIndex}.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Salvataggio {slotIndex} eliminato");
        }
    }

    public static List<SaveData> GetAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();

        if (!Directory.Exists(savePath))
        {
            return saves;
        }

        string[] files = Directory.GetFiles(savePath, "save_*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            saves.Add(data);
        }

        return saves;
    }
}