using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("References")]
    [SerializeField] private HealthBarUI healthBarUI;

    // Eventi per notificare cambiamenti di salute
    public event Action<float, float> OnHealthChanged; // currentHealth, maxHealth
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;

        // Inizializza la health bar se presente
        if (healthBarUI != null)
        {
            healthBarUI.maxHealthValue = maxHealth;
            healthBarUI.healthValue = currentHealth;
        }
    }

    private void Update()
    {
        // Debug controls - rimuovi in produzione
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Heal(15f);
        }
    }

    /// <summary>
    /// Infligge danno al sistema di salute
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning("Il danno non può essere negativo. Usa Heal() per recuperare vita.");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Recupera vita
    /// </summary>
    public void Heal(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di guarigione non può essere negativo. Usa TakeDamage() per infliggere danno.");
            return;
        }

        if (currentHealth <= 0)
        {
            Debug.LogWarning("Non è possibile guarire un'entità morta.");
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Imposta la salute a un valore specifico
    /// </summary>
    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Imposta la salute massima e opzionalmente recupera completamente
    /// </summary>
    public void SetMaxHealth(float value, bool healToMax = false)
    {
        maxHealth = Mathf.Max(value, 1f);

        if (healToMax)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        if (healthBarUI != null)
        {
            healthBarUI.maxHealthValue = maxHealth;
        }

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Recupera completamente la salute
    /// </summary>
    public void HealToMax()
    {
        Heal(maxHealth - currentHealth);
    }

    private void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.healthValue = currentHealth;
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} è morto!");
        // Aggiungi qui la logica di morte (animazioni, disattivazione, ecc.)
    }

    // Getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public bool IsAtMaxHealth() => Mathf.Approximately(currentHealth, maxHealth);
}