using System;
using System.Collections;
using System.Collections.Generic;
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
    float runSpeed = 10f;
    float walkSpeed = 5f;
    float fov = 50f;
    float viewDistance = 110f;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
        
        Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.red);
        if (CanSeePlayer())
        {
            Debug.Log("player is in sight");
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
        Debug.Log("Going to waypoint " + destPoint);

        // Choose the next destination point when the agent gets to one
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    void Chase()
    {
        agent.speed = runSpeed;
        agent.destination = player.position;
    }
    
    void Attack()
    {
        agent.speed = walkSpeed;
        agent.destination = player.position;
    }

    //TODO: fix this function so that it can actually see the player
    bool CanSeePlayer()
    {
        Vector3 targetDirection = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        if (angle < fov * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, targetDirection, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
