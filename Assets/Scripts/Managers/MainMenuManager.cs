using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private string _gameSceneName = "Scene1"; // Nome della scena di gioco

    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.menuMusic);
        // Controlla se esiste un salvataggio e abilita/disabilita il pulsante Continue
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        if (_continueButton != null)
        {
            bool saveExists = SaveSystem.SaveFileExists();
            _continueButton.interactable = saveExists;

            // Opzionale: cambia anche l'opacità visiva del bottone
            var colors = _continueButton.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f); // Bianco semi-trasparente
            _continueButton.colors = colors;

            Debug.Log(saveExists ? "Salvataggio trovato - Continue attivo" : "Nessun salvataggio - Continue disabilitato");
        }
        else
        {
            Debug.LogWarning("Continue Button non assegnato nell'Inspector!");
        }
    }

    // Chiamato dal pulsante "New Game"
    public void OnNewGameClicked()
    {
        // Cancella il salvataggio esistente per iniziare da zero
        string path = Application.persistentDataPath + "/save.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Salvataggio precedente eliminato");
        }
        SceneManager.LoadScene(_gameSceneName);
        AudioManager.Instance.PlayBGM(AudioManager.Instance.gameMusic);
    }

    // Chiamato dal pulsante "Continue"
    public void OnContinueClicked()
    {
        if (SaveSystem.SaveFileExists())
        {
            SceneManager.LoadScene(_gameSceneName);
        }
        else
        {
            Debug.LogWarning("Tentativo di continuare ma non esiste un salvataggio!");
        }
    }

    // Metodo opzionale per cancellare il salvataggio (per testing)
    public void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Salvataggio eliminato");
            UpdateContinueButton();
        }
    }
}