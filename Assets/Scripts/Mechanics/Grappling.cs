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
    PlayerMovement pm;
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
    public KeyCode grappleKey = KeyCode.F;

    [Header("Cooldown")]
    public float grapplingCooldown;
    float grapplingCooldownTimer;

    bool grappling;
    public bool activeGrapple;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        //pm = GetComponent<RPGController>();
        //inputActions = RPGInputManager.GetInputActions();
        //grappleAction = inputActions.Character.Grapple;
        //grappleAction.performed += ctx => StartGrapple();
        //grappleAction.Enable();
    }

    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();
        
        if (grapplingCooldownTimer > 0)
        {
            grapplingCooldownTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
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

        grappling = true;

        pm.freezeMovement = true;

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
        pm.freezeMovement = false;

        Vector3 lowestPoint = new(transform.position.x, transform.position.y -1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;
        Debug.Log("lowestPoint: " + lowestPoint + " grapplePoint: " + grapplePoint + " highestPointOnArc: " + highestPointOnArc);
        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    void StopGrapple()
    {
        pm.freezeMovement = false;
        grappling = false;
        grapplingCooldownTimer = grapplingCooldown;
        lr.enabled = false;
    }
    public bool IsGrappling()
    {
        return grappling;
    }

}
