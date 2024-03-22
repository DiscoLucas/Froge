using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour

{
    [SerializeField]
    Transform leftLeg;
    [SerializeField]
    Transform rightLeg;

    public Rigidbody rb;
    public bool isGrounded;

    [Header("Player Movement")]
    public float forwardForce = 10;
    public float rotationForce = 50;
    public float jumpForce = 5;
    
    PIDController PID;
    [Header("PID Controller - X rotation")]
    public float xP = 1;
    public float xI = 1;
    public float xD = 1;


    // Start is called before the first frame update
    void Start()
    {
        PID = new PIDController(xP, xI, xD);
        
    }

    
    void FixedUpdate()
    {
        // check if the player is on the ground
        isGrounded = Physics.Raycast(leftLeg.position, Vector3.down, 0.2f) || Physics.Raycast(rightLeg.position, Vector3.down, 0.2f);

        // Run function for forward jump
        if (Input.GetAxis("Vertical") > 0)
        {
            ForwardMovement(rb);
        }

        // Run function for stabilizing the player rotation
        InAir(rb);

        // Jump up
        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            Jump();
        }
        

        // rotate the player
        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddTorque(Vector3.up * rotationForce * Input.GetAxis("Horizontal"));
        }
        
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void InAir(Rigidbody rb)
    {
        if (!isGrounded)
        {
            // Stabalize the player rotation
            print("stabilizing");
            var Angle = PID.Update(Time.deltaTime, rb.rotation.x, 0);
            Vector3 xAngle = rb.rotation.eulerAngles;
            rb.rotation = Quaternion.Euler(Angle, xAngle.y, xAngle.z);
        }
    }

    private void ForwardMovement(Rigidbody rb)
    {
        if (isGrounded)
        {
            rb.AddForceAtPosition(Vector3.forward * forwardForce, leftLeg.position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.forward * forwardForce, rightLeg.position, ForceMode.Impulse);
        }
    }
}
