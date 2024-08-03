using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lootable : MonoBehaviour,IPooledObjects
{
    public Minable minable;
    [SerializeField] private float lootHealth;
    [SerializeField] private float lootBonus;

    [SerializeField] private float upForce = 1f;
    [SerializeField] private float sideForce = 0.1f;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] bool randomRotationX = true; // Whether to randomize X rotation
    [SerializeField] bool randomRotationY = true; // Whether to randomize Y rotation
    [SerializeField] bool randomRotationZ = true; // Whether to randomize Z rotation

    public float _lootHealth { get { return lootHealth; }}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(20);
        }
    }
    private void Start()
    {
        SpawnOre();
    }

    public void SpawnOre()
    {
        lootHealth = minable._health;
        lootBonus = minable._bonus;
    }

    public void TakeDamage(float damage)
    {
        if (lootHealth <= 0)
        {
            lootHealth = 0;
        }
        spawnResource();
    }

    public void spawnResource()
    {
        GameObject res;

        // Apply desired randomized rotation
        Quaternion randomRotation = Quaternion.identity;
        if (randomRotationX)
        {
            randomRotation *= Quaternion.Euler(Random.Range(0f, 360f), 0f, 0f);
        }
        if (randomRotationY)
        {
            randomRotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        if (randomRotationZ)
        {
            randomRotation *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        // Randomize spawn position within a radius around the ore
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );
        Vector3 spawnPosition = spawnPos.position + randomOffset;

        // Instantiate the resource
        //res = Instantiate(minable._prefab, spawnPosition, randomRotation);
        res = ObjectPooler.instance.SpawnFromPool(minable._tag, spawnPosition, randomRotation);

        // Apply force to the spawned object
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);
        Vector3 force = new Vector3(xForce, yForce, zForce);
        res.GetComponent<Rigidbody>().velocity = force;

        // Set the value of the loot
        res.GetComponent<Loot>().value = (long)Random.Range(minable._minGather, minable._maxGather);
    }

    public void OnObjectSpawn()
    {
        spawnResource();
    }
}
