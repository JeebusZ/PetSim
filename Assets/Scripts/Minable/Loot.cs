using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour, IPooledObjects
{
    public long value;
    public float floatHeight = 1f; // Height at which the loot will float
    public float floatDamping = 0.5f; // How quickly the object stabilizes at the float height
    public float magnetRange = 5f; // Range within which the loot will be attracted to the player
    public float magnetSpeed = 5f; // Speed at which the loot moves towards the player
    public float pickupDistance = 0.5f; // Distance at which the loot is picked up

    private Rigidbody rb;
    private Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Check the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= magnetRange)
            {
                // Move towards the player
                rb.isKinematic = false; // Ensure the Rigidbody is not kinematic during movement

                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                rb.velocity = directionToPlayer * magnetSpeed;

                // Check if the loot is close enough to be picked up
                if (distanceToPlayer <= pickupDistance)
                {
                    CollectLoot();
                }
            }
        }
    }

    private void CollectLoot()
    {
        // Add loot value to the player's inventory
        playerTransform.GetComponent<PlayerInventory>().AddCopper(value);

        // Play VFX here (optional)

        // Deactivate the loot object
        this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            StartCoroutine(FloatAboveGround());
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private IEnumerator FloatAboveGround()
    {
        // Ensure the Rigidbody is kinematic so it doesn't fall
        rb.isKinematic = true;

        // Determine the target height to float above the ground
        float targetHeight = transform.position.y + floatHeight;

        while (true)
        {
            float newY = Mathf.Lerp(transform.position.y, targetHeight, floatDamping * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }

    public void OnObjectSpawn()
    {
        // Initialize or reset any parameters if needed
        playerTransform = GameObject.FindWithTag("Player").transform;
    }
}
