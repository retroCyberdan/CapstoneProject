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

                    // Disattiva il nemico dopo il tempo di vita specificato
                    PoolManager.Instance.StartCoroutine(PoolManager.Instance.DisableAfterDelay(enemy, e.lifetime));
                }

                return;
            }
            randomSpawnFactor -= e.spawnProbability;
        }
    }
}