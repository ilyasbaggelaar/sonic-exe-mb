using UnityEngine;
using System.Collections;

public class CrabController_01 : MonoBehaviour
{

    public float movementSpeed = 2f;
    public float moveDuration = 5f;

    public float idleDuration = 2f;

    private Rigidbody2D rb;

    private Animator animator;

    private int direction = -1;

    public int lives = 1;

    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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

    

    void OnCollisionEnter2D(Collision2D collision)
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
                    Destroy(gameObject);
                }
            }
        }
    }
}
