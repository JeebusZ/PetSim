using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lootable : MonoBehaviour,IPooledObjects
{
    public Minable minable;
    [SerializeField] private long lootHealth;
    [SerializeField] private float lootBonus;

    [SerializeField] private float upForce = 1f;
    [SerializeField] private float sideForce = 0.1f;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] bool randomRotationX = true; // Whether to randomize X rotation
    [SerializeField] bool randomRotationY = true; // Whether to randomize Y rotation
    [SerializeField] bool randomRotationZ = true; // Whether to randomize Z rotation

    [SerializeField] private LootableUI lootableUI;
    public OreSpawners spawners;

    public long _lootHealth { get { return lootHealth; }}

    private void Start()
    {
        SpawnOre();
    }

    public void SpawnOre()
    {
        lootHealth = minable._health;
        lootBonus = minable._bonus;
        lootableUI.SetupUI(lootHealth);
    }

    private void Update()
    {
        if(lootHealth <= 0)
        {
            if(spawners != null)
            {
                spawners.spawnedOres -= 1;
            }
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(long damage)
    {
        if (lootHealth <= 0)
        {
            lootHealth = 0;
            gameObject.SetActive(false);
        }
        spawnResource();
        lootHealth -= damage;
        lootableUI.TakeDamage((long)damage);
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
        int roll = Random.Range(1, 101);
        res = null;
        if (roll <= 75) //75% Chance
        {
            res = ObjectPooler.instance.SpawnFromPool(minable._lootDropCoinTag, spawnPosition, randomRotation);
            res.GetComponent<Loot>().value = (long)Random.Range(minable._minGather, minable._maxGather);
        }
        else if (roll > 75 && roll <= 95) // 20% chance
        {
            res = ObjectPooler.instance.SpawnFromPool(minable._lootDropBagTag, spawnPosition, randomRotation);
            res.GetComponent<Loot>().value = ((long)Random.Range(minable._minGather, minable._maxGather) * (long)minable._bonus);
        }
        else if (roll > 95 && roll <= 100) // 5% chance
        {
            res = ObjectPooler.instance.SpawnFromPool(minable._lootDropDiamondTag, spawnPosition, randomRotation);
            res.GetComponent<Loot>().value = (long)Random.Range(minable._minDiamondDrop, minable._maxDiamondDrop);
        }

        // Apply force to the spawned object
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);
        Vector3 force = new Vector3(xForce, yForce, zForce);
        res.GetComponent<Rigidbody>().velocity = force;

        // Set the value of the loot
    }

    public void OnObjectSpawn()
    {
        SpawnOre();
    }
}
