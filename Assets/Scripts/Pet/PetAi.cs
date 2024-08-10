using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;

public enum PetState
{
    FollowingPlayer,
    MovingToOre,
    InteractingWithOre
}
public class PetAi : MonoBehaviour
{
    
    public Transform player;
    public float followRange = 10f;
    public float oreDetectionRange = 15f;
    public float orePrioritizationDistance = 5f;
    public float oreBreakDistance = 5f;
    public float pickupDistance = 0.5f;
    public float interactionCooldown = 2f;

    [SerializeField] private Pet pet;
    private NavMeshAgent agent;
    [SerializeField] private PetState currentState;
    [SerializeField] private Transform currentOre;
    [SerializeField] private List<Transform> oresInRange = new List<Transform>();
    private bool isInteracting = false;
    private Animator anim;
    public Pet _pet { get { return pet; } set { pet = value; } }
    public List<Transform> ores { get {  return oresInRange; } set { oresInRange = value; } }
    public PetState _currentState { get { return currentState; } set {  currentState = value; } }
    public Transform _currentOre {  get { return currentOre; } set {  currentOre = value; } }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = PetState.FollowingPlayer;
        interactionCooldown = pet._attackSpeed;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case PetState.FollowingPlayer:
                agent.stoppingDistance = 3.7f;
                FollowPlayer();
                break;
            case PetState.MovingToOre:
                agent.stoppingDistance = pickupDistance;
                MoveToOre();
                break;
            case PetState.InteractingWithOre:
                agent.stoppingDistance = pickupDistance;
                InteractWithOre();
                break;
            default:
                break;
        }
    }
    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if(distanceToPlayer <= followRange )
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(player.position);
        }

        //Check ores within range
        if (oresInRange.Count > 0)
        {
            if(currentOre != null)
            {
                currentState = PetState.MovingToOre;
            }
            else
            {
                UpdateOreTarget();
            }
        }
    }
    private void MoveToOre()
    {
        // Check if the pet is too far from the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > followRange)
        {
            currentOre = null;
            currentState = PetState.FollowingPlayer;
            return;
        }

        if (currentOre != null)
        {
            Lootable lootable = currentOre.GetComponent<Lootable>();
            if (lootable == null || lootable._lootHealth <= 0)
            {
                // Ore was destroyed or has no health, find another ore
                currentOre = null;
                UpdateOreTarget();
                return;
            }

            float distanceToOre = Vector3.Distance(transform.position, currentOre.position);
            agent.SetDestination(currentOre.position);

            if (distanceToOre <= orePrioritizationDistance)
            {
                currentState = PetState.InteractingWithOre;
            }
        }
        else
        {
            currentState = PetState.FollowingPlayer;
        }
    }
    private void InteractWithOre()
    {
        // Check if the pet is too far from the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > followRange)
        {
            currentOre = null;
            currentState = PetState.FollowingPlayer;
            return;
        }

        if (currentOre != null)
        {
            Lootable lootable = currentOre.GetComponent<Lootable>();
            if (lootable == null || lootable._lootHealth <= 0)
            {
                // Ore was destroyed or has no health, find another ore
                currentOre = null;
                UpdateOreTarget();
                return;
            }

            float distanceToOre = Vector3.Distance(transform.position, currentOre.position);
            if (distanceToOre <= pickupDistance)
            {
                // Perform damage to ore
                if (!isInteracting)
                {
                    anim.SetTrigger("Attack");
                }
            }
        }
        else
        {
            currentState = PetState.FollowingPlayer;
        }
    }
    private IEnumerator InteractCooldown()
    {
        isInteracting = true;

        if (currentOre != null)
        {
            Lootable lootable = currentOre.GetComponent<Lootable>();

            if (lootable != null)
            {
                lootable.TakeDamage((long)pet._power);

                // Check if the ore is destroyed
                if (lootable._lootHealth <= 0)
                {
                    oresInRange.Remove(currentOre);
                    currentOre = null;
                    UpdateOreTarget();
                }
            }
        }

        yield return new WaitForSeconds(interactionCooldown);

        isInteracting = false;
        if (currentOre == null)
        {
            UpdateOreTarget();
        }
    }
    public void Attack()
    {
        StartCoroutine(InteractCooldown());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lootable"))
        {
            Transform ore = other.transform;
            if(ore != null)
            {
                oresInRange.Add(ore);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lootable"))
        {
            Transform ore = other.GetComponent<Transform>();
            if (ore != null)
            {
                oresInRange.Remove(ore);
            }
        }
    }

    private void UpdateOreTarget()
    {
        // Remove ores with no health from the list
        oresInRange.RemoveAll(ore => ore == null || ore.GetComponent<Lootable>()._lootHealth <= 0);

        if (oresInRange.Count > 0)
        {
            // Randomly pick an ore from the remaining ones
            int randomIndex = Random.Range(0, oresInRange.Count);
            currentOre = oresInRange[randomIndex];
            currentState = PetState.MovingToOre;
        }
        else
        {
            // If no ores are available, return to following the player
            currentOre = null;
            currentState = PetState.FollowingPlayer;
        }
    }
}
