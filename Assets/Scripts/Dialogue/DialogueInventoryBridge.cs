using UnityEngine;
using Ink.Runtime;

public class DialogueInventoryBridge : MonoBehaviour
{
    [Header("Oggetti da controllare")]
    [SerializeField] SO_Items[] itemsToCheck;

    public static DialogueInventoryBridge Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void BindInventoryFunctionsToStory(Story story) // <- metodo da chiamare PRIMA di avviare un dialogo INK
    {
        if (story == null || InventoryManager.Instance == null)
        {
            Debug.LogWarning("Story o InventoryManager è null!");
            return;
        }

        // registra la funzione "hasItem" che INK può chiamare
        story.BindExternalFunction("hasItem", (string itemID) =>
        {
            SO_Items item = FindItemByID(itemID); // <- cerca l'oggetto per ID
            if (item != null)
            {
                bool hasIt = InventoryManager.Instance.HasItem(item);
                Debug.Log($"INK chiede se hai '{itemID}': {hasIt}");
                return hasIt;
            }
            Debug.LogWarning($"Oggetto con ID '{itemID}' non trovato nell'array itemsToCheck!");
            return false;
        });

        // registra la funzione "hasFlag" per controllare le flags globali
        story.BindExternalFunction("hasFlag", (string flagName) =>
        {
            if (DialogueGlobalFlagsManager.Instance != null)
            {
                bool hasIt = DialogueGlobalFlagsManager.Instance.HasFlag(flagName);
                Debug.Log($"INK chiede se flag '{flagName}' esiste: {hasIt}");
                return hasIt;
            }
            return false;
        });

        Debug.Log("Funzioni inventario collegate a INK!");
    }

    SO_Items FindItemByID(string itemID) // <- trova un oggetto per ID nell'array
    {
        foreach (SO_Items item in itemsToCheck)
        {
            if (item != null && item.itemID == itemID)
            {
                return item;
            }
        }
        return null;
    }

    public void UnbindInventoryFunctions(Story story) // <- rimuove il binding quando finisce il dialogo (opzionale ma consigliato!!)
    {
        if (story != null)
        {
            story.UnbindExternalFunction("hasItem");
        }
    }
}