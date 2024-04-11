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
    bool isExtending;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = RPGInputManager.GetInputActions();
        punchAction = inputActions.Character.Punch;
        punchAction.performed += ctx => OnPunch();
        punchAction.Enable();

        rb = tounge.GetComponent<Rigidbody>();
        rb.useGravity = false;

        startPos = tounge.transform.position;
        var endPos = tounge.transform.position + tounge.transform.forward * toungeLength;
    }

    // Update is called once per frame
    void Update()
    {
        /*var maxDistance = Vector3.Distance(tounge.transform.position, tounge.transform.position * toungeLength);
        if (maxDistance > toungeLength)
        {
            RetractTongue();
        }*/

        if (isExtending)
        {
            float distance = Vector3.Distance(tounge.transform.position, startPos);
            if (distance >= toungeLength)
            {
                isExtending = false;
                Debug.Log("setting isExtending to false and running RetractTongue");
                RetractTongue();
            }
        }
    }

    /*private void RetractTongue()
    {
        Debug.Log("Retracting");
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        //rb.AddForce((startPos - tounge.transform.position).normalized * toungeSpeed, ForceMode.Impulse);
        rb.MovePosition(startPos);
        
    }*/

    private void OnPunch()
    {
        isExtending = true;
        rb.useGravity = true;
        rb.AddForce(transform.forward * toungeSpeed, ForceMode.Impulse);

        Vector3 targetPosition = transform.position + transform.forward * toungeLength;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
    }
    private void RetractTongue()
    {
        StartCoroutine(RetractCoroutine());
    }
    private IEnumerator RetractCoroutine()
    {
        rb.velocity = Vector3.zero;
        // Disable gravity to prevent interference
        rb.useGravity = false;

        // Move the tongue back to its starting position using interpolation
        float elapsedTime = 0f;
        float retractDuration = 1f; // Adjust as needed
        Vector3 initialPosition = tounge.transform.position;
        Debug.DrawLine(initialPosition, startPos, Color.red, 1f);
        while (elapsedTime < retractDuration)
        {
            tounge.transform.position = Vector3.Lerp(initialPosition, startPos, elapsedTime / retractDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the tongue ends up at the starting position
        tounge.transform.position = startPos;
    }


}
