using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform[] waypoints;
    int destPoint = 0;
    
    [Header("Physical Attributes")]
    public float runSpeed = 10f;
    public float walkSpeed = 5f;
    public float fov = 50f;
    private float currentFOV;
    public int viewDistance = 10;
    public float lookSpeed;

    [Header("Combat Attributes")]
    public float attackDistance = 0.5f;
    public float attackCooldown = 2f;
    public float alertTime = 10f;
    public float alertFOVModifier = 1.5f;

    [Header("Debugging")]
    public string stateDisplay;
    public bool startLookout;
    public bool isAlerted = false;
    public bool isLooking;
    [Tooltip("While alerted, the enemy will always be able to see the player within this range.")]
    public float situationalAwareness = 3f;

    Transform playerPos;
    GameObject player;

    public enum EnemyState
    {
        PATROLLING,
        ALERT,
        ATTACKING
    }
    EnemyState currentState;
    float alertTimeElapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform;
        currentState = EnemyState.PATROLLING;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case EnemyState.PATROLLING:
                stateDisplay = "Patrolling";
                currentFOV = fov;
                Patrol();
                if (CanSeePlayer())
                {
                    currentState = EnemyState.ALERT;
                    alertTimeElapsed = 0f; // Reset alert timer after spotting the player again
                }
                break;
            case EnemyState.ALERT:
                stateDisplay = "Alert";
                currentFOV = fov * alertFOVModifier;
                Chase();
                // if the player is no longer in sight, start the alert timer
                if (!CanSeePlayer())
                {
                    alertTimeElapsed += Time.deltaTime;
                    if (alertTimeElapsed > alertTime)
                    {
                        // AI can't find the player, go back to patrolling
                        currentState = EnemyState.PATROLLING;
                    }
                }
                if (Vector3.Distance(transform.position, playerPos.position) < attackDistance)
                {
                    currentState = EnemyState.ATTACKING;
                }
                break;
            case EnemyState.ATTACKING:
                stateDisplay = "Attacking";
                Attack();
                if (Vector3.Distance(transform.position, playerPos.position) > attackDistance)
                {
                    currentState = EnemyState.ALERT;
                }
                break;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 targetDirection = playerPos.position + Vector3.up - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        if (angle < currentFOV * 0.5f ||
            isAlerted && Vector3.Distance(transform.position, playerPos.position) < situationalAwareness)
        {
            //Debug.Log("Player is in field of view");
            RaycastHit hit;

            if (Physics.Raycast(transform.position, targetDirection, out hit, viewDistance))
            {
                Debug.DrawRay(transform.position, targetDirection, Color.red);
                //Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void Patrol()
    {
        agent.speed = walkSpeed;
        // Chek if any waypoints have been set up
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints set up for enemy");
            return;
        }
        agent.destination = waypoints[destPoint].position;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Choose the next destination point when the agent gets to one
            destPoint = (destPoint + 1) % waypoints.Length;
        }
    }
    void Chase()
    {
        // Draw yellow ray above the agent
        Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
        agent.speed = runSpeed;
        // transform.LookAt(playerPos.position, Vector3.up); TODO: make the AI face the player only in the XZ plane

        // Look for the player
        if (CanSeePlayer())
        {
            agent.destination = playerPos.position;
            isLooking = false;
        }
        else if (!isLooking)
        {
            // Player lost, look from side to side
           StartCoroutine(LookAround());
           isLooking = true;
        }
    }
    private IEnumerator LookAround() // TODO: fix this
    {
        stateDisplay = "Alert" + " - Looking around";
        agent.isStopped = true;
        float lookTimer = 0f;
        float lookAngle = 0f; 
        float lookRange = 60f;

        while (!CanSeePlayer())
        {
            lookTimer += Time.deltaTime;
            lookAngle = Mathf.Sin(lookTimer * lookSpeed / Mathf.PI) * lookRange;
            transform.RotateAround(transform.position, Vector3.up, lookAngle * Time.deltaTime);
            yield return null;
        }

        // Player found again, resume chase
        isLooking = false;
        agent.isStopped = false;
        agent.destination = playerPos.position;
        StopCoroutine(LookAround());
    }

    void Attack()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        agent.destination = playerPos.position;
        // transform.LookAt(playerPos.position, Vector3.up); TODO: make the AI face the player only in the XZ plane
        // checking if the time since the last attack is greater than the cooldown
        if (Time.time > attackCooldown)
        {
            // Attack the player
            Debug.Log("Attacking player");
            // Reset the attack cooldown
            attackCooldown = Time.time + attackCooldown;
        }
    }

    
    /*
    /// <summary>
    /// runs a function after a random amount of time
    /// </summary>
    /// <param name="onComplete">This function is run after a randomized period</param>
    /// <returns></returns>
    private IEnumerator Wait(Action onComplete)
    {

        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        
        yield return new WaitForSeconds(waitTime);
        {
            try
            {
                isWaiting = false;
                onComplete();
            }
            // Catch any errors if no function was declared
            catch (Exception e)
            {
                Debug.LogError("Error in Wait: " + e.Message + ", did you forget to declare a function parameter?");
            }
            
        }

    }

    

    

    */
}
