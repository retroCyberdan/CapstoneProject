using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;
    private HealthSystem _healthSystem;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _healthSystem = GetComponent<HealthSystem>();

        // Sottoscrivi agli eventi del HealthSystem
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged += OnHealthChanged;
            _healthSystem.OnDeath += OnDeath;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Aggiorna parametri di movimento solo se il player è vivo
        if (_healthSystem != null && _healthSystem.IsAlive())
        {
            _animator.SetFloat("horizontal", _playerController.Horizontal);
            _animator.SetFloat("vertical", _playerController.Vertical);
            _animator.SetFloat("speed", _playerController.CurrentSpeed);
        }
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        // Trigger animazione di hit quando viene inflitto danno
        if (currentHealth < maxHealth)
        {
            _animator.SetTrigger("isHitted");

            // Riproduce suono di hit
            if (AudioManager.Instance != null) AudioManager.Instance.PlayHit(transform.position);
        }
    }

    private void OnDeath()
    {
        // Attiva animazione di morte
        _animator.SetBool("isDead", true);

        // Riproduce suono di morte
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDeath(transform.position);
    }

    private void OnDestroy()
    {
        // Rimuovi sottoscrizioni agli eventi per evitare memory leak
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= OnHealthChanged;
            _healthSystem.OnDeath -= OnDeath;
        }
    }
}