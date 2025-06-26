using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float deceleration = 7f;
    public float turnResistance = 12f;
    public float maxSpeed = 20f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    public bool isGrounded { get; private set; }

    private bool hasJumped;

    private Animator animator;


    public AudioSource audioData;

    public AudioSource damageSound;

    public AudioSource deathSound;

    [Header("Ring Settings")]

    public int ringCount = 10;

    public event Action OnRingsLost;
    public event Action OnPlayerDeath;

    [Header("Death screen")]

    public Sprite deathSprite;
    public GameObject gameOverImage;

    private SpriteRenderer spriteRenderer;

    private CircleCollider2D circleCollider;

    private bool isDead = false;

    //private Vector2 deathVelocity = new Vector2(0, 8f);


    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        rb.freezeRotation = true;
        audioData.Stop();

    }
    private IEnumerator HandleDeathAnimation()
    {
        isDead = true;
        FindFirstObjectByType<PlayerFollower>().isFollowing = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        moveInput = 0;
        animator.enabled = false;
        circleCollider.enabled = false;

        spriteRenderer.sprite = deathSprite;

        yield return new WaitForSeconds(0.1f);

        float t = 0f;
        float duration = 1.5f;

        //MathF.Sin gives you the heigh of a point going around a cricle.
        //Imagine drawing a cricle with a pen, if you walkk around the circle, Sin tells you
        // how high you are. It's there to smoothen out calculations through PI.
        //pi is perfect because it gives youa perfect arc. anything less would make the sin
        //curve not smooth, or more would make it wiggly. 
        //this is good for bounces, a jump, a bobbing animation or a sinewave-style float.

        while (t < duration)
        {
            float y = Mathf.Sin(Mathf.PI * (t / duration)) * 6f;
            transform.position += Vector3.up * y * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = 2f;
        rb.linearVelocity = new Vector2(0, -10f);

        yield return new WaitForSeconds(2f);

        if (gameOverImage != null)
        {
            gameOverImage.SetActive(true);
        }
    }
    public void TakeDamage()

    {
        if (isDead) return;
        if (ringCount > 0)
        {
            ringCount = 0;
            OnRingsLost?.Invoke();
            Debug.Log("rings lost");
            damageSound.Play();
            rb.linearVelocity = new Vector2(-8f, -8f);
        }
        else
        {
            OnPlayerDeath?.Invoke();
            Debug.Log("you've died");
            deathSound.Play();
            StartCoroutine(HandleDeathAnimation());
        }
    }


    void Update()
    {

        if (isDead) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isRunning", moveInput != 0 && isGrounded);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumped = true;
            audioData.Play();
        }



        animator.SetBool("isJumping", !isGrounded);


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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Always allow movement; freeze rotation only
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Deceleration logic if no input
        if (isGrounded && moveInput == 0)
        {
            // Apply deceleration manually as a force
            float decelForce = -Mathf.Sign(rb.linearVelocity.x) * deceleration;
            rb.linearVelocity = new Vector2(
                Mathf.MoveTowards(rb.linearVelocity.x, 0f, Mathf.Abs(decelForce) * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );

            // Once velocity is basically zero, freeze position
            if (Mathf.Abs(rb.linearVelocity.x) < 0.05f)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }

            return;
        }




        // ✅ Keep your acceleration system
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate = (moveInput != 0)
            ? (Mathf.Sign(moveInput) != Mathf.Sign(rb.linearVelocity.x) ? turnResistance : acceleration)
            : deceleration;

        float movement = accelRate * speedDiff * Time.fixedDeltaTime;
        float newX = rb.linearVelocity.x + movement;

        // ✅ Clamp to max speed (in case of overshoot)
        newX = Mathf.Clamp(newX, -maxSpeed, maxSpeed);

        // ✅ FORCE X velocity to ignore slope-induced changes, preserve vertical Y
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
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
