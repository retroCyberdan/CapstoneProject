using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    [Header("References")]
    [SerializeField] private UI_HealthBar _uiHealthBar;
    [SerializeField] private CanvasGroup _deathCanvas;

    [Header("Death Settings")]
    [SerializeField] private float _deathCanvasFadeSpeed = 1f;
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    // events per notificare cambiamenti di salute
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    // getters
    public float GetCurrentHealth() => _currentHealth;
    public float GetMaxHealth() => _maxHealth;
    public float GetHealthPercentage() => _currentHealth / _maxHealth;
    public bool IsAlive() => _currentHealth > 0;
    public bool IsAtMaxHealth() => Mathf.Approximately(_currentHealth, _maxHealth);

    private void Start()
    {
        _currentHealth = _maxHealth;

        if (_uiHealthBar != null)
        {
            _uiHealthBar.maxHealthValue = _maxHealth;
            _uiHealthBar.healthValue = _currentHealth;
        }

        // si assicura che la death canvas sia invisibile all'inizio
        if (_deathCanvas != null)
        {
            _deathCanvas.alpha = 0f;
            _deathCanvas.gameObject.SetActive(false);
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

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0f);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("L'ammontare di guarigione non può essere negativo. Usa TakeDamage() per infliggere danno.");
            return;
        }

        if (_currentHealth <= 0)
        {
            Debug.LogWarning("Non è possibile guarire un'entità morta.");
            return;
        }

        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void SetHealth(float value)
    {
        _currentHealth = Mathf.Clamp(value, 0f, _maxHealth);

        UpdateHealthBar();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0) Die();
    }

    public void SetMaxHealth(float value, bool healToMax = false)
    {
        _maxHealth = Mathf.Max(value, 1f);

        if (healToMax) _currentHealth = _maxHealth;
        else _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

        if (_uiHealthBar != null) _uiHealthBar.maxHealthValue = _maxHealth;

        UpdateHealthBar();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void HealToMax()
    {
        Heal(_maxHealth - _currentHealth);
    }

    private void UpdateHealthBar()
    {
        if (_uiHealthBar != null) _uiHealthBar.healthValue = _currentHealth;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} è morto!");

        // disabilita il controller del player
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        var characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;

        StartCoroutine(HandleDeath()); // <- ferma il gioco e mostra la death canvas
    }

    private IEnumerator HandleDeath()
    {
        // attiva la death canvas
        if (_deathCanvas != null)
        {
            _deathCanvas.gameObject.SetActive(true);

            // fade in graduale
            while (_deathCanvas.alpha < 1f)
            {
                _deathCanvas.alpha += Time.unscaledDeltaTime * _deathCanvasFadeSpeed;
                yield return null;
            }

            _deathCanvas.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(0.5f); // <- aspetta un attimo prima di freezare completamente

        //Time.timeScale = 0f; // <- freezare la scena

        while (!Input.GetKeyDown(KeyCode.Space)) yield return null; // <- aspetta che il giocatore prema Spazio per tornare al main menu

        // ripristina il timeScale e torna al MainMenu
        //Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    public void ResetGame() // <- metodo pubblico per resettare il gioco (da chiamare da un bottone di restart)
    {
        Time.timeScale = 1f;

        if (_deathCanvas != null)
        {
            _deathCanvas.alpha = 0f;
            _deathCanvas.gameObject.SetActive(false);
        }

        _currentHealth = _maxHealth;
        UpdateHealthBar();

        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = true;

        var characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = true;
    }
}