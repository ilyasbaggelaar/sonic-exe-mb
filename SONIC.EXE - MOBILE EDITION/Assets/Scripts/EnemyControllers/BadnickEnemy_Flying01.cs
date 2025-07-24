using UnityEngine;
using System.Collections;

public class FlyingEnemyController : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float moveDuration = 5f;
    public float idleDuration = 2f;
    public int lives = 1;

    public AudioSource flyingBadnikDefeatedSound;

    [Header("Attack Functionality")]

    public float detectionRange = 5f;
    public float detectionAngle = 120f;
    public float attackCooldown = 4f;

    public GameObject beamPrefab;
    public Transform firePoint;
    private GameObject player;

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D theColliderOfThisStupidFlyingEnemeny;

    private int direction = -1; // -1 = left, 1 = right
    private bool isMoving = false;

    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        theColliderOfThisStupidFlyingEnemeny = GetComponent<Collider2D>();
        flyingBadnikDefeatedSound.Stop();
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            isMoving = false;

            yield return new WaitForSeconds(idleDuration);

            isMoving = true;

            yield return new WaitForSeconds(moveDuration);

            direction *= -1;
            FlipSprite();
        }
    }

    private void Update()
    {

        if (!isAttacking && Time.time > lastAttackTime + attackCooldown)
        {
            DetectAndAttackPlayer();
        }

        if (isMoving)
        {
            transform.position += new Vector3(direction * movementSpeed * Time.deltaTime, 0f, 0f);
        }
    }

    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction == 1;
        }
        if (firePoint != null)
        {
            Vector3 localPos = firePoint.localPosition;
            localPos.x = direction == -1 ? -0.143f : 0.143f;
            firePoint.localPosition = localPos;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.isGrounded)
                {
                    player.TakeDamage(transform.position);
                }
                else if (lives >= 1)
                {
                    lives--;
                    player.TakeKnockback(transform.position);
                }
                else if (lives == 0)
                {
                    player.TakeKnockback(transform.position);
                    isMoving = false;
                    theColliderOfThisStupidFlyingEnemeny.enabled = false;
                    StartCoroutine(EnemyDefeated());
                }
            }
        }
    }

    private void DetectAndAttackPlayer()
    {
        if (player == null)
        {
            GameObject potentialPlayer = GameObject.FindGameObjectWithTag("Player");
            if (potentialPlayer != null)
            {
                player = potentialPlayer;
            }
        }

        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(GetFacingDirection(), directionToPlayer);



            if (Mathf.Abs(angle - 45f) <= 1f && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                isAttacking = true;
                lastAttackTime = Time.time;

                animator.SetTrigger("Attack");

                Invoke(nameof(FireBeam), 0.5f);
            }
        }
    }

    private Vector2 GetFacingDirection()
    {
        return direction == -1 ? Vector2.left : Vector2.right;
    }

    private void FireBeam()
    {
        if (beamPrefab != null && firePoint != null)
        {

            Vector2 directionToPlayer = (player.transform.position - firePoint.position).normalized;

            GameObject beam = Instantiate(beamPrefab, firePoint.position, Quaternion.identity);
            BeamController beamScript = beam.GetComponent<BeamController>();
            beamScript.Initialize(directionToPlayer);

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            beam.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        isAttacking = false;
    }


    IEnumerator EnemyDefeated()
    {
        flyingBadnikDefeatedSound.Play();
        animator.SetBool("isDefeated", true);

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
