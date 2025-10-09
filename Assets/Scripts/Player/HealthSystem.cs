using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("References")]
    [SerializeField] private UI_HealthBar uiHealthBar;

    // events per notificare cambiamenti di salute
    public event Action<float, float> OnHealthChanged; // <- currentHealth, maxHealth
    public event Action OnDeath;


    // getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public bool IsAtMaxHealth() => Mathf.Approximately(currentHealth, maxHealth);

    private void Start()
    {
        currentHealth = maxHealth;

        if (uiHealthBar != null)
        {
            uiHealthBar.maxHealthValue = maxHealth;
            uiHealthBar.healthValue = currentHealth;
        }
    }

    private void Update()
    {
        // Debug controls - rimuovi in produzione
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(10f);

        if (Input.GetKeyDown(KeyCode.G)) Heal(15f);
    }

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

        if (currentHealth <= 0) Die();
    }

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

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    public void SetMaxHealth(float value, bool healToMax = false)
    {
        maxHealth = Mathf.Max(value, 1f);

        if (healToMax) currentHealth = maxHealth;

        else currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (uiHealthBar != null) uiHealthBar.maxHealthValue = maxHealth;

        UpdateHealthBar();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void HealToMax()
    {
        Heal(maxHealth - currentHealth);
    }

    private void UpdateHealthBar()
    {
        if (uiHealthBar != null) uiHealthBar.healthValue = currentHealth;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} è morto!");
        // Aggiungi qui la logica di morte (animazioni, disattivazione, ecc.)
    }
}