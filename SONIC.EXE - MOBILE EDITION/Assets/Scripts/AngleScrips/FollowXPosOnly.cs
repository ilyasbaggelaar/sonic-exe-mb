using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowXOnlyWithGravity : MonoBehaviour
{
    public Transform playerTransform;
    public float xOffset = 1.0f; // ahead of player
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 pos = rb.position;
        pos.x = playerTransform.position.x + (Mathf.Sign(playerTransform.localScale.x) * xOffset);
        rb.MovePosition(new Vector2(pos.x, rb.position.y)); // only X changes, gravity handles Y
    }
}
