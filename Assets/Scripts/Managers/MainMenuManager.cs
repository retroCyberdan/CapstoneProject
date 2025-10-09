using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainMenuCanvas;
    public GameObject loadGameCanvas;
    public GameObject optionsCanvas;
    public GameObject creditsCanvas;

    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioClip buttonClickSound;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        loadGameCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void NewGame()
    {
        PlayClickSound();
        // Carica la scena del gioco (cambia "GameScene" con il nome della tua scena)
        SceneManager.LoadScene("Scene1");
    }

    public void ShowLoadGame()
    {
        PlayClickSound();
        mainMenuCanvas.SetActive(false);
        loadGameCanvas.SetActive(true);
    }

    public void ShowOptions()
    {
        PlayClickSound();
        mainMenuCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void ShowCredits()
    {
        PlayClickSound();
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        PlayClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void PlayClickSound()
    {
        if (menuAudioSource != null && buttonClickSound != null)
        {
            menuAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}