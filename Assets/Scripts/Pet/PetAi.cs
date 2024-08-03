using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private PetState currentState;
    private Transform currentOre;
    [SerializeField] private List<Lootable> oresInRange = new List<Lootable>();
    private bool isInteracting = false;
    private float interactionTimer = 0f;

    public Pet _pet { get { return pet; } set { pet = value; } }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = PetState.FollowingPlayer;
        interactionCooldown = pet._attackSpeed;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PetState.FollowingPlayer:
                FollowPlayer();
                break;
            case PetState.MovingToOre:
                MoveToOre();
                break;
            case PetState.InteractingWithOre:
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
        if(currentOre != null)
        {
            float distanceToOre = Vector3.Distance(transform.position, currentOre.position);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            agent.SetDestination(currentOre.position);

            if(distanceToOre <= orePrioritizationDistance)
            {
                currentState = PetState.InteractingWithOre;
            }
            else if (distanceToPlayer <= oreBreakDistance)
            {
                isInteracting = false;
                currentState = PetState.FollowingPlayer;
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
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if(distanceToOre <= pickupDistance)
            {
                //Perform damage to ore
                if(!isInteracting)
                {
                    StartCoroutine(InteractCooldown());

                }
            }
            else if(distanceToPlayer <= oreBreakDistance)
            {
                isInteracting = false;
                currentState = PetState.FollowingPlayer;
            }
        }
    }
    private IEnumerator InteractCooldown()
    {
        isInteracting = true;
        //Perfom damage
        currentOre.GetComponent<Lootable>().TakeDamage(pet._power);

        yield return new WaitForSeconds(interactionCooldown);

        isInteracting = false;
        currentOre = null;
        currentState = PetState.FollowingPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lootable"))
        {
            Lootable ore = other.GetComponent<Lootable>();
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
            Lootable ore = other.GetComponent<Lootable>();
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
            Lootable closestOre = null;
            float minHealth = Mathf.Infinity;

            foreach (Lootable ore in oresInRange)
            {
                if (ore._lootHealth < minHealth)
                {
                    minHealth = ore._lootHealth;
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
