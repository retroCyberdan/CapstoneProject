using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static SaveData currentSaveData;
    public static GameManager instance;

    [Header("Player Reference")]
    public GameObject player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Se esiste un salvataggio caricato, applica i dati
        if (currentSaveData != null)
        {
            LoadGameState();
        }
    }

    void LoadGameState()
    {
        if (player != null && currentSaveData != null)
        {
            // Applica posizione del giocatore
            player.transform.position = currentSaveData.playerPosition;

            // Applica salute (assumendo che il player abbia un componente Health)
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.SetHealth(currentSaveData.playerHealth);
            }

            // Carica altri dati di gioco come necessario
            Debug.Log($"Stato di gioco caricato: Livello {currentSaveData.currentLevel}");
        }
    }

    public void SaveCurrentGame(int slotIndex)
    {
        SaveData data = new SaveData
        {
            saveName = $"Salvataggio {slotIndex + 1}",
            saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            currentLevel = SceneManager.GetActiveScene().buildIndex,
            playerPosition = player != null ? player.transform.position : Vector3.zero,
            playerHealth = 100f // Sostituisci con il valore reale
        };

        // Aggiungi dati del player
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                data.playerHealth = health.currentHealth;
            }
        }

        SaveManager.SaveGame(data, slotIndex);
        Debug.Log("Gioco salvato con successo!");
    }

    public void ReturnToMainMenu()
    {
        currentSaveData = null;
        SceneManager.LoadScene("MainMenu");
    }
}

// Esempio di componente salute per il player
public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    void Die()
    {
        Debug.Log("Player è morto!");
        // Implementa logica di morte
    }
}