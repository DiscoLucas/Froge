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

    [Header("Cooldown")]
    public float grapplingCooldown;
    float grapplingCooldownTimer;

    bool isGrappling;

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

        RaycastHit hit;
        if (Physics.Raycast(Mouth.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
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

    }

    void StopGrapple()
    {
        isGrappling = false;
        grapplingCooldownTimer = grapplingCooldown;
        lr.enabled = false;
    }

    
}
