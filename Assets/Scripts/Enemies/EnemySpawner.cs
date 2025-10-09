using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public string poolTag;
    public float spawnProbability; // <- probabilità di generazione
    public float lifetime; // <-- durata vita del nemico (in secondi)
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

                // se il nemico è valido, disattivalo dopo (lifetime)
                if (enemy != null) PoolManager.Instance.StartCoroutine(PoolManager.Instance.DisableAfterDelay(enemy, e.lifetime));

                return;
            }
            randomSpawnFactor -= e.spawnProbability;
        }
    }
}