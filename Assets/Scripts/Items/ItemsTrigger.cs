using UnityEngine;

public class ItemsTrigger : MonoBehaviour
{
    [SerializeField] Canvas popUpCanvas;
    [SerializeField] SO_Items itemData;
    [SerializeField] bool destroyAfterPickup = true; // Distrugge l'oggetto dopo la raccolta
    [SerializeField] bool addToInventory = true; // Aggiunge l'oggetto all'inventario

    bool isPlayerInRange;
    bool isItemCollected;

    private void Awake()
    {
        if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && !isItemCollected && Input.GetKeyDown(KeyCode.E))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        isItemCollected = true;

        if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);

        // Aggiungi all'inventario se richiesto
        if (addToInventory && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemData);
        }

        // Attiva la UI dell'oggetto
        ItemsManager.Instance.canvasGroup.gameObject.SetActive(true);
        ItemsManager.Instance.ShowItem(itemData);

        // Distruggi o disattiva l'oggetto
        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isItemCollected)
        {
            isPlayerInRange = true;
            if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);
        }
    }
}