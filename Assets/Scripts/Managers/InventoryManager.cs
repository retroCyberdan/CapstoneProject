using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] Transform gridContainer;
    [SerializeField] GameObject inventorySlotPrefab;

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