using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TMPro;
using UnityEngine.Playables;
public class KnuckesRunning : MonoBehaviour
{

    [Header("Jump Settings")]
    public float jumpForce = 10f;

    public Transform groundCheck;

    public float groundCheckRadius = 0.2f;

    public LayerMask groundLayer;

    private Rigidbody2D rb;

    public bool isGrounded { get; private set; }

    private Animator animator;

    public AudioSource bGM;

    public AudioSource jumpSound;

    public AudioSource deathSound;

    public AudioSource deathTrack; //This is different than deathSound, this is the music that accompanies it.

    public TextMeshProUGUI ringText;

    //rings
    public int ringCountRunning = 3;

    public int ringCount = 10;

    public static int lives = 3;

    [Header("Death Screen")]

    //sprite for when the character dies.
    public Sprite deathSprite;

    public PlayableDirector cutsceneDirector;

    //GameObject image on CanvasUI for game over transfer.
    public GameObject gameOverImage;

    private SpriteRenderer spriteRenderer;

    private CircleCollider2D circleCollider;

    private BoxCollider2D boxCollider;

    private bool isDead = false;

    private bool canMove = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
             boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (SaveManager.HasSaveData())
        {
            SaveManager.LoadRunningMode(out lives, out ringCountRunning);
            Debug.Log($"Loaded: {lives}");
        }

        UpdateRingUI();

        StartCoroutine(PlayTitleCard());

        rb.freezeRotation = true;

        bGM.Play();
        bGM.volume = 0.2f;
        //bGM.pitch = -1.3f;
        deathSound.Stop();
        jumpSound.Stop();
    }

    IEnumerator PlayTitleCard()
    {
        canMove = false;
        yield return new WaitForSeconds(2f);

        canMove = true;
    }

    public IEnumerator HandleDeathAnimationForRunning()
    {
        isDead = true;
        Debug.Log("You're dead.");
        deathSound.Play();
        StartCoroutine(VolumeFadeOut());

        FindFirstObjectByType<SideScrollerLevel>().pushLevel = false;

        rb.gravityScale = 0;

        animator.enabled = false;

        //circleCollider.enabled = false;
        cutsceneDirector.enabled = false;

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        else if (circleCollider != null)
        {
            circleCollider.enabled = false;
        }
        spriteRenderer.sprite = deathSprite;



        yield return new WaitForSeconds(0.1f);

        float t = 0f;
        float duration = 1.5f;

        while (t < duration)
        {

            float y = Mathf.Sin(MathF.PI * (t / duration)) * 6f;
            transform.position += Vector3.up * y * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = 2f;
        rb.linearVelocity = new Vector2(0, -10f);
        if (ringCountRunning >= 1)
        {
            ringCountRunning--;
        }

        UpdateRingUI();

        if (ringCountRunning <= 0)
        {
            lives--;
        }
        SaveManager.SaveRunningMode(lives, ringCountRunning);

        yield return new WaitForSeconds(3f);

        if (lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        else if (lives <= 0)
        {
             deathTrack.Play();
            gameOverImage.transform.SetAsLastSibling();
            RectTransform rect = gameOverImage.GetComponent<RectTransform>();

            rect.anchoredPosition = Vector2.zero;

            if (!PlayerPrefs.HasKey("NextLifeTime"))
            {
                string nextLifeTime = System.DateTime.UtcNow.AddMinutes(30).ToString();
                PlayerPrefs.SetString("NextLifeTime", nextLifeTime);
                PlayerPrefs.Save();
            }

            StartCoroutine(FadeInGameOver());
        }
    }


    public IEnumerator VolumeFadeOut()
    {
        float startVol = bGM.volume;
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            bGM.volume = Mathf.Lerp(startVol, 0, t / duration);

            t += Time.deltaTime;

            yield return null;
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

       
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene("MainMenu");
    }
    public void UpdateRingUI()
    {
        ringText.text = ringCountRunning.ToString();
    }
    void OnApplicationQuit()
    {
        SaveManager.Save(lives, ringCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || !canMove) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpSound.Play();
        }

    }

    void FixedUpdate()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }
}
