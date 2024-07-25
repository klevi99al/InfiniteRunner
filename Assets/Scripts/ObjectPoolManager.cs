using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pooling Settings")]
    public GameObject[] prefabs;
    public int poolSize = 5;

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Transform poolContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePool();
        }
    }

    private void InitializePool()
    {
        poolContainer = new GameObject("PoolContainer").transform;

        foreach (GameObject prefab in prefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0), poolContainer);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            pools[prefab] = pool;
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (pools.ContainsKey(prefab) && pools[prefab].Count > 0)
        {
            GameObject obj = pools[prefab].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("No pooled objects available for " + prefab.name);
            return null;
        }
    }

    public void ReturnObjectToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}
