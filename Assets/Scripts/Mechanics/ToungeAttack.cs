using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnStairs.RCC.Inputs;
using UnityEngine.InputSystem;
using System;

public class ToungeAttack : MonoBehaviour
{
    [Header("References")]
    private RPGInputActions inputActions;
    InputAction punchAction;
    public Rigidbody rb;
    public SpringJoint spring;
    public Collider tongueCollider;
    public Transform mouthPos;

    [Header("Forces")]
    public float tongueForce = 10f;
    public float retractionSpringForce = 100f;


    // Start is called before the first frame update
    void Awake()
    {
        inputActions = RPGInputManager.GetInputActions();
        punchAction = inputActions.Character.Punch;
        punchAction.performed += ctx => OnPunch();
        punchAction.Enable();
        rb = rb.GetComponentInChildren<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        spring = GetComponent<SpringJoint>();
        spring.spring = 0;
        
    }

    private void OnPunch()
    {
        rb.velocity = Vector3.zero;
        spring.spring = 0;
        rb.AddForce(Vector3.forward * tongueForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, mouthPos.position) >= spring.maxDistance)
        {
            Invoke(nameof(RetractTongue), 0.1f);
        }
    }

    private void RetractTongue()
    {
        spring.spring = retractionSpringForce;
    }

    private void OnTriggerEnter(Collider mouth)
    {
        // Reset the tongue position
        rb.MovePosition(mouthPos.position);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
    }
}
