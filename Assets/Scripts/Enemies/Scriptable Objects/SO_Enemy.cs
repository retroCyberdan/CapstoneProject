using UnityEngine;

/// <summary>
/// Classe base astratta per tutti i nemici.
/// Non può essere istanziata direttamente - serve come template per creare nemici specifici.
/// </summary>
public abstract class SO_Enemy : ScriptableObject
{
    [Header("Prefab Settings")]
    [Tooltip("Il prefab del nemico con tutti i componenti necessari (Animator, CharacterController, EnemyVisionAI)")]
    public GameObject enemyPrefab;

    [Header("Movement Settings")]
    [Tooltip("Velocità di movimento del nemico")]
    public float moveSpeed = 3f;

    [Tooltip("Tempo minimo di attesa tra i movimenti")]
    public float minWaitTime = 2f;

    [Tooltip("Tempo massimo di attesa tra i movimenti")]
    public float maxWaitTime = 5f;

    [Tooltip("Tempo minimo di movimento")]
    public float minMoveTime = 2f;

    [Tooltip("Tempo massimo di movimento")]
    public float maxMoveTime = 4f;

    [Tooltip("Velocità di rotazione del nemico")]
    public float rotationSpeed = 10f;

    [Header("Damage Settings")]
    [Tooltip("Danno inflitto al giocatore")]
    public float damage = 10f;

    [Tooltip("Cooldown tra un danno e l'altro")]
    public float damageCooldown = 1f;

    [Header("Vision Settings")]
    [Tooltip("Raggio di visione del nemico")]
    public float visionRadius = 10f;

    [Tooltip("Numero di segmenti per il campo visivo")]
    public int fovSegments = 50;

    /// <summary>
    /// Metodo virtuale per inizializzazioni custom dei nemici specifici
    /// </summary>
    public virtual void Initialize(EnemyController controller)
    {
        // I nemici derivati possono sovrascrivere questo metodo per logiche custom
    }

    /// <summary>
    /// Valida i dati dello Scriptable Object
    /// </summary>
    public virtual void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        minWaitTime = Mathf.Max(0f, minWaitTime);
        maxWaitTime = Mathf.Max(minWaitTime, maxWaitTime);
        minMoveTime = Mathf.Max(0f, minMoveTime);
        maxMoveTime = Mathf.Max(minMoveTime, maxMoveTime);
        rotationSpeed = Mathf.Max(0f, rotationSpeed);
        damage = Mathf.Max(0f, damage);
        damageCooldown = Mathf.Max(0f, damageCooldown);
        visionRadius = Mathf.Max(0f, visionRadius);
        fovSegments = Mathf.Max(3, fovSegments);
    }
}