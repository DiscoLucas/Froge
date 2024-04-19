using JohnStairs.RCC.Character.Motor;
using JohnStairs.RCC.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// inspired by https://www.youtube.com/watch?v=TYzZsBl3OI0

public class Grappling : MonoBehaviour
{
    [Header("References")]
    RPGController pm;
    RPGInputActions inputActions;
    InputAction grappleAction;
    public Transform cam;
    public Transform Mouth;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grapple Settings")]
    public float maxGrappleDistance;
    public float grappleDelay;
    Vector3 grapplePoint;
    public float overShootYAxis;

    [Header("Cooldown")]
    public float grapplingCooldown;
    float grapplingCooldownTimer;

    bool isGrappling;
    public bool activeGrapple;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<RPGController>();
        inputActions = RPGInputManager.GetInputActions();
        grappleAction = inputActions.Character.Grapple;
        grappleAction.performed += ctx => StartGrapple();
        grappleAction.Enable();
    }

    void Update()
    {
        if (grapplingCooldownTimer > 0)
        {
            grapplingCooldownTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, Mouth.position);
        }
    }

    /// <summary>
    /// Called when the player presses the grapple button.
    /// Starts selecting a grapple point, and plays the grapple animation.
    /// </summary>
    private void StartGrapple()
    {
        if (grapplingCooldownTimer > 0) return;

        isGrappling = true;

        pm.ActivateControl = false;

        if (Physics.Raycast(Mouth.position, cam.forward, out RaycastHit hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            grapplePoint = Mouth.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    void ExecuteGrapple()
    {
        pm.ActivateControl = true;

        Vector3 lowestPoint = new(transform.position.x, transform.position.y -1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;
        Debug.Log("lowestPoint: " + lowestPoint + " grapplePoint: " + grapplePoint + " highestPointOnArc: " + highestPointOnArc);
        JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    void StopGrapple()
    {
        pm.ActivateControl = true;
        isGrappling = false;
        grapplingCooldownTimer = grapplingCooldown;
        lr.enabled = false;
    }
    /// <summary>
    /// Calculate the kinematics of a required Vector to go from startPoint to endPoint with a given trajectory height.
    /// </summary>
    /// <param name="startPoint">Where you start</param>
    /// <param name="endPoint">Where you want to go</param>
    /// <param name="trajectoryHeight"></param>
    /// <returns></returns>
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Debug.Log("Velocity to set: " + velocityToSet);
        Invoke(nameof(SetVelocity), 0.1f);
        //cc.attachedRigidbody.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
    }
    


    private Vector3 velocityToSet;
    

    private void SetVelocity()
    {
        Debug.Log("Setting velocity");
        //cc.attachedRigidbody.velocity = velocityToSet;
        //Debug.Log("Velocity set: " + cc.attachedRigidbody.velocity);
    }

}
