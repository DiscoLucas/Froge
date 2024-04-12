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
    Vector3 startPos;
    bool isExtending;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = RPGInputManager.GetInputActions();
        punchAction = inputActions.Character.Punch;
        punchAction.performed += ctx => OnPunch();
        punchAction.Enable();

    }

    private void OnPunch()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        /*var maxDistance = Vector3.Distance(tounge.transform.position, tounge.transform.position * toungeLength);
        if (maxDistance > toungeLength)
        {
            RetractTongue();
        }*/

        
    }

    /*private void RetractTongue()
    {
        Debug.Log("Retracting");
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        //rb.AddForce((startPos - tounge.transform.position).normalized * toungeSpeed, ForceMode.Impulse);
        rb.MovePosition(startPos);
        
    }*/


}
