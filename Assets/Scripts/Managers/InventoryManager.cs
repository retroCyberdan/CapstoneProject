using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] Transform gridContainer;
    [SerializeField] GameObject inventorySlotPrefab;

    [Header("Database Items")]
    [Tooltip("Lista di TUTTI gli ScriptableObject items del gioco per il caricamento")]
    [SerializeField] List<SO_Items> allItemsDatabase = new List<SO_Items>();

    List<SO_Items> collectedItems = new List<SO_Items>();

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

        Debug.Log($"Aggiunto {item.name} all'inventario. Totale items: {collectedItems.Count}");
    }

    void CreateInventorySlot(SO_Items item)
    {
        GameObject slotObj = Instantiate(inventorySlotPrefab, gridContainer);

        InventorySlot slot = slotObj.GetComponent<InventorySlot>();
        if (slot != null)
        {
            slot.Setup(item);
        }
        else
        {
            Debug.LogError("Il prefab InventorySlot non ha il componente InventorySlot!");
        }
    }

    // Carica l'inventario dal salvataggio
    public void LoadInventory(List<string> itemIDs)
    {
        // Pulisci l'inventario corrente
        ClearInventory();

        if (itemIDs == null || itemIDs.Count == 0)
        {
            Debug.Log("Nessun oggetto da caricare nell'inventario");
            return;
        }

        // Per ogni ID, trova il corrispondente ScriptableObject e aggiungilo
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

    // Trova un item per ID nel database
    SO_Items FindItemByID(string itemID)
    {
        foreach (SO_Items item in allItemsDatabase)
        {
            if (item != null && item.itemID == itemID)
            {
                return item;
            }
        }
        return null;
    }

    // Pulisce l'inventario
    void ClearInventory()
    {
        collectedItems.Clear();

        // Rimuovi tutti gli slot UI
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
}