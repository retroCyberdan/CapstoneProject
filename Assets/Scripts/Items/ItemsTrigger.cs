using UnityEngine;

public class ItemsTrigger : MonoBehaviour
{
    [SerializeField] Canvas popUpCanvas;
    [SerializeField] SO_Items itemData;
    [SerializeField] bool destroyAfterPickup = true;
    [SerializeField] bool addToInventory = true;

    bool isPlayerInRange;
    bool isItemCollected;

    private void Awake()
    {
        if (popUpCanvas != null) popUpCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Controlla se questo oggetto è già stato raccolto
        // Fatto in Start() per assicurarsi che SaveSystem abbia caricato i dati
        if (SaveSystem.Instance != null && itemData != null)
        {
            if (SaveSystem.Instance.IsItemCollected(itemData.itemID))
            {
                // Se già raccolto, disattiva/distruggi immediatamente
                if (destroyAfterPickup)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }
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

        // Registra l'oggetto come raccolto nel SaveSystem
        if (SaveSystem.Instance != null && itemData != null)
        {
            SaveSystem.Instance.RegisterCollectedItem(itemData.itemID);
        }

        // Attiva la UI dell'oggetto
        ItemsUiManager.Instance.canvasGroup.gameObject.SetActive(true);
        ItemsUiManager.Instance.ShowItem(itemData);

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