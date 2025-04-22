using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherrySpawner : MonoBehaviour
{
    public static CherrySpawner instance;  // 添加这行

    public GameObject cherryPrefab;
    public Terrain terrain;
    public int maxCherries = 10;
    public float spawnRadius = 5f;

    private void Awake()
    {
        // 单例初始化
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnInitialCherries();
    }

    void SpawnInitialCherries()
    {
        for (int i = 0; i < maxCherries; i++)
        {
            SpawnCherry();
        }
    }

    public void SpawnCherry()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        float randomX = Random.Range(spawnRadius, terrainSize.x - spawnRadius);
        float randomZ = Random.Range(spawnRadius, terrainSize.z - spawnRadius);
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));
        Vector3 spawnPos = new Vector3(randomX, terrainHeight + 0.5f, randomZ);

        Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
    }
}