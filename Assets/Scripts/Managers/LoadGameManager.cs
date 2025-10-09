using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadGameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject saveSlotPrefab;
    public Transform saveSlotContainer;
    public Button backButton;

    [Header("Confirmation Panel")]
    public GameObject confirmDeletePanel;
    public TextMeshProUGUI confirmDeleteText;
    private int slotToDelete = -1;

    void OnEnable()
    {
        RefreshSaveSlots();
    }

    public void RefreshSaveSlots()
    {
        // Pulisci slot esistenti
        foreach (Transform child in saveSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // Crea slot per salvataggi (ad esempio 5 slot)
        for (int i = 0; i < 5; i++)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            if (slotUI != null)
            {
                slotUI.Initialize(i, this);
            }
        }
    }

    public void LoadSave(int slotIndex)
    {
        SaveData data = SaveManager.LoadGame(slotIndex);

        if (data != null)
        {
            // Carica la scena del gioco e applica i dati salvati
            GameManager.currentSaveData = data;
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("Nessun salvataggio trovato in questo slot");
        }
    }

    public void ShowDeleteConfirmation(int slotIndex)
    {
        slotToDelete = slotIndex;
        confirmDeletePanel.SetActive(true);
        confirmDeleteText.text = $"Vuoi davvero eliminare il salvataggio dello slot {slotIndex + 1}?";
    }

    public void ConfirmDelete()
    {
        if (slotToDelete >= 0)
        {
            SaveManager.DeleteSave(slotToDelete);
            RefreshSaveSlots();
            slotToDelete = -1;
        }
        confirmDeletePanel.SetActive(false);
    }

    public void CancelDelete()
    {
        slotToDelete = -1;
        confirmDeletePanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<MainMenuManager>().ShowMainMenu();
    }
}

// Classe per gestire la UI di ogni singolo slot
public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotNumberText;
    public TextMeshProUGUI saveNameText;
    public TextMeshProUGUI saveDateText;
    public TextMeshProUGUI levelText;
    public Button loadButton;
    public Button deleteButton;
    public GameObject emptySlotPanel;
    public GameObject filledSlotPanel;

    private int slotIndex;
    private LoadGameManager loadManager;

    public void Initialize(int index, LoadGameManager manager)
    {
        slotIndex = index;
        loadManager = manager;

        slotNumberText.text = $"SLOT {index + 1}";

        if (SaveManager.SaveExists(index))
        {
            SaveData data = SaveManager.LoadGame(index);
            ShowFilledSlot(data);
        }
        else
        {
            ShowEmptySlot();
        }

        loadButton.onClick.AddListener(() => loadManager.LoadSave(slotIndex));
        deleteButton.onClick.AddListener(() => loadManager.ShowDeleteConfirmation(slotIndex));
    }

    void ShowFilledSlot(SaveData data)
    {
        emptySlotPanel.SetActive(false);
        filledSlotPanel.SetActive(true);

        saveNameText.text = data.saveName;
        saveDateText.text = data.saveDate;
        levelText.text = $"Livello {data.currentLevel}";

        loadButton.interactable = true;
        deleteButton.interactable = true;
    }

    void ShowEmptySlot()
    {
        emptySlotPanel.SetActive(true);
        filledSlotPanel.SetActive(false);

        loadButton.interactable = false;
        deleteButton.interactable = false;
    }
}