using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemiesSpawnData
{
    public string tag;
    public float spawnProbability; // <- probabilità di generazione (enemy > villain)
}

public class EnemiesSpawner : MonoBehaviour
{
    public EnemiesSpawnData[] enemies;
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

        float randomSpawnFactor = Random.value * totalSpawnProbability; // <- fattore randomizzante

        foreach (var e in enemies)
        {
            if (randomSpawnFactor < e.spawnProbability)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                PoolManager.Instance.SpawnFromPool(e.tag, spawnPoint.position, Quaternion.identity);
                return;
            }
            randomSpawnFactor -= e.spawnProbability;
        }
    }
}
