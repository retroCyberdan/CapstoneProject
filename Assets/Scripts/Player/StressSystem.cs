using System;
using UnityEngine;

public class StressSystem : MonoBehaviour
{
    [Header("Stress Settings")]
    [SerializeField] private float maxStress = 100f;
    [SerializeField] private float currentStress = 0f;
    [SerializeField] private float stressPerEnemy = 5f; // <- stress aggiunto per ogni nemico attivo
    [SerializeField] private float stressDecayRate = 2f; // <- riduzione stress al secondo
    [SerializeField] private int enemyToGetStressed = 3; // <- soglia di nemici: sopra aumenta stress, sotto diminuisce

    [Header("References")]
    [SerializeField] private UI_StressBar uiStressBar;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private PlayerVisionAI playerVision;

    // Eventi per notificare cambiamenti di stress
    public event Action<float, float> OnStressChanged; // currentStress, maxStress
    public event Action OnStressed; // <- quando lo stress raggiunge il massimo
    public event Action OnStressRelieved; // <- quando lo stress torna sotto il massimo

    private bool isStressed = false;
    private int previousActiveEnemies = 0;

    // Getters
    public float GetCurrentStress() => currentStress;
    public float GetMaxStress() => maxStress;
    public float GetStressPercentage() => currentStress / maxStress;
    public bool IsStressed() => isStressed;

    private void Start()
    {
        currentStress = 0f;

        if (uiStressBar != null)
        {
            uiStressBar.maxStressValue = maxStress;
            uiStressBar.stressValue = currentStress;
        }

        if (poolManager == null)
        {
            poolManager = PoolManager.Instance;
        }

        if (playerVision == null)
        {
            playerVision = GetComponent<PlayerVisionAI>();
        }
    }

    private void Update()
    {
        // Debug controls - rimuovi in produzione
        if (Input.GetKeyDown(KeyCode.K)) AddStress(10f);
        if (Input.GetKeyDown(KeyCode.L)) ReduceStress(10f);

        // Calcola lo stress in base ai nemici attivi
        UpdateStressBasedOnEnemies();
    }

    private void UpdateStressBasedOnEnemies()
    {
        // Conta solo i nemici nel campo visivo del player
        int activeEnemies = CountEnemiesInPlayerVision();

        if (activeEnemies >= enemyToGetStressed)
        {
            // Aumenta lo stress in base al numero di nemici
            float stressIncrease = activeEnemies * stressPerEnemy * Time.deltaTime;
            AddStress(stressIncrease);
        }
        else
        {
            // Riduce lo stress quando i nemici sono sotto la soglia
            ReduceStress(stressDecayRate * Time.deltaTime);
        }

        previousActiveEnemies = activeEnemies;
    }

    private int CountEnemiesInPlayerVision()
    {
        // Se non c'è PlayerVisionAI, fallback al vecchio metodo
        if (playerVision == null)
        {
            return CountActiveEnemies();
        }

        // Conta solo i nemici visibili dal player
        return playerVision.EnemiesInSightCount;
    }

    private int CountActiveEnemies()
    {
        if (poolManager == null) return 0;

        int count = 0;

        // Conta tutti gli oggetti con tag "Enemy" o "Villain" attivi
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

    /// <summary>
    /// Aggiunge stress
    /// </summary>
    public void AddStress(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di stress non può essere negativo. Usa ReduceStress() per ridurre lo stress.");
            return;
        }

        currentStress += amount;
        currentStress = Mathf.Min(currentStress, maxStress);

        UpdateStressBar();
        OnStressChanged?.Invoke(currentStress, maxStress);

        // Controlla se lo stress ha raggiunto il massimo
        if (!isStressed && currentStress >= maxStress)
        {
            isStressed = true;
            OnStressed?.Invoke();
            Debug.Log($"{gameObject.name} è completamente stressato!");
        }
    }

    /// <summary>
    /// Riduce lo stress
    /// </summary>
    public void ReduceStress(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di riduzione stress non può essere negativo. Usa AddStress() per aggiungere stress.");
            return;
        }

        bool wasStressed = isStressed;

        currentStress -= amount;
        currentStress = Mathf.Max(currentStress, 0f);

        UpdateStressBar();
        OnStressChanged?.Invoke(currentStress, maxStress);

        // Controlla se lo stress è tornato sotto il massimo
        if (wasStressed && currentStress < maxStress)
        {
            isStressed = false;
            OnStressRelieved?.Invoke();
            Debug.Log($"{gameObject.name} si è calmato!");
        }
    }

    /// <summary>
    /// Imposta lo stress a un valore specifico
    /// </summary>
    public void SetStress(float value)
    {
        bool wasStressed = isStressed;

        currentStress = Mathf.Clamp(value, 0f, maxStress);

        UpdateStressBar();
        OnStressChanged?.Invoke(currentStress, maxStress);

        // Controlla i cambiamenti di stato
        if (!wasStressed && currentStress >= maxStress)
        {
            isStressed = true;
            OnStressed?.Invoke();
            Debug.Log($"{gameObject.name} è completamente stressato!");
        }
        else if (wasStressed && currentStress < maxStress)
        {
            isStressed = false;
            OnStressRelieved?.Invoke();
            Debug.Log($"{gameObject.name} si è calmato!");
        }
    }

    /// <summary>
    /// Imposta lo stress massimo
    /// </summary>
    public void SetMaxStress(float value, bool resetStress = false)
    {
        maxStress = Mathf.Max(value, 1f);

        if (resetStress)
        {
            currentStress = 0f;
        }
        else
        {
            currentStress = Mathf.Min(currentStress, maxStress);
        }

        if (uiStressBar != null) uiStressBar.maxStressValue = maxStress;

        UpdateStressBar();
        OnStressChanged?.Invoke(currentStress, maxStress);
    }

    /// <summary>
    /// Azzera completamente lo stress
    /// </summary>
    public void ResetStress()
    {
        ReduceStress(currentStress);
    }

    /// <summary>
    /// Imposta la velocità di riduzione stress
    /// </summary>
    public void SetStressDecayRate(float rate)
    {
        stressDecayRate = Mathf.Max(rate, 0f);
    }

    /// <summary>
    /// Imposta la soglia di nemici per l'aumento/riduzione stress
    /// </summary>
    public void SetEnemyThreshold(int threshold)
    {
        enemyToGetStressed = Mathf.Max(threshold, 0);
    }

    /// <summary>
    /// Imposta lo stress aggiunto per nemico
    /// </summary>
    public void SetStressPerEnemy(float stress)
    {
        stressPerEnemy = Mathf.Max(stress, 0f);
    }

    private void UpdateStressBar()
    {
        if (uiStressBar != null) uiStressBar.stressValue = currentStress;
    }
}