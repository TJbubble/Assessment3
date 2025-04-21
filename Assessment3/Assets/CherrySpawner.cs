using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherrySpawner : MonoBehaviour
{
    public GameObject cherryPrefab;
    public Terrain terrain;
    public int maxCherries = 10;
    public float spawnRadius = 5f;

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
        // 获取地形范围
        Vector3 terrainSize = terrain.terrainData.size;

        // 随机位置（XZ平面）
        float randomX = Random.Range(spawnRadius, terrainSize.x - spawnRadius);
        float randomZ = Random.Range(spawnRadius, terrainSize.z - spawnRadius);

        // 获取地形高度
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        // 生成位置（略高于地面）
        Vector3 spawnPos = new Vector3(randomX, terrainHeight + 0.5f, randomZ);

        // 实例化樱桃
        Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
    }
}