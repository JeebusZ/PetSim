using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawners : MonoBehaviour
{
    public Minable orePrefabs; // Prefab of ore to spawn
    public string _tag;
    public Transform platform; // Reference to the platform transform
    public int oresToSpawn; // Number of ores to spawn
    public float spawnInterval = 0.1f; // Time interval between spawns
    public float spawnCheckRadius = 0.5f; // Radius for overlap check
    public float yOffset;
    public int spawnedOres;

    // Layers to consider as obstacles (walls, platforms)
    public LayerMask obstacleLayer;

    private Vector3 spawnAreaMin;
    private Vector3 spawnAreaMax;

    void Start()
    {
        // Initialize spawn area based on platform dimensions
        InitializeSpawnArea();

        // Start spawning ores
        StartCoroutine(SpawnOresCoroutine());
    }

    void InitializeSpawnArea()
    {
        // Get the platform's bounds
        Renderer platformRenderer = platform.GetComponent<Renderer>();
        Bounds bounds = platformRenderer.bounds;

        spawnAreaMin = bounds.min;
        spawnAreaMax = bounds.max;
    }

    IEnumerator SpawnOresCoroutine()
    {
        while (true)
        {
            while (spawnedOres < oresToSpawn)
            {
                // Generate random position within the spawn area
                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    spawnAreaMax.y, // Ensure ores spawn on top of the platform
                    Random.Range(spawnAreaMin.z, spawnAreaMax.z)
                );

                // Check if the position is valid
                if (IsValidSpawnPosition(spawnPosition))
                {
                    spawnedOres++;
                    spawnPosition.y = yOffset;
                    GameObject oreSpawned = ObjectPooler.instance.SpawnFromPool(_tag, spawnPosition, Quaternion.identity);
                    oreSpawned.GetComponent<Lootable>().spawners = this;
                }
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return null; // Wait for the next frame before rechecking
        }
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // Check for overlap with obstacles
        Collider[] hitColliders = Physics.OverlapSphere(position, spawnCheckRadius, obstacleLayer);
        return hitColliders.Length == 0;
    }

    void OnDrawGizmosSelected()
    {
        if (platform == null)
            return;

        // Draw the spawn area in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(spawnAreaMin.x, spawnAreaMax.y, spawnAreaMin.z), new Vector3(spawnAreaMax.x, spawnAreaMax.y, spawnAreaMin.z));
        Gizmos.DrawLine(new Vector3(spawnAreaMax.x, spawnAreaMax.y, spawnAreaMin.z), new Vector3(spawnAreaMax.x, spawnAreaMax.y, spawnAreaMax.z));
        Gizmos.DrawLine(new Vector3(spawnAreaMax.x, spawnAreaMax.y, spawnAreaMax.z), new Vector3(spawnAreaMin.x, spawnAreaMax.y, spawnAreaMax.z));
        Gizmos.DrawLine(new Vector3(spawnAreaMin.x, spawnAreaMax.y, spawnAreaMax.z), new Vector3(spawnAreaMin.x, spawnAreaMax.y, spawnAreaMin.z));
    }
}
