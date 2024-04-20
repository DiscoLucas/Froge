using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnStairs.RCC.Inputs;
using UnityEngine.InputSystem;
using System;

public class SpringTongue : MonoBehaviour
{
    private RPGInputActions inputActions;
    InputAction punchAction;
    public float tongueForce = 10f;
    public float retractionSpringForce = 100f;
    public Rigidbody rb;
    public SpringJoint spring;
    public Transform mouth;
    bool isExtending;
    bool moveToMouth;

    // Start is called before the first frame update
    void Start()
    {
        rb = rb.GetComponentInChildren<Rigidbody>();
        spring = rb.GetComponent<SpringJoint>();
        inputActions = RPGInputManager.GetInputActions();
        punchAction = inputActions.Character.Punch;
        punchAction.performed += ctx => OnPunch();
        punchAction.Enable();
        rb.useGravity = false;
        spring.axis = Vector3.forward;
    }

    private void OnPunch()
    {
        spring.spring = 0;
        isExtending = true;
        //rb.useGravity = true;
        rb.AddForce(transform.forward * tongueForce, ForceMode.Impulse);
        
    }

    // Update is called once per frame
    void Update()
    {

        
        if (isExtending)
        {
            float distance = Vector3.Distance(rb.transform.position, mouth.position);
            // distance from the tongue to the mouth
            if (distance >= spring.maxDistance)
            {

                Debug.Log("retracting");
                RetractTongue();
            }
        }
    }

    private void RetractTongue()
    {
        rb.useGravity = false;
        isExtending = false;
        spring.spring = retractionSpringForce;
        moveToMouth = true;
        
    }

    private void FixedUpdate()
    {
        // if the tongue is close enough to the mouth, stop the spring and move the tongue to the mouth
        if (moveToMouth && Vector3.Distance(rb.transform.position, mouth.position) <= 2f)
        {
            Debug.Log("returning to mouth");
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(mouth.position);
            moveToMouth = false;
        }
    }
}
