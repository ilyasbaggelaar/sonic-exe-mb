using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public Transform player;
    private Collider2D col;



    void Start()
    {
        col = GetComponent<Collider2D>();

    }
    void FixedUpdate()
    {
        float playerHeight = player.GetComponent<Collider2D>().bounds.size.y;

        if (player == null) return;

        float playerY = player.position.y - (playerHeight / 2f);
        float platformY = col.bounds.max.y;


        col.enabled = playerY > platformY + 0.9f;

        Debug.DrawLine(new Vector2(col.bounds.min.x, col.bounds.max.y),
               new Vector2(col.bounds.max.x, col.bounds.max.y),
               Color.green, 0.1f);
    }
}
