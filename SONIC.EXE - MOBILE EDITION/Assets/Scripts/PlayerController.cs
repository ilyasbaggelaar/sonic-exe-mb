using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float deceleration = 7f;
    public float turnResistance = 12f;
    public float maxSpeed = 20f;
    public float friction = 3f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    private Animator animator;

    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private Transform slopeCheck;
    [SerializeField] private float slopeCheckDistance = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;

    }

    void Update()
    {

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isRunning", moveInput != 0);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }


        //animation / sprite flip

        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (moveInput > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.01f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);






        // Friction when idle
        if (isGrounded && moveInput == 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            if (rb.linearVelocity.magnitude > 0.01f)
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        // float accelRate = 0f;

        float accelRate = (moveInput != 0)
        ? (Mathf.Sign(moveInput) != Mathf.Sign(rb.linearVelocity.x) ? turnResistance : acceleration)
        : deceleration;

        float movement = accelRate * speedDiff * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

    }
}


//study this instead.
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class PlayerController : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     public float acceleration = 5f;
//     public float deceleration = 7f;
//     public float turnResistance = 12f;
//     public float maxSpeed = 20f;
//     public float friction = 3f;

//     [Header("Jump Settings")]
//     public float jumpForce = 12f;
//     public Transform groundCheck;
//     public float groundCheckRadius = 0.2f;
//     public LayerMask groundLayer;

//     private Rigidbody2D rb;
//     private float moveInput;
//     private bool isGrounded;
//     private Animator animator;

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         rb.freezeRotation = true;
//     }

//     void Update()
//     {
//         moveInput = Input.GetAxisRaw("Horizontal");
//         animator.SetBool("isRunning", moveInput != 0);

//         if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//         {
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//         }

//         if (moveInput != 0)
//         {
//             Vector3 scale = transform.localScale;
//             scale.x = Mathf.Abs(scale.x) * (moveInput > 0 ? 1 : -1);
//             transform.localScale = scale;
//         }

//         if (Input.GetKeyDown(KeyCode.LeftShift))
//         {
//             Time.timeScale = Time.timeScale == 1.0f ? 0.01f : 1.0f;
//         }
//     }

//     void FixedUpdate()
//     {
//         isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

//         if (isGrounded && moveInput == 0)
//         {
//             // Stop gravity from pushing player down hills
//             rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

//             if (rb.linearVelocity.magnitude > 0.01f)
//             {
//                 rb.linearVelocity = Vector2.zero;
//             }
//             return;
//         }
//         else
//         {
//             rb.constraints = RigidbodyConstraints2D.FreezeRotation; // re-enable movement
//         }

//         // Horizontal movement
//         float targetSpeed = moveInput * maxSpeed;
//         float speedDiff = targetSpeed - rb.linearVelocity.x;

//         float accelRate = (moveInput != 0)
//             ? (Mathf.Sign(moveInput) != Mathf.Sign(rb.linearVelocity.x) ? turnResistance : acceleration)
//             : deceleration;

//         float movement = accelRate * speedDiff * Time.fixedDeltaTime;
//         rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);
//     }
// }
