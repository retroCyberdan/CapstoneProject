using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] Transform gridContainer;
    [SerializeField] GameObject inventorySlotPrefab;

    [Header("Database of Items to Load")]
    [Tooltip("Lista di TUTTI gli ScriptableObject items del gioco per il caricamento")]
    [SerializeField] List<SO_Items> allItemsToLoad = new List<SO_Items>();

    List<SO_Items> collectedItems = new List<SO_Items>();
    Dictionary<SO_Items, GameObject> itemSlots = new Dictionary<SO_Items, GameObject>(); // Traccia gli slot creati

    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(SO_Items item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativo di aggiungere un item null all'inventario!");
            return;
        }

        collectedItems.Add(item);
        CreateInventorySlot(item);

        if (SaveSystem.Instance != null && !string.IsNullOrEmpty(item.itemID)) SaveSystem.Instance.RegisterCollectedItem(item.itemID); // <- registra automaticamente nel SaveSystem

        Debug.Log($"Aggiunto {item.name} all'inventario. Totale items: {collectedItems.Count}");
    }

    void CreateInventorySlot(SO_Items item)
    {
        GameObject slotObj = Instantiate(inventorySlotPrefab, gridContainer);

        InventorySlot slot = slotObj.GetComponent<InventorySlot>();
        if (slot != null)
        {
            slot.Setup(item);
            // salva il riferimento slot-item nel dizionario
            itemSlots[item] = slotObj;
        }
        else
        {
            Debug.LogError("Il prefab InventorySlot non ha il componente InventorySlot!");
        }
    }

    public void LoadInventory(List<string> itemIDs) // <- carica l'inventario dal salvataggio
    {
        ClearInventory(); // <- pulisci l'inventario corrente

        if (itemIDs == null || itemIDs.Count == 0)
        {
            Debug.Log("Nessun oggetto da caricare nell'inventario");
            return;
        }

        // per ogni ID, trova il corrispondente ScriptableObject e lo aggiunge
        foreach (string id in itemIDs)
        {
            SO_Items item = FindItemByID(id);
            if (item != null)
            {
                collectedItems.Add(item);
                CreateInventorySlot(item);
            }
            else
            {
                Debug.LogWarning($"Impossibile trovare l'item con ID: {id}");
            }
        }

        Debug.Log($"Inventario caricato: {collectedItems.Count} oggetti");
    }

    SO_Items FindItemByID(string itemID) // <- trova un item per ID nel database
    {
        foreach (SO_Items item in allItemsToLoad)
        {
            if (item != null && item.itemID == itemID)
            {
                return item;
            }
        }
        return null;
    }

    void ClearInventory() // <- pulisce l'inventario
    {
        collectedItems.Clear();
        itemSlots.Clear();

        // rimuove tutti gli slot UI
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public bool HasItem(SO_Items item)
    {
        return collectedItems.Contains(item);
    }

    public int GetItemCount()
    {
        return collectedItems.Count;
    }

    public List<SO_Items> GetAllItems()
    {
        return new List<SO_Items>(collectedItems);
    }

    public void RemoveItem(SO_Items item) // <- rimuove un item dall'inventario (usato da INK)
    {
        if (collectedItems.Contains(item))
        {
            collectedItems.Remove(item);

            // rimuove anche lo slot visuale
            if (itemSlots.ContainsKey(item))
            {
                Destroy(itemSlots[item]);
                itemSlots.Remove(item);
            }

            Debug.Log($"Rimosso {item.name} dall'inventario.");
        }
        else
        {
            Debug.LogWarning($"Tentativo di rimuovere {item.name} ma non è nell'inventario!");
        }
    }

    public SO_Items GetItemByID(string itemID) // <- trova un item per ID negli oggetti raccolti (usato da INK)
    {
        foreach (SO_Items item in collectedItems)
        {
            if (item != null && item.itemID == itemID)
            {
                return item;
            }
        }
        return null;
    }
}