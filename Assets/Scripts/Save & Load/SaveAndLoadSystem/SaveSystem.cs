using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private PlayerSave _playerSave;
    private string _dataString;
    private string _path;

    [SerializeField] private Transform _player;
    [SerializeField] private HealthSystem _healthSystem;
    [SerializeField] private StressSystem _stressSystem;

    private void Awake()
    {
        // Pattern Singleton
        if (Instance == null)
        {
            Instance = this;
            // Non serve DontDestroyOnLoad per questo caso
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerSave = new PlayerSave();
        _path = Application.persistentDataPath + "/save.json"; // Meglio usare persistentDataPath
        Debug.Log("Save path: " + _path);
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

    // Metodo statico per controllare se esiste un salvataggio (chiamabile da qualsiasi scena)
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

        _playerSave = new PlayerSave(pos, rot, currentHealth, maxHealth, currentStress, maxStress);
        _dataString = JsonConvert.SerializeObject(_playerSave, Formatting.Indented);

        try
        {
            File.WriteAllText(_path, _dataString);
            Debug.Log("Save completato: " + _path);
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

                Debug.Log("Load completato");
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

    // Metodo pubblico per cancellare il salvataggio (opzionale, utile per testing)
    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
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