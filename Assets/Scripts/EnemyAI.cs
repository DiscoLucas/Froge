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
    float minWaitTime = 1f;
    float maxWaitTime = 5f;
    bool isWaiting = false;
    [Header("Physical Attributes")]
    public float runSpeed = 10f;
    public float walkSpeed = 5f;
    public float fov = 50f;
    public int viewDistance = 10;
    [Header("Combat Attributes")]
    public float attackDistance = 0.5f;
    public float attackCooldown = 2f;
    public float alertTime = 10f;
    public bool isAlerted = false;
    public float alertFOVModifier = 1.5f;

    Transform playerPos;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform;
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {
            //Debug.Log("Reached destination");
            // Wait before moving to the next waypoint
            isWaiting = true;
            StartCoroutine(Wait(Patrol));
        }*/
        
        
        if (CanSeePlayer())
        {
            Chase();
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                //Debug.Log("Reached destination");
                // Wait before moving to the next waypoint
                Patrol();
            }
    }

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

    void Patrol()
    {
        isWaiting = false;
        agent.speed = walkSpeed;
        // Chek if any waypoints have been set up
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints set up for enemy");
            return;
        }
            

        agent.destination = waypoints[destPoint].position;
        //Debug.Log("Going to waypoint " + destPoint);

        // Choose the next destination point when the agent gets to one
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    void Chase()
    {
        // Draw yellow cube above the agent
        Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
        agent.speed = runSpeed;
        agent.destination = playerPos.position;

        // Attack the player when there within a certain distance
        if (Vector3.Distance(transform.position, playerPos.position) < attackDistance)
        {
            Attack();
        }
    }
    
    void Attack()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        agent.destination = playerPos.position;
        // Checking attack cooldown
        if (Time.time > attackCooldown)
        {
            Debug.Log("Attacking player");
            //player.GetComponent<PlayerHealth>().TakeDamage(1);
            attackCooldown = Time.time + attackCooldown;
            Debug.Log("Attack cooldown: " + attackCooldown);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 targetDirection = playerPos.position + Vector3.up - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        if (angle < fov * 0.5f)
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
}
