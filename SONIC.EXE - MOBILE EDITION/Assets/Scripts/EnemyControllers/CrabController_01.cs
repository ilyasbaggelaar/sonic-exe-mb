using UnityEngine;
using System.Collections;

public class CrabController_01 : MonoBehaviour
{

    public float movementSpeed = 2f;
    public float moveDuration = 5f;

    public float idleDuration = 2f;

     public AudioSource defeatedEnemyBadnikSound;

    private Rigidbody2D rb;

    private Animator animator;

    private Collider2D colliders;
    private int direction = -1;

    public int lives = 1;

    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        colliders = GetComponent<Collider2D>();

        defeatedEnemyBadnikSound.Stop();
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            SetAnimationState(isIdle: true);

            isMoving = false;

            yield return new WaitForSeconds(idleDuration);

            SetAnimationState(
    isIdle: false,
    isMovingLeft: direction == -1,
    isMovingRight: direction == 1
);

            isMoving = true;


            yield return new WaitForSeconds(moveDuration);

            direction *= -1;

        }
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position += new Vector3(direction * movementSpeed * Time.deltaTime, 0f, 0f);
        }


    }

    private void SetAnimationState(bool isIdle = false, bool isMovingLeft = false, bool isMovingRight = false)
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isMovingLeft", isMovingLeft);
        animator.SetBool("isMovingRight", isMovingRight);
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.invFrame)
                {
                    Debug.Log("player is invincible- destroying crab!");
                    Destroy(gameObject);
                }
                if (player.isGrounded)
                {
                    Debug.Log("player takes damage");
                    player.TakeDamage(transform.position);
                    player.UpdateRingUI();
                }
                else if (lives >= 1)
                {
                    Debug.Log("player takeknocback");
                    lives--;
                    player.TakeKnockback(transform.position);
                }
                else if (lives == 0)
                {
                    Debug.Log("player takeknocback and destroys crab");
                    player.TakeKnockback(transform.position);
                    StopCoroutine(PatrolRoutine());
                    isMoving = false;

                    colliders.enabled = false;
                    StartCoroutine(EnemyDefeated());

                }
            }
        }
    }


    private IEnumerator EnemyDefeated()
    {
        animator.SetBool("isDefeated", true);

        defeatedEnemyBadnikSound.Play();

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
