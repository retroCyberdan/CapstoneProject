using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerActivator : MonoBehaviour
{
    [Header("Script Settings")]
    [Tooltip("Lo script da attivare")]
    public EnemySpawner enemySpawner;

    [Header("Identificatore Unico")]
    [Tooltip("ID univoco per questo activator (usato per il salvataggio)")]
    public string activatorID = "EnemySpawner_1";

    [Header("Trigger Settings")]
    [Tooltip("Tag del player (default: 'Player')")]
    public string playerTag = "Player";

    [Tooltip("Attiva lo script una sola volta?")]
    public bool activateOnce = true;

    [Tooltip("Disattiva questo game object dopo l'attivazione?")]
    public bool disableAfterActivation = true;

    private bool _hasActivated = false;

    void Start()
    {
        // verifica che ci sia un Collider con isTrigger attivo
        Collider col = GetComponent<Collider>();
        if (col == null) Debug.LogWarning($"[EnemySpawnerActivator] Nessun Collider trovato su {gameObject.name}!");

        else if (!col.isTrigger) Debug.LogWarning($"[EnemySpawnerActivator] Il Collider su {gameObject.name} non è impostato come Trigger!");

        if (enemySpawner != null)
        {
            // controlla se lo script era già stato attivato in un salvataggio precedente
            if (SaveSystem.Instance != null && SaveSystem.Instance.IsScriptActivated(activatorID))
            {
                // riattiva lo spawner senza far partire il trigger
                enemySpawner.enabled = true;
                _hasActivated = true;
                Debug.Log($"[EnemySpawnerActivator] Script '{activatorID}' ripristinato da salvataggio!");

                if (disableAfterActivation)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                // assicura che lo spawner sia disattivato all'inizio
                enemySpawner.enabled = false;
            }
        }
        else
        {
            Debug.LogError($"[EnemySpawnerActivator] Script Activator non assegnato su {gameObject.name}!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (activateOnce && _hasActivated) return; // <- se deve attivarsi solo una volta e è già stato attivato, esci

            ActivateSpawner(); // <- attiva lo spawner
        }
    }

    void ActivateSpawner()
    {
        if (enemySpawner != null)
        {
            enemySpawner.enabled = true;
            _hasActivated = true;

            if (SaveSystem.Instance != null) SaveSystem.Instance.RegisterActivatedScript(activatorID); // <- registra l'attivazione nel SaveSystem

            Debug.Log($"[EnemySpawnerActivator] Script attivato su {enemySpawner.gameObject.name}!");

            if (disableAfterActivation) gameObject.SetActive(false); // <- disattiva questo game object se richiesto
        }
    }
}