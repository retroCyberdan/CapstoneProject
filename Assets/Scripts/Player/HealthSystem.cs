using System;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("References")]
    [SerializeField] private UI_HealthBar uiHealthBar;
    [SerializeField] private CanvasGroup deathCanvas;

    [Header("Death Settings")]
    [SerializeField] private float deathCanvasFadeSpeed = 1f;

    // events per notificare cambiamenti di salute
    public event Action<float, float> OnHealthChanged;
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

        // Assicurati che la death canvas sia invisibile all'inizio
        if (deathCanvas != null)
        {
            deathCanvas.alpha = 0f;
            deathCanvas.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning("Il danno non può essere negativo. Usa Heal() per recuperare vita.");
            return;
        }

        if (!IsAlive()) return;

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

        // Disabilita il controller del player
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        var characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;

        // Ferma il gioco e mostra la death canvas
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Attiva la death canvas
        if (deathCanvas != null)
        {
            deathCanvas.gameObject.SetActive(true);

            // Fade in graduale
            while (deathCanvas.alpha < 1f)
            {
                deathCanvas.alpha += Time.unscaledDeltaTime * deathCanvasFadeSpeed;
                yield return null;
            }

            deathCanvas.alpha = 1f;
        }

        // Aspetta un attimo prima di freezare completamente
        yield return new WaitForSecondsRealtime(0.5f);

        // Freezare la scena
        Time.timeScale = 0f;
    }

    // Metodo pubblico per resettare il gioco (da chiamare da un bottone di restart)
    public void ResetGame()
    {
        Time.timeScale = 1f;

        if (deathCanvas != null)
        {
            deathCanvas.alpha = 0f;
            deathCanvas.gameObject.SetActive(false);
        }

        currentHealth = maxHealth;
        UpdateHealthBar();

        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = true;

        var characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = true;
    }
}