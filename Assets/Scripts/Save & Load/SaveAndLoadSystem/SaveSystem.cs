using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private Transform _player;
    [SerializeField] private HealthSystem _healthSystem;
    [SerializeField] private StressSystem _stressSystem;

    private PlayerSave _playerSave;
    private string _dataString;
    private string _path;

    private HashSet<string> _collectedItemIDs = new HashSet<string>(); // <- lista a runtime degli oggetti raccolti

    private bool _introVista = false; // <- flag per tracciare se l'intro è stata vista

    private HashSet<string> _activatedScriptIDs = new HashSet<string>(); // <- lista degli script attivati

    public static bool CheckIntroVistaFromFile() // <- metodo statico per controllare se l'intro è stata vista PRIMA che SaveSystem sia istanziato
    {
        string path = Application.persistentDataPath + "/save.json";

        if (!MainMenu.ShouldLoadSave || !File.Exists(path))
        {
            return false;
        }

        try
        {
            string jsonData = File.ReadAllText(path);
            PlayerSave save = JsonConvert.DeserializeObject<PlayerSave>(jsonData);
            return save.introVista;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante il controllo intro: " + e.Message);
            return false;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Importante: se vuoi che il SaveSystem persista tra scene diverse
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerSave = new PlayerSave();
        _path = Application.persistentDataPath + "/save.json";
        Debug.Log("Save path: " + _path);

        // carica subito gli ID degli oggetti raccolti e lo stato dell'intro se richiesto dal menu
        if (MainMenu.ShouldLoadSave)
        {
            LoadCollectedItemsOnly();
        }
    }

    private void Start()
    {
        if (MainMenu.ShouldLoadSave)
        {
            if (Load())
            {
                Debug.Log("<color=green>Salvataggio caricato automaticamente</color>");
            }
            MainMenu.ShouldLoadSave = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (Save()) Debug.Log("<color=green>Corretto Save</color>");
            else Debug.Log("<color=red>Errore Save</color>");
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            if (Load()) Debug.Log("<color=green>Corretto Load</color>");
            else Debug.Log("<color=red>Errore Load</color>");
        }
    }

    public static bool SaveFileExists()
    {
        string path = Application.persistentDataPath + "/save.json";
        return File.Exists(path);
    }

    public bool Save()
    {
        if (_player == null)
        {
            Debug.LogError("Player reference is null!");
            return false;
        }

        float[] pos = new float[3];
        pos[0] = _player.position.x;
        pos[1] = _player.position.y;
        pos[2] = _player.position.z;

        float[] rot = new float[4];
        rot[0] = _player.rotation.x;
        rot[1] = _player.rotation.y;
        rot[2] = _player.rotation.z;
        rot[3] = _player.rotation.w;

        float currentHealth = _healthSystem != null ? _healthSystem.GetCurrentHealth() : 0f;
        float maxHealth = _healthSystem != null ? _healthSystem.GetMaxHealth() : 0f;
        float currentStress = _stressSystem != null ? _stressSystem.GetCurrentStress() : 0f;
        float maxStress = _stressSystem != null ? _stressSystem.GetMaxStress() : 0f;

        List<string> collectedIDs = new List<string>(_collectedItemIDs); // <- crea la lista degli ID degli items raccolti

        List<string> activatedIDs = new List<string>(_activatedScriptIDs); // <- crea la lista degli ID degli scripts attivati

        float masterVol = PlayerPrefs.GetFloat(PlayerPrefsKeys.Volume, 0f);
        float bgmVol = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGMVolume, 0f);
        float sfxVol = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFXVolume, 0f);

        _playerSave = new PlayerSave(pos, rot, currentHealth, maxHealth, currentStress, maxStress, collectedIDs, _introVista, activatedIDs, PauseMenu.GameIsPaused, masterVol, bgmVol, sfxVol);

        _dataString = JsonConvert.SerializeObject(_playerSave, Formatting.Indented);

        try
        {
            File.WriteAllText(_path, _dataString);
            Debug.Log($"Save completato: {_path} - Oggetti salvati: {collectedIDs.Count} - Intro vista: {_introVista} - Script attivati: {activatedIDs.Count} - Paused: {PauseMenu.GameIsPaused}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante il salvataggio: " + e.Message);
            return false;
        }
    }

    public bool Load()
    {
        try
        {
            if (File.Exists(_path))
            {
                _dataString = File.ReadAllText(_path);
                _playerSave = JsonConvert.DeserializeObject<PlayerSave>(_dataString);

                if (_player != null)
                {
                    _player.transform.position = new Vector3(_playerSave.position[0], _playerSave.position[1], _playerSave.position[2]);
                    _player.transform.rotation = new Quaternion(_playerSave.rotation[0], _playerSave.rotation[1], _playerSave.rotation[2], _playerSave.rotation[3]);
                }

                if (_healthSystem != null)
                {
                    _healthSystem.SetMaxHealth(_playerSave.maxHealth);
                    _healthSystem.SetHealth(_playerSave.currentHealth);
                }

                if (_stressSystem != null)
                {
                    _stressSystem.SetMaxStress(_playerSave.maxStress);
                    _stressSystem.SetStress(_playerSave.currentStress);
                }

                // ripristina gli oggetti raccolti
                _collectedItemIDs.Clear();
                if (_playerSave.collectedItemIDs != null)
                {
                    foreach (string id in _playerSave.collectedItemIDs)
                    {
                        _collectedItemIDs.Add(id);
                    }
                }

                // ripristina lo stato dell'intro
                _introVista = _playerSave.introVista;

                // ripristina gli script attivati
                _activatedScriptIDs.Clear();
                if (_playerSave.activatedScriptIDs != null)
                {
                    foreach (string id in _playerSave.activatedScriptIDs)
                    {
                        _activatedScriptIDs.Add(id);
                    }
                }

                // ripristina lo stato del menu di pausa
                PauseMenu.GameIsPaused = _playerSave.gameIsPaused;
                Time.timeScale = _playerSave.gameIsPaused ? 0f : 1f;

                // riproduce automaticamente la musica del gioco al caricamento
                if (AudioManager.Instance != null)
                {
                    PlayerPrefs.SetFloat(PlayerPrefsKeys.Volume, _playerSave.masterVolume);
                    PlayerPrefs.SetFloat(PlayerPrefsKeys.BGMVolume, _playerSave.bgmVolume);
                    PlayerPrefs.SetFloat(PlayerPrefsKeys.SFXVolume, _playerSave.sfxVolume);
                    PlayerPrefs.Save();

                    AudioManager.Instance.UpdateVolumeFromPlayerPrefs();

                    AudioManager.Instance.PlayGameMusic();

                    // forza un aggiornamento immediato della musica in base a cosa è presente nella scena
                    EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
                    if (spawner != null) spawner.Invoke(nameof(spawner.HandleDynamicMusic), 0.1f);
                }

                // ripristina l'inventario
                if (InventoryManager.Instance != null) InventoryManager.Instance.LoadInventory(_playerSave.collectedItemIDs);

                Debug.Log($"Load completato - Oggetti caricati: {_collectedItemIDs.Count} - Intro vista: {_introVista} - Script attivati: {_activatedScriptIDs.Count} - Paused: {PauseMenu.GameIsPaused}");
                return true;
            }
            else
            {
                Debug.LogWarning("File di salvataggio non trovato: " + _path);
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante il caricamento: " + e.Message);
            return false;
        }
    }

    public bool DeleteSave() // <- per cancellare il file di salvataggio
    {
        try
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
                _collectedItemIDs.Clear();
                _introVista = false;
                _activatedScriptIDs.Clear();
                PauseMenu.GameIsPaused = false;
                Time.timeScale = 1f;
                Debug.Log("Salvataggio eliminato");
                return true;
            }
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante l'eliminazione: " + e.Message);
            return false;
        }
    }

    public void RegisterCollectedItem(string itemID) // <- registra un oggetto come raccolto
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            _collectedItemIDs.Add(itemID);
            Debug.Log($"Oggetto registrato come raccolto: {itemID}");
        }
    }

    public bool IsItemCollected(string itemID) // <- controlla se un oggetto è stato raccolto
    {
        return _collectedItemIDs.Contains(itemID);
    }

    private void LoadCollectedItemsOnly() // <- carica solo gli ID degli oggetti raccolti e lo stato dell'intro (per ItemsTrigger e IntroManager in Awake)
    {
        try
        {
            if (File.Exists(_path))
            {
                _dataString = File.ReadAllText(_path);
                _playerSave = JsonConvert.DeserializeObject<PlayerSave>(_dataString);

                _collectedItemIDs.Clear();
                if (_playerSave.collectedItemIDs != null)
                {
                    foreach (string id in _playerSave.collectedItemIDs)
                    {
                        _collectedItemIDs.Add(id);
                    }
                }

                _introVista = _playerSave.introVista;

                _activatedScriptIDs.Clear();
                if (_playerSave.activatedScriptIDs != null)
                {
                    foreach (string id in _playerSave.activatedScriptIDs)
                    {
                        _activatedScriptIDs.Add(id);
                    }
                }

                Debug.Log($"Pre-caricati {_collectedItemIDs.Count} ID oggetti raccolti - Intro già vista: {_introVista} - Script attivati: {_activatedScriptIDs.Count}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante il pre-caricamento oggetti: " + e.Message);
        }
    }

    public void SetIntroAsViewed() // <- segna l'intro come vista
    {
        _introVista = true;
        Debug.Log("Intro segnata come vista");
    }

    public bool IsIntroViewed() // <- controlla se l'intro è già stata vista
    {
        return _introVista;
    }

    public void RegisterActivatedScript(string scriptID) // <- registra uno script come attivato
    {
        if (!string.IsNullOrEmpty(scriptID))
        {
            _activatedScriptIDs.Add(scriptID);
            Debug.Log($"Script registrato come attivato: {scriptID}");
        }
    }

    public bool IsScriptActivated(string scriptID) // <- controlla se uno script è stato attivato
    {
        return _activatedScriptIDs.Contains(scriptID);
    }
}