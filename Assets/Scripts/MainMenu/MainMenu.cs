using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private string _gameSceneName = "Scene1";

    public static bool ShouldLoadSave { get; set; } = false;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.menuMusic);

        UpdateContinueButton(); // <- controlla se esiste un salvataggio e abilita/disabilita il pulsante Continue
    }

    private void UpdateContinueButton()
    {
        if (_continueButton != null)
        {
            bool saveExists = SaveSystem.SaveFileExists();
            _continueButton.interactable = saveExists;

            // opzionale: cambia anche l'opacità visiva del bottone
            var colors = _continueButton.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f); // bianco semi-trasparente
            _continueButton.colors = colors;

            Debug.Log(saveExists ? "Salvataggio trovato - Continue attivo" : "Nessun salvataggio - Continue disabilitato");
        }
        else
        {
            Debug.LogWarning("Continue Button non assegnato nell'Inspector!");
        }
    }

    public void OnNewGameClicked() // <- chiamato dal pulsante "New Game"
    {
        //// Cancella il salvataggio esistente per iniziare da zero
        //string path = Application.persistentDataPath + "/save.json";
        //if (System.IO.File.Exists(path))
        //{
        //    System.IO.File.Delete(path);
        //    Debug.Log("Salvataggio precedente eliminato");
        //}

        ShouldLoadSave = false;
        SceneManager.LoadScene(_gameSceneName);
        AudioManager.Instance.StopBGM();
    }

    public void OnContinueClicked() // <- chiamato dal pulsante "Continue"
    {
        if (SaveSystem.SaveFileExists())
        {
            ShouldLoadSave = true;
            SceneManager.LoadScene("Scene1");
        }
        else
        {
            Debug.LogWarning("Tentativo di continuare ma non esiste un salvataggio!");
        }
    }

    public void OnExitClicked() // <- chiamato dal pulsante "Exit"
    {
        Debug.Log("Uscita dal gioco");
        Application.Quit();
    }

    public void DeleteSaveFile() // <- metodo opzionale per cancellare il salvataggio (per testing)
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