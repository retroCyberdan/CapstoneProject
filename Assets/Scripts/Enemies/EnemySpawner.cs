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
    public string bossPoolTag = "Boss"; // <- tag del boss per identificarlo

    private GameObject _currentBoss; // <- riferimento al boss attivo

    private bool _isEnemyMusicPlaying = false;
    private bool _isBossMusicPlaying = false;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, spawnInterval);
    }

    private void Update()
    {
        HandleDynamicMusic();
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

                // configura il nemico con i dati dello ScriptableObject
                if (enemy != null)
                {
                    EnemyController controller = enemy.GetComponent<EnemyController>();
                    if (controller != null && e.enemyData != null)
                    {
                        controller.SetEnemyData(e.enemyData);
                    }

                    // gestione audio in base al tipo di nemico
                    if (AudioManager.Instance != null)
                    {
                        if (e.poolTag == bossPoolTag)
                        {
                            AudioManager.Instance.PlayBossSpawnSound(spawnPoint.position);
                            _currentBoss = enemy;
                        }
                        else
                        {
                            AudioManager.Instance.PlayEnemySpawnSound(spawnPoint.position);
                        }
                    }

                    PoolManager.Instance.StartCoroutine(DisableEnemyAfterDelay(enemy, e.lifetime, e.poolTag == bossPoolTag)); // <- disattiva il nemico dopo il tempo di vita specificato
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

            // se era il boss, resetta il riferimento
            if (isBoss && enemy == _currentBoss)
            {
                _currentBoss = null;
            }
        }
    }

    public bool HasActiveEnemies()
    {
        foreach (var e in enemies)
        {
            if (e.poolTag != bossPoolTag)
            {
                List<GameObject> activeObjects = PoolManager.Instance.GetActiveObjects(e.poolTag);
                if (activeObjects.Exists(obj => obj.activeInHierarchy))
                    return true;
            }
        }
        return false;
    }

    public bool HasActiveBoss()
    {
        return _currentBoss != null && _currentBoss.activeInHierarchy;
    }

    public void HandleDynamicMusic()
    {
        if (AudioManager.Instance == null) return;

        bool hasEnemies = HasActiveEnemies();
        bool hasBoss = HasActiveBoss();

        if (hasBoss) // <- caso 1: boss presente -> interrompe musica nemici e avvia boss music
        {
            if (!_isBossMusicPlaying)
            {
                AudioManager.Instance.StopBGM();
                AudioManager.Instance.PlayBossSpawnSound(transform.position);
                _isBossMusicPlaying = true;
                _isEnemyMusicPlaying = false;
            }
            return;
        }

        if (hasEnemies && !_isEnemyMusicPlaying) // <- caso 2: nessun boss ma ci sono nemici -> musica nemici
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayEnemySpawnSound(transform.position);
            _isEnemyMusicPlaying = true;
            _isBossMusicPlaying = false;
            return;
        }

        if (!hasEnemies && !hasBoss && (_isEnemyMusicPlaying || _isBossMusicPlaying)) // <- caso 3: nessun nemico e nessun boss -> ferma tutto
        {
            AudioManager.Instance.StopBGM();
            _isEnemyMusicPlaying = false;
            _isBossMusicPlaying = false;
        }
    }
}