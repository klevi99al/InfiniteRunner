using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    [Header("References")]
    public float nextDistance = 90;
    public static Manager Instance { get; private set; }
    public GameObject[] areaPrefabs; // Prefabs of the areas to be pooled
    public GameObject[] initialActiveAreas; // Initial areas already active in the hierarchy
    private int areaIndex = 2;

    private List<GameObject> areaPool = new();
    private List<GameObject> activeAreas = new();

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
        }

        // Initialize the pool
        InitializePool();
    }

    private void InitializePool()
    {
        // Add already active areas to the pool
        foreach (GameObject activeArea in initialActiveAreas)
        {
            if (activeArea != null)
            {
                activeAreas.Add(activeArea);
                areaPool.Add(activeArea);
                Debug.Log("Adding initial active area to pool: " + activeArea.name);
            }
        }

        // Instantiate and add prefab areas to the pool
        foreach (GameObject areaPrefab in areaPrefabs)
        {
            if (areaPrefab != null)
            {
                GameObject area = Instantiate(areaPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0), transform);
                area.SetActive(false);
                areaPool.Add(area);
                Debug.Log("Instantiating and adding prefab area to pool: " + area.name);
            }
        }

        // Shuffle the pool initially
        Shuffle(areaPool);
    }

    public void SpawnNextArea()
    {
        areaIndex++;

        // Deactivate the oldest active area
        if (activeAreas.Count > 0)
        {
            GameObject oldestActiveArea = activeAreas[0];
            Debug.Log("Deactivating: " + oldestActiveArea.name);
            oldestActiveArea.SetActive(false);
            activeAreas.RemoveAt(0);
        }

        // Shuffle the pool
        Shuffle(areaPool);

        // Activate the next area from the pool
        if (areaPool.Count > 0)
        {
            GameObject nextArea = areaPool[0];
            areaPool.RemoveAt(0);
            nextArea.SetActive(true);
            nextArea.transform.position = new Vector3(0, 0, areaIndex * nextDistance);
            Debug.Log("Activating: " + nextArea.name);
            activeAreas.Add(nextArea);
        }
        else
        {
            Debug.LogWarning("No areas available in the pool.");
        }
    }

    private void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}