    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    GameManager gameManager;
    NavMeshAgent agent;
    AudioManager audioManager;
    public Transform[] waypointsArray;
    private Animator animator;
    int destPoint = 0;

    public float minWaitTime = 2f; //Min wait time between audio plays.
    public float maxWaitTime = 5f; //Max wait time between audio plays.
    public bool audioHasRun = false;

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
    float attackCooldownTimer;
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
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        audioManager = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found in the scene, disabling AI");
            enabled = false;
        }
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
                WalkAnim();
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
                if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
                Attack();
                ChompAnim();
                if (Vector3.Distance(transform.position, playerPos.position) > attackDistance)
                {
                    currentState = EnemyState.ALERT;
                }
                break;
        }
    }

    IEnumerator PlayAudioRandomly() 
    { 
        if (!audioHasRun)
        {
            audioHasRun = true;
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            audioManager.Play("Enemy_sound");
            audioHasRun = false;
        }
    }

    
    public bool CanSeePlayer()
    {
        if (playerPos == null) return false; // Ensure playerPos is not null

        Vector3 targetDirection = (playerPos.position + Vector3.up - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, targetDirection);

        if (angle < currentFOV * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, targetDirection, out hit, viewDistance))
            {
                Debug.DrawRay(transform.position, targetDirection * viewDistance, Color.red);
                //Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        // If not in patrol state, the enemy can always see the player within a certain range
        else if (currentState != EnemyState.PATROLLING &&
            Vector3.Distance(transform.position, playerPos.position) < situationalAwareness)
        {
            return true;
        }
        return false;
    }
    void Patrol()
    {
        agent.speed = walkSpeed;
        // Chek if any waypointsArray have been set up
        if (waypointsArray.Length == 0)
        {
            Debug.LogWarning("No waypoints set up for enemy");
            return;
        }
        
        agent.destination = waypointsArray[destPoint].position;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            //TODO: start coroutine to wait for a random amount of time or edit LookAround to work with the patrol state
            // Choose the next destination point when the agent gets to one
            destPoint = (destPoint + 1) % waypointsArray.Length;
        }
    }
    void Chase()
    {
        // Draw yellow ray above the agent
        Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
        agent.speed = runSpeed;
        //StartCoroutine(PlayAudioRandomly());
        // transform.LookAt(playerPos.position, Vector3.up); TODO: make the AI face the player only in the XZ plane

        // Look for the player
        if (CanSeePlayer())
        {
            StopCoroutine(LookAround());
            agent.destination = playerPos.position;
            RunAnim();
            AudioManager.instance.Play("Enemy_sound");
            isLooking = false;
        }
        else if (!isLooking)
        {
            // Player lost, look from side to side
           StartCoroutine(LookAround());
           isLooking = true;
        }
    }
    private IEnumerator LookAround() // TODO: fix AI gettig stuck in this coroutine
    {
        stateDisplay = currentState + " - Looking around";
        //Debug.Log("Looking around");
        agent.isStopped = true;
        IdleAnim();

        float lookTimer = 0f;
        float lookRange = 60f;
        Quaternion initialRotation = transform.rotation;

        float checkInterval = 0.1f; // How often to check if the player is visible
        float nextCheckTime = 0f;

        while (!CanSeePlayer())
        {
            lookTimer += Time.deltaTime;
            float lookAngle = Mathf.Sin(lookTimer * lookSpeed / Mathf.PI) * lookRange;
            transform.rotation = initialRotation * Quaternion.Euler(0, lookAngle, 0);

            
            if (Time.time > nextCheckTime)
            {
                bool canSee = CanSeePlayer();
                //Debug.Log("Looking around. Can see player: " + canSee);
                if (canSee) break;
                nextCheckTime = Time.time + checkInterval;
            }


            if (lookTimer > alertTime)
            {
                yield return null;
            }
            
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

        if (attackCooldownTimer > 0) return;
        
        if (Time.time > attackCooldown)
        {
            // Attack the player
            Debug.Log("Attacking player");
            // Reset the attack cooldown
            attackCooldown = Time.time + attackCooldown;
            AudioManager.instance.Play("Bonk");
            gameManager.PlayerHit();
        }
    }

    //For the animations
    void IdleAnim()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isChomp", false);
    }

    void WalkAnim()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isChomp", false);
    }

    void RunAnim()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
        animator.SetBool("isChomp", false);
    }

    void ChompAnim()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isChomp", true);
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
