using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    [Tooltip("Tag del pool per identificare il tipo di nemico")]
    public string poolTag;

    [Tooltip("Dati dello ScriptableObject del nemico")]
    public SO_Enemy enemyData;

    [Tooltip("Probabilità di generazione di questo nemico")]
    public float spawnProbability;

    [Tooltip("Durata vita del nemico (in secondi)")]
    public float lifetime;
}

public class EnemySpawner : MonoBehaviour
{
    public EnemySpawnData[] enemies;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;

    [Header("Audio Settings")]
    public AudioClip enemySpawnSound; // <- suono quando spawna un nemico normale
    public AudioClip bossSpawnMusic; // <- musica quando spawna il boss
    public string bossPoolTag = "Boss"; // <- tag del boss per identificarlo

    private GameObject _currentBoss; // <- riferimento al boss attivo

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemies.Length == 0 || spawnPoints.Length == 0) return;

        float totalSpawnProbability = 0;
        foreach (var e in enemies) totalSpawnProbability += e.spawnProbability;

        float randomSpawnFactor = Random.value * totalSpawnProbability;

        foreach (var e in enemies)
        {
            if (randomSpawnFactor < e.spawnProbability)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = PoolManager.Instance.SpawnFromPool(e.poolTag, spawnPoint.position, Quaternion.identity);

                // Configura il nemico con i dati dello ScriptableObject
                if (enemy != null)
                {
                    EnemyController controller = enemy.GetComponent<EnemyController>();
                    if (controller != null && e.enemyData != null)
                    {
                        controller.SetEnemyData(e.enemyData);
                    }

                    // Gestione audio in base al tipo di nemico
                    if (e.poolTag == bossPoolTag)
                    {
                        // Se è il boss, riproduci la musica del boss
                        if (bossSpawnMusic != null && AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayOneShot(bossSpawnMusic, spawnPoint.position);
                        }
                        _currentBoss = enemy;
                    }
                    else
                    {
                        // Se è un nemico normale, riproduci il suono di spawn
                        if (enemySpawnSound != null && AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayOneShot(enemySpawnSound, spawnPoint.position);
                        }
                    }

                    // Disattiva il nemico dopo il tempo di vita specificato
                    PoolManager.Instance.StartCoroutine(DisableEnemyAfterDelay(enemy, e.lifetime, e.poolTag == bossPoolTag));
                }

                return;
            }
            randomSpawnFactor -= e.spawnProbability;
        }
    }

    private IEnumerator DisableEnemyAfterDelay(GameObject enemy, float delay, bool isBoss)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null)
        {
            enemy.SetActive(false);

            // Se era il boss, resetta il riferimento
            if (isBoss && enemy == _currentBoss)
            {
                _currentBoss = null;
            }
        }
    }
}