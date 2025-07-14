using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

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

    private bool isDashing;
    public bool isGrounded { get; private set; }

    private Animator animator;

    public AudioSource backGroundMusic;

    public AudioSource audioData;

    public AudioSource damageSound;

    public AudioSource deathSound;

    [Header("speed Dash")]

    private float chargeAmount = 0f;

    public float maxCharge = 10f;

    public float chargeDecayRate = 2f;

    public float chargeBoosMultiplier = 2f;

    private bool isCharging = false;



    [Header("Ring Settings")]

    public TextMeshProUGUI ringText;

    public GameObject ringDropPrefab;
    public int ringCount = 10;

    public int specialRing = 0;

   public static int lives = 3;

    public AudioSource ringPickup;
    public AudioSource specialRingPickup;

    public event Action OnRingsLost;
    public event Action OnPlayerDeath;

    [Header("Death screen")]

    public Sprite deathSprite;
    public GameObject gameOverImage;

    private SpriteRenderer spriteRenderer;

    private CircleCollider2D circleCollider;

    private bool isDead = false;

    public bool invFrame = false;

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

        UpdateRingUI();

        rb.freezeRotation = true;
        backGroundMusic.volume = 0.4f;
        audioData.Stop();
        damageSound.Stop();
        deathSound.Stop();
        ringPickup.Stop();
        specialRingPickup.Stop();

    }

    public void CollectRing()
    {
        ringCount += 1;
        ringPickup.Play();
    }

    public void specialRings()
    {
        specialRing += 1;
        Debug.Log(specialRing);
        specialRingPickup.Play();
    }

    public void UpdateRingUI()
    {
        ringText.text = ringCount.ToString();
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
        lives--;
        SaveManager.Save(lives, ringCount);
        yield return new WaitForSeconds(3f);

        if (lives > 0)
        {
            Debug.Log("Lives Left:" + lives);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }

        else if (lives <= 0 && !PlayerPrefs.HasKey("NextLifeTime"))
        {
            gameOverImage.transform.SetAsLastSibling(); // Make sure it's on top in the Canvas hierarchy

            RectTransform rect = gameOverImage.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            string nextLifeTime = System.DateTime.UtcNow.AddMinutes(30).ToString();
            PlayerPrefs.SetString("NextLifeTime", nextLifeTime);
            PlayerPrefs.Save();
            StartCoroutine(FadeInGameOver());
        }
    }

    IEnumerator FadeInGameOver()
    {
        gameOverImage.SetActive(true);
        CanvasGroup cg = gameOverImage.GetComponent<CanvasGroup>();

        if (cg == null) cg = gameOverImage.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime / 1.5f;
            yield return null;
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

    private IEnumerator PerformBoost(float direction, float charge)
    {
        isDashing = true;
        invFrame = true;
        isCharging = false;

        float boost = charge * chargeBoosMultiplier;

        rb.AddForce(new Vector2(direction * boost, 0f), ForceMode2D.Impulse);

      animator.SetBool("isCharging", false);
        animator.SetBool("isDashing", true);
        Debug.Log("isDashing is now true");
        circleCollider.radius = 0.30f;
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("isDashing", false);
        circleCollider.radius = 0.45f;
        Debug.Log("isdashing is now false");

        isDashing = false;

        invFrame = false;
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
            ringSpawner(ringCount);
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

    public void ringSpawner(int amount)
    {

        //not sure yet if this should go into the PlayerController or if it can be here, will figure it out as i write the code.

        //should spawn rings when hit by enemy. The equal am++ount to how many the player controller has. Might be needed to make that static.
        //in fixed update, apply physics so it bounces, up and down.

        //make a private ienumator (called something like that) that would run for a duration  and afterwards destroys the gameobject itself.

        for (int i = 0; i < amount && i < 50; i++)
        {
            Vector3 spawnOffset = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 0.5f);

            GameObject ring = Instantiate(ringDropPrefab, transform.position + spawnOffset, Quaternion.identity);
            Rigidbody2D rb = ring.GetComponent<Rigidbody2D>();



            Vector2 bounceDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;

            rb.AddForce(bounceDirection * 5, ForceMode2D.Impulse);
            RingDrop ringDrop = ring.GetComponent<RingDrop>();

            if (ringDrop != null)
            {
                StartCoroutine(ringDrop.destroyItem());
            }

        }


    }

    void OnApplicationQuit()
    {
        SaveManager.Save(lives, ringCount);   
    }
    void Update()
    {

        if (isDead) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        isCharging = Input.GetAxisRaw("Vertical") < 0;

        

        if (isCharging && Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            chargeAmount = Mathf.Min(chargeAmount + 1f, maxCharge);
            animator.SetBool("isCharging", moveInput == 0 && isCharging && Input.GetKeyDown(KeyCode.Z));
            circleCollider.radius = 0.30f;
        }
        else if (!isCharging)
        {
            animator.SetBool("isCharging", false);
            circleCollider.radius = 0.45f;
        }

        if (!isCharging && chargeAmount > 0f)
        {
            chargeAmount = Mathf.Max(chargeAmount - chargeDecayRate * Time.deltaTime, 0f);
        }




        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioData.Play();
        }

        if (!isDashing)
        {
            animator.SetBool("isRunning", moveInput != 0 && isGrounded);
            animator.SetBool("isJumping", !isGrounded);
            animator.SetBool("isDucking", isCharging);
        }




        //animation / sprite flip

        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (moveInput > 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }

    void FixedUpdate()
    {

        if (isDead) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Always allow movement; freeze rotation only
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!isCharging && chargeAmount > 0f && moveInput != 0 && !isDashing)
        {



            StartCoroutine(PerformBoost(moveInput, chargeAmount));
            chargeAmount = 0f;
        }

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
