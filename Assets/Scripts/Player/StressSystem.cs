using System;
using UnityEngine;

public class StressSystem : MonoBehaviour
{
    [Header("Stress Settings")]
    [SerializeField] private float _maxStress = 100f;
    [SerializeField] private float _currentStress = 0f;
    [SerializeField] private float _stressPerEnemy = 5f; // <- stress aggiunto per ogni nemico attivo
    [SerializeField] private float _stressDecayRate = 2f; // <- riduzione stress al secondo
    [SerializeField] private int _enemyToGetStressed = 3; // <- soglia di nemici: sopra aumenta stress, sotto diminuisce

    [Header("Stress Effects Settings")]
    [SerializeField] private GameObject _stressedCanvas; // <- canvas da mostrare quando stressato
    [SerializeField] private AudioClip _playerStressedSound; // <- audio da riprodurre quando stressato
    [SerializeField] private float _stressReliefThreshold = 50f; // <- soglia per tornare alla normalità (default: metà)

    [Header("References")]
    [SerializeField] private UI_StressBar _uiStressBar;
    [SerializeField] private PoolManager _poolManager;
    [SerializeField] private PlayerVisionAI _playerVision;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _playerAnimator;

    // Eventi per notificare cambiamenti di stress
    public event Action<float, float> OnStressChanged; // currentStress, maxStress
    public event Action OnStressed; // <- quando lo stress raggiunge il massimo
    public event Action OnStressRelieved; // <- quando lo stress torna sotto il massimo

    private bool _isStressed = false;
    private int _previousActiveEnemies = 0;
    private AudioSource _stressedAudioSource; // <- riferimento all'audio source dello stress

    // Getters
    public float GetCurrentStress() => _currentStress;
    public float GetMaxStress() => _maxStress;
    public float GetStressPercentage() => _currentStress / _maxStress;
    public bool IsStressed() => _isStressed;

    private void Start()
    {
        _currentStress = 0f;

        if (_uiStressBar != null)
        {
            _uiStressBar.maxStressValue = _maxStress;
            _uiStressBar.stressValue = _currentStress;
        }

        if (_poolManager == null)
        {
            _poolManager = PoolManager.Instance;
        }

        if (_playerVision == null)
        {
            _playerVision = GetComponent<PlayerVisionAI>();
        }

        if (_playerController == null)
        {
            _playerController = GetComponent<PlayerController>();
        }

        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();
        }

        // si assicura che il canvas sia disattivato all'inizio
        if (_stressedCanvas != null)
        {
            _stressedCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        //// debug controls - rimuovi in produzione
        //if (Input.GetKeyDown(KeyCode.K)) AddStress(10f);
        //if (Input.GetKeyDown(KeyCode.L)) ReduceStress(10f);

        UpdateStressBasedOnEnemies(); // <- calcola lo stress in base ai nemici attivi
    }

    private void UpdateStressBasedOnEnemies()
    {
        // conta solo i nemici nel campo visivo del player
        int activeEnemies = CountEnemiesInPlayerVision();

        if (activeEnemies >= _enemyToGetStressed)
        {
            // aumenta lo stress in base al numero di nemici
            float stressIncrease = activeEnemies * _stressPerEnemy * Time.deltaTime;
            AddStress(stressIncrease);
        }
        else
        {
            // riduce lo stress quando i nemici sono sotto la soglia
            ReduceStress(_stressDecayRate * Time.deltaTime);
        }

        _previousActiveEnemies = activeEnemies;
    }

    private int CountEnemiesInPlayerVision()
    {
        // se non c'è PlayerVisionAI, fallback al vecchio metodo
        if (_playerVision == null)
        {
            return CountActiveEnemies();
        }

        // conta solo i nemici visibili dal player
        return _playerVision.EnemiesInSightCount;
    }

    private int CountActiveEnemies()
    {
        if (_poolManager == null) return 0;

        int count = 0;

        // conta tutti gli oggetti con tag "Enemy" o "Villain" attivi
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] villains = GameObject.FindGameObjectsWithTag("Villain");

        foreach (var enemy in enemies)
        {
            if (enemy.activeInHierarchy) count++;
        }

        foreach (var villain in villains)
        {
            if (villain.activeInHierarchy) count++;
        }

        return count;
    }

    public void AddStress(float amount) // <- aggiunge Stress
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di stress non può essere negativo. Usa ReduceStress() per ridurre lo stress.");
            return;
        }

        _currentStress += amount;
        _currentStress = Mathf.Min(_currentStress, _maxStress);

        UpdateStressBar();
        OnStressChanged?.Invoke(_currentStress, _maxStress);

        // controlla se lo stress ha raggiunto il massimo
        if (!_isStressed && _currentStress >= _maxStress)
        {
            _isStressed = true;
            ActivateStressEffects();
            OnStressed?.Invoke();
            Debug.Log($"{gameObject.name} è completamente stressato!");
        }
    }

    public void ReduceStress(float amount) // <- riduce lo Stress
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di riduzione stress non può essere negativo. Usa AddStress() per aggiungere stress.");
            return;
        }

        bool wasStressed = _isStressed;

        _currentStress -= amount;
        _currentStress = Mathf.Max(_currentStress, 0f);

        UpdateStressBar();
        OnStressChanged?.Invoke(_currentStress, _maxStress);

        // controlla se lo stress è tornato sotto la soglia di rilassamento
        if (wasStressed && _currentStress <= _stressReliefThreshold)
        {
            _isStressed = false;
            DeactivateStressEffects();
            OnStressRelieved?.Invoke();
            Debug.Log($"{gameObject.name} si è calmato!");
        }
    }

    public void SetStress(float value) // <- imposta lo Stress a un valore specifico
    {
        bool wasStressed = _isStressed;

        _currentStress = Mathf.Clamp(value, 0f, _maxStress);

        UpdateStressBar();
        OnStressChanged?.Invoke(_currentStress, _maxStress);

        // controlla i cambiamenti di stato
        if (!wasStressed && _currentStress >= _maxStress)
        {
            _isStressed = true;
            ActivateStressEffects();
            OnStressed?.Invoke();
            Debug.Log($"{gameObject.name} è completamente stressato!");
        }
        else if (wasStressed && _currentStress <= _stressReliefThreshold)
        {
            _isStressed = false;
            DeactivateStressEffects();
            OnStressRelieved?.Invoke();
            Debug.Log($"{gameObject.name} si è calmato!");
        }
    }

    private void ActivateStressEffects() // <- attiva tutti gli effetti dello Stress
    {
        // mostra il canvas
        if (_stressedCanvas != null)
        {
            _stressedCanvas.SetActive(true);
        }

        // riproduce il suono di stress
        if (_playerStressedSound != null && AudioManager.Instance != null)
        {
            GameObject audioObject = new GameObject("StressedAudio");
            audioObject.transform.position = transform.position;
            audioObject.transform.SetParent(transform);

            _stressedAudioSource = audioObject.AddComponent<AudioSource>();
            _stressedAudioSource.clip = _playerStressedSound;
            _stressedAudioSource.loop = true;
            _stressedAudioSource.volume = 0.7f;
            _stressedAudioSource.Play();
        }

        // disabilita la corsa del player
        if (_playerController != null)
        {
            _playerController.canSprint = false;
        }

        // attiva la bool nell'animator
        if (_playerAnimator != null)
        {
            _playerAnimator.SetBool("isStressed", true);
        }
    }

    private void DeactivateStressEffects() // <- disattiva tutti gli effetti dello Stress
    {
        // nasconde il canvas
        if (_stressedCanvas != null)
        {
            _stressedCanvas.SetActive(false);
        }

        // ferma il suono di stress
        if (_stressedAudioSource != null)
        {
            _stressedAudioSource.Stop();
            Destroy(_stressedAudioSource.gameObject);
            _stressedAudioSource = null;
        }

        // riabilita la corsa del player
        if (_playerController != null)
        {
            _playerController.canSprint = true;
        }

        // disattiva la bool nell'animator
        if (_playerAnimator != null)
        {
            _playerAnimator.SetBool("isStressed", false);
        }
    }

    public void SetMaxStress(float value, bool resetStress = false) // <- imposta lo Stress massimo
    {
        _maxStress = Mathf.Max(value, 1f);

        if (resetStress)
        {
            _currentStress = 0f;
        }
        else
        {
            _currentStress = Mathf.Min(_currentStress, _maxStress);
        }

        if (_uiStressBar != null) _uiStressBar.maxStressValue = _maxStress;

        UpdateStressBar();
        OnStressChanged?.Invoke(_currentStress, _maxStress);
    }

    public void ResetStress() // <- azzera completamente lo stress
    {
        ReduceStress(_currentStress);
    }

    public void SetStressDecayRate(float rate) // <- imposta la velocità di riduzione dello Stress
    {
        _stressDecayRate = Mathf.Max(rate, 0f);
    }

    public void SetEnemyThreshold(int threshold) // <- imposta la soglia di nemici per aumentare/ridurre lo Stress
    {
        _enemyToGetStressed = Mathf.Max(threshold, 0);
    }

    public void SetStressPerEnemy(float stress) // <- imposta lo Stress aggiunto per ogni nemico attivo
    {
        _stressPerEnemy = Mathf.Max(stress, 0f);
    }

    private void UpdateStressBar()
    {
        if (_uiStressBar != null) _uiStressBar.stressValue = _currentStress;
    }

    private void OnDestroy()
    {
        // cleanup quando lo script viene distrutto
        if (_stressedAudioSource != null)
        {
            Destroy(_stressedAudioSource.gameObject);
        }
    }
}