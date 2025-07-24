using UnityEngine;
using System.Collections;

public class LoopMotionController : MonoBehaviour
{
    public PlayerController controller;
    private Rigidbody2D rb;

    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null && controller != null)
        {
            animator = controller.GetComponent<Animator>();
        }
    }

    public void StartLoop(LoopPoint[] path)
    {
        animator?.SetBool("isCharging", true);
        animator?.SetBool("isDashing", true);
        
        StartCoroutine(FollowPath(path));
    }
private IEnumerator FollowPath(LoopPoint[] path)
{
    rb.linearVelocity = Vector2.zero;
    rb.gravityScale = 0;
    rb.bodyType = RigidbodyType2D.Kinematic;
    controller.enabled = false;

    float moveSpeed = 10f;

    for (int i = 0; i < path.Length; i++)
    {
        while (Vector2.Distance(transform.position, path[i].point.position) > 0.05f)
        {
            // 🔁 Use manually set zRotation for each point
            transform.rotation = Quaternion.Euler(0, 0, path[i].zRotation);

            transform.position = Vector2.MoveTowards(
                transform.position,
                path[i].point.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    transform.rotation = Quaternion.identity;
    rb.bodyType = RigidbodyType2D.Dynamic;
    rb.gravityScale = 2f;
    controller.enabled = true;
    animator?.SetBool("isCharging", false);
    animator?.SetBool("isDashing", false);
}


}
