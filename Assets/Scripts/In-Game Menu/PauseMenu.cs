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

    [Header("Camera Controller")]
    [SerializeField] ThirdCameraController cameraController;

    void Start()
    {
        // sincronizza lo stato del menu con GameIsPaused (importante per il caricamento)
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(GameIsPaused);

            // si assicura che il cursore sia nello stato corretto
            if (cameraController != null)
            {
                if (GameIsPaused)
                {
                    cameraController.ShowCursor();
                }
                else
                {
                    cameraController.HideCursor();
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                bool otherCanvasOpen = IsAnyOtherCanvasOpen(); // <- se il gioco è in pausa, controlla se ci sono altri canvas aperti

                if (!otherCanvasOpen) Resume(); // <- chiudi solo se nessun altro canvas specifico è aperto
                // se altri canvas sono aperti, ESC non fa nulla
            }
            else Pause(); // <- apri il pause menu (non importa quali canvas sono aperti)
        }
    }

    bool IsAnyOtherCanvasOpen()
    {
        if (inventoryCanvas != null && inventoryCanvas.activeSelf) return true; // <- controlla se l'inventario è aperto

        if (optionsCanvas != null && optionsCanvas.activeSelf) return true; // <- controlla se le opzioni sono aperte

        if (ItemsUiManager.Instance != null && ItemsUiManager.Instance.canvasGroup.gameObject.activeSelf) return true; // <- controlla se ItemsManager è aperto


        return false;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (cameraController != null) cameraController.HideCursor(); // <- nascondi il cursore quando riprendi il gioco

        if (menuCloseSound != null && AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(menuCloseSound, transform.position, menuSoundVolume);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        if (cameraController != null) cameraController.ShowCursor(); // <- mostra il cursore quando apri il menu di pausa

        if (menuOpenSound != null && AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(menuOpenSound, transform.position, menuSoundVolume);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false; // <- resetta lo stato quando torni al menu principale
        SceneManager.LoadScene("MainMenu");
    }
}