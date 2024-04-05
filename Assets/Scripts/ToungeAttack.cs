using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnStairs.RCC.Inputs;
using UnityEngine.InputSystem;
using System;

public class ToungeAttack : MonoBehaviour
{
    private RPGInputActions inputActions;
    InputAction punchAction;

    public GameObject tounge;
    public float toungeSpeed = 10f;
    public float toungeLength = 10f;
    [SerializeField]
    Rigidbody rb;
    Vector3 startPos;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = RPGInputManager.GetInputActions();
        punchAction = inputActions.Character.Punch;
        punchAction.performed += ctx => OnPunch();
        punchAction.Enable();

        rb = tounge.GetComponent<Rigidbody>();

        startPos = tounge.transform.position;
        var endPos = tounge.transform.position + tounge.transform.forward * toungeLength;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnPunch()
    {
        Vector3 targetPosition = transform.position + transform.forward * toungeLength;
        
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > toungeLength)
        {
            targetPosition = transform.position + transform.forward * toungeLength;
        }
        
    }

    
}
