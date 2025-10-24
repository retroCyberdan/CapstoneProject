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

    // Lista runtime degli oggetti raccolti
    private HashSet<string> _collectedItemIDs = new HashSet<string>();

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

        // Carica subito gli ID degli oggetti raccolti se richiesto dal menu
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

    // Carica solo gli ID degli oggetti raccolti (per ItemsTrigger in Awake)
    private void LoadCollectedItemsOnly()
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
                Debug.Log($"Pre-caricati {_collectedItemIDs.Count} ID oggetti raccolti");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore durante il pre-caricamento oggetti: " + e.Message);
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

    // Registra un oggetto come raccolto
    public void RegisterCollectedItem(string itemID)
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            _collectedItemIDs.Add(itemID);
            Debug.Log($"Oggetto registrato come raccolto: {itemID}");
        }
    }

    // Controlla se un oggetto è stato raccolto
    public bool IsItemCollected(string itemID)
    {
        return _collectedItemIDs.Contains(itemID);
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

        // Crea la lista degli ID raccolti
        List<string> collectedIDs = new List<string>(_collectedItemIDs);

        _playerSave = new PlayerSave(pos, rot, currentHealth, maxHealth, currentStress, maxStress, collectedIDs);
        _dataString = JsonConvert.SerializeObject(_playerSave, Formatting.Indented);

        try
        {
            File.WriteAllText(_path, _dataString);
            Debug.Log($"Save completato: {_path} - Oggetti salvati: {collectedIDs.Count}");
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

                // Ripristina gli oggetti raccolti
                _collectedItemIDs.Clear();
                if (_playerSave.collectedItemIDs != null)
                {
                    foreach (string id in _playerSave.collectedItemIDs)
                    {
                        _collectedItemIDs.Add(id);
                    }
                }

                // Ripristina l'inventario
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.LoadInventory(_playerSave.collectedItemIDs);
                }

                Debug.Log($"Load completato - Oggetti caricati: {_collectedItemIDs.Count}");
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

    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
                _collectedItemIDs.Clear();
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
}