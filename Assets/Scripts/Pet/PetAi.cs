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
            UpdateOreTarget();
            if(currentOre != null)
            {
                currentState = PetState.MovingToOre;
            }
        }
    }
    private void MoveToOre()
    {
        Lootable lootable = currentOre.GetComponent<Lootable>();
        //if(currentOre != null && !lootable.isOpen)
        //{
        //    int rand = Random.Range(0, oresInRange.Count);
        //    currentOre = oresInRange[rand];
        //}
        if (currentOre != null)
        {
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
        if(currentOre != null)
        {
            float distanceToOre = Vector3.Distance(transform.position, currentOre.position);
            //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if(distanceToOre <= pickupDistance)
            {
                //Perform damage to ore
                if(!isInteracting)
                {
                    StartCoroutine(InteractCooldown());
                    anim.SetTrigger("Attack");
                }
            }
            //else if(distanceToPlayer <= oreBreakDistance)
            //{
            //    isInteracting = false;
            //    currentState = PetState.FollowingPlayer;
            //}
        }
    }
    private IEnumerator InteractCooldown()
    {
        if(currentOre.GetComponent<Lootable>()._lootHealth <= 0) 
        {
            if(currentOre != null)
            {
                oresInRange.Remove(currentOre);
            }
            isInteracting = false;
            currentOre = null;
            currentState = PetState.FollowingPlayer;
        }
        else
        {
            isInteracting = true;
            //Perfom damage
            currentOre.GetComponent<Lootable>().TakeDamage((long)pet._power);

            yield return new WaitForSeconds(interactionCooldown);

            isInteracting = false;
            currentOre = null;
            currentState = PetState.FollowingPlayer;
        }
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
        if (oresInRange.Count > 0)
        {
            //Prio low health ores
            Transform closestOre = null;
            float minHealth = Mathf.Infinity;

            foreach (Transform ore in oresInRange)
            {
                if (ore.GetComponent<Lootable>()._lootHealth < minHealth)
                {
                    minHealth = ore.GetComponent<Lootable>()._lootHealth;
                    closestOre = ore;
                }
            }
            currentOre = closestOre.transform;
        }
        else
        {
            currentOre = null;
        }
    }
}
