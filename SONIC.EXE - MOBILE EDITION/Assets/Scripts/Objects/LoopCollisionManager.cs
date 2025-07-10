using UnityEngine;

public class LoopCollisionManager : MonoBehaviour
{
    public GameObject leftColliderObject;
    public GameObject rightColliderObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float xVel = rb.linearVelocity.x;

        Debug.Log(xVel);

        if (xVel > 0) // Coming from left
        {
            leftColliderObject.SetActive(true);
            rightColliderObject.SetActive(false);
        }
        else if (xVel < 0) // Coming from right
        {
            rightColliderObject.SetActive(true);
            leftColliderObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Reset both colliders when Sonic leaves
        leftColliderObject.SetActive(true);
        rightColliderObject.SetActive(true);
    }
}
