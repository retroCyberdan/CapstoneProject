using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolData
{
    public string poolTag;
    public GameObject prefab;
    public int poolSize;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public List<PoolData> pools;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }

    void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (PoolData pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.poolTag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool con tag " + tag + " non esiste!");
            return null;
        }

        GameObject obj = _poolDictionary[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        _poolDictionary[tag].Enqueue(obj);

        return obj;
    }

    public IEnumerator DisableAfterDelay(GameObject obj, float delay) // <- disattiva un oggetto dopo un certo tempo
    {
        yield return new WaitForSeconds(delay);

        if (obj != null) obj.SetActive(false);
    }
}