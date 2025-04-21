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
        // ��ȡ���η�Χ
        Vector3 terrainSize = terrain.terrainData.size;

        // ���λ�ã�XZƽ�棩
        float randomX = Random.Range(spawnRadius, terrainSize.x - spawnRadius);
        float randomZ = Random.Range(spawnRadius, terrainSize.z - spawnRadius);

        // ��ȡ���θ߶�
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        // ����λ�ã��Ը��ڵ��棩
        Vector3 spawnPos = new Vector3(randomX, terrainHeight + 0.5f, randomZ);

        // ʵ����ӣ��
        Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
    }
}