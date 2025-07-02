using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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

    private Animator animator;

    public AudioSource backGroundMusic;

    public AudioSource audioData;

    public AudioSource damageSound;

    public AudioSource deathSound;

    [Header("Ring Settings")]

    public int ringCount = 10;

   public static int lives = 3;

    public event Action OnRingsLost;
    public event Action OnPlayerDeath;

    [Header("Death screen")]

    public Sprite deathSprite;
    public GameObject gameOverImage;

    private SpriteRenderer spriteRenderer;

    private CircleCollider2D circleCollider;

    private bool isDead = false;

    private bool invFrame = false;

    //private Vector2 deathVelocity = new Vector2(0, 8f);


    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();


        if (SaveManager.HasSaveData())
        {
            SaveManager.Load(out lives, out ringCount, out _);
            Debug.Log($"Loaded: {lives}, rings: {ringCount}");
        }

        rb.freezeRotation = true;
        backGroundMusic.volume = 0.4f;
        audioData.Stop();
        damageSound.Stop();
        deathSound.Stop();

    }
    public IEnumerator HandleDeathAnimation()
    {
        isDead = true;

                    OnPlayerDeath?.Invoke();
            Debug.Log("you've died");
            deathSound.Play();
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

        yield return new WaitForSeconds(3f);

        if (lives > 0)
        {
            lives--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Lives Left:" + lives);
        }

        else if (lives == 0)
        {
            gameOverImage.SetActive(true);
        }
    }

    private IEnumerator VolumefadeOut()
    {
        float startVolume = backGroundMusic.volume;
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            backGroundMusic.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            t += Time.deltaTime;

            yield return null;

        }

        backGroundMusic.mute = true;

           Debug.Log("Volume faded to 0.");
    }
    public void TakeDamage(Vector2 sourcePosition)

    {
        if (invFrame) return;
        if (isDead) return;

         StartCoroutine(HandleDamageKnockback(sourcePosition));
    }


//this could be mundane. same with the above. polish up when everything works fine.
    public void TakeKnockback(Vector2 sourcePosition)
    {
        StartCoroutine(HandleKnockback(sourcePosition));
    }

    private IEnumerator HandleKnockback(Vector2 sourcePosition)
    {
        Vector2 knockbackDirection = (transform.position - (Vector3)sourcePosition).normalized;
        Vector2 knockbackForce = new Vector2(knockbackDirection.x * 8f, 8f);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 2f;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        yield break;
    }

    private IEnumerator HandleDamageKnockback(Vector2 sourcePosition)
    {
        isDead = true;
        invFrame = true;

        Vector2 knockBackDirection = (transform.position - (Vector3)sourcePosition).normalized;
        Vector2 knockBackForce = new Vector2(knockBackDirection.x * 10f, 8f);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 2f;
        rb.AddForce(knockBackForce, ForceMode2D.Impulse);

        if (ringCount > 0)
        {
            ringCount = 0;
            OnRingsLost?.Invoke();
            Debug.Log("rings lost");
            damageSound.Play();
        }
        else
        {

            StartCoroutine(HandleDeathAnimation());
            StartCoroutine(VolumefadeOut());
            yield break; // Skip re-enabling control
        }

        yield return new WaitForSeconds(0.6f);

        isDead = false;
        yield return new WaitForSeconds(3f);
        invFrame = false;
    }

    void OnApplicationQuit()
    {
        SaveManager.Save(lives, ringCount);   
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

        if (isDead) return;
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
