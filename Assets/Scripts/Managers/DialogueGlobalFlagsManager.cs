using System.Collections.Generic;
using UnityEngine;

public class DialogueGlobalFlagsManager : MonoBehaviour
{
    public static DialogueGlobalFlagsManager Instance { get; private set; }

    Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake()
    {
        Instance = this;
    }

    public void SetFlag(string flagName, bool value) // <- setta una flag
    {
        flags[flagName] = value;
        Debug.Log($"Flag '{flagName}' settato a {value}");
    }

    public bool GetFlag(string flagName) // <- ottiene una flag (ritorna false se non esiste)
    {
        if (flags.ContainsKey(flagName))
        {
            return flags[flagName];
        }
        return false;
    }

    public bool HasFlag(string flagName) // <- controlla se una flag esiste ed è true
    {
        return flags.ContainsKey(flagName) && flags[flagName];
    }

    public Dictionary<string, bool> GetAllFlags() // <- per il salvataggio
    {
        return new Dictionary<string, bool>(flags);
    }

    public void LoadFlags(Dictionary<string, bool> loadedFlags) // <- per il caricamento
    {
        if (loadedFlags != null)
        {
            flags = new Dictionary<string, bool>(loadedFlags);
            Debug.Log($"Caricati {flags.Count} flags");
        }
    }

    public void ClearFlags() // <- reset di tutte le flags
    {
        flags.Clear();
    }
}