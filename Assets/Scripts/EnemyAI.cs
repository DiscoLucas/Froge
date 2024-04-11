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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {

            // Wait before moving to the next waypoint
            isWaiting = true;
            StartCoroutine(Wait(Patrol));
        }
        else if (!isWaiting)
        {
            
        }


    }

    /// <summary>
    /// runs a function after a random amount of time
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator Wait(Action onComplete)
    {

        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        Debug.Log("Waiting " + waitTime);
        
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
        // Chek if any waypoints have been set up
        if (waypoints.Length == 0)
            return;

        agent.destination = waypoints[destPoint].position;

        // Choose the next destination point when the agent gets to one
        destPoint = (destPoint + 1) % waypoints.Length;
    }
}
