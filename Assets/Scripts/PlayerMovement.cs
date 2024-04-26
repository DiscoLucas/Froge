using JohnStairs.RCC.Inputs;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static TheraBytes.BetterUi.LocationAnimations;
//Sauce https://youtu.be/f473C43s8nE?si=3GUVweo51ebm9OZ9
public class PlayerMovement : MonoBehaviour

{ 
    AudioManager audioManager;

    [Header("Animation")]
    public Animator animator;
    int isWalkingHash;
    int isRunningHash;
    float aniVelocity;
    int velocityHash;

    float movementSpeed;
    float localMovementX;
    float localMovementZ;

    int movementHash;
    int localMovementXHash;
    int localMovementZHash;
    int turnDirectionHash;

    [Header("Movement")]
    InputAction movement;
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    [HideInInspector] public bool freezeMovement;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Sounds")]
    [SerializeField] string[] soundArray;


    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        controls.CharacterControls.Jump.performed += ctx => Jump();
        movement = controls.CharacterControls.Move;

        velocityHash = Animator.StringToHash("Velocity");
        movementHash = Animator.StringToHash("Movement Speed");
        localMovementXHash = Animator.StringToHash("Local Movement X");
        localMovementZHash = Animator.StringToHash("Local Movement Z");
        turnDirectionHash = Animator.StringToHash("Turning Direction");
    }
    private void OnEnable()
    {
        controls = new Inputs();
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (freezeMovement)
            return;

        horizontalInput = movement.ReadValue<Vector2>().x;
        verticalInput = movement.ReadValue<Vector2>().y;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        
        
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        aniVelocity = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        movementSpeed = rb.velocity.magnitude;
        animator.SetFloat(movementHash, movementSpeed);
        //handleAnimation();
    }
    
            private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            animator.SetFloat(velocityHash, aniVelocity);
        }
            


        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {

        if (readyToJump && grounded)
        {
            readyToJump = false;

        audioManager.Play("Jump_Sound");

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
    }
    private void ResetJump()
    {
        readyToJump = true;
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
        activateGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Debug.Log("Velocity to set: " + velocityToSet);
        Invoke(nameof(SetVelocity), 0.1f);

        rb.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
    }

    private bool activateGrapple;
    private Vector3 velocityToSet;
    private Inputs controls;

    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }

    void handleAnimation()
    {
    bool isRunning = animator.GetBool(isRunningHash);
    bool isWalking = animator.GetBool(isWalkingHash);
       
        float movingMagnitude = movement.ReadValue<Vector2>().magnitude;
        if (movingMagnitude != 0 && !isWalking) 
        {
            animator.SetBool("isWalking", true);
        }
        else if (movingMagnitude != 0 && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }
}
