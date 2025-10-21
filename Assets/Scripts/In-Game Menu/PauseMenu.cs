using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    [Header("Altri Canvas da controllare")]
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject optionsCanvas;

    [Header("Audio Settings")]
    public AudioClip menuOpenSound;
    public AudioClip menuCloseSound;
    [Range(0f, 1f)] public float menuSoundVolume = 0.5f;

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                // Se il gioco è in pausa, controlla se ci sono altri canvas aperti
                bool otherCanvasOpen = IsAnyOtherCanvasOpen();

                if (!otherCanvasOpen)
                {
                    // Chiudi solo se nessun altro canvas specifico è aperto
                    Resume();
                }
                // Se altri canvas sono aperti, ESC non fa nulla
            }
            else
            {
                // Apri il pause menu (non importa quali canvas sono aperti)
                Pause();
            }
        }
    }

    bool IsAnyOtherCanvasOpen()
    {
        // Controlla se l'inventario è aperto
        if (inventoryCanvas != null && inventoryCanvas.activeSelf)
            return true;

        // Controlla se le opzioni sono aperte
        if (optionsCanvas != null && optionsCanvas.activeSelf)
            return true;

        // Controlla se ItemsManager è aperto
        if (ItemsManager.Instance != null && ItemsManager.Instance.canvasGroup.gameObject.activeSelf)
            return true;

        return false;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (menuCloseSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShot(menuCloseSound, transform.position, menuSoundVolume);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        if (menuOpenSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShot(menuOpenSound, transform.position, menuSoundVolume);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}