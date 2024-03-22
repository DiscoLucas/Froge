using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    [SerializeField]
    GameObject leftLeg;
    [SerializeField]
    GameObject rightLeg;
    private Collider leftLegCollider;
    private Collider rightLegCollider;

    public Rigidbody rb;

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
        leftLegCollider = leftLeg.GetComponent<Collider>();
        rightLegCollider = rightLeg.GetComponent<Collider>();
        
    }

    
    void FixedUpdate()
    {
        // check if the player is on the ground
        
        
        // Run function for forward jump
        if (Input.GetAxis("Vertical") > 0)
        {
            ForwardMovement(rb);
        }

        // Run function for stabilizing the player rotation
        InAir(rb);

        // Jump up
        if (Input.GetAxis("Jump") > 0 && IsGrounded)
        {
            Jump();
        }
        

        // rotate the player
        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddTorque(Vector3.up * rotationForce * Input.GetAxis("Horizontal"));
        }
        
    }

    
// check if the player is on the ground
    public bool IsGrounded
    {
        get
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.6f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void InAir(Rigidbody rb)
    {
        if (!IsGrounded)
        {
            // Stabalize the player rotation
            print("stabilizing");
            var newXAngle = PID.Update(Time.deltaTime, rb.rotation.x, 0);
            var newZAngle = PID.Update(Time.deltaTime, rb.rotation.z, 0);
            Vector3 currentAngle = rb.rotation.eulerAngles;
            rb.rotation = Quaternion.Euler(newXAngle, currentAngle.y, newZAngle);
        }
    }

    private void ForwardMovement(Rigidbody rb)
    {
        if (IsGrounded)
        {
            rb.AddForceAtPosition(Vector3.forward * forwardForce, leftLeg.transform.position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.forward * forwardForce, rightLeg.transform.position, ForceMode.Impulse);
        }
    }
}
