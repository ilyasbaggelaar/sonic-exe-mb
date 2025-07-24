using UnityEngine;

[System.Serializable]
public struct LoopPoint
{
    public Transform point;
    public float zRotation;
}

public class LoopZone : MonoBehaviour
{
    public LoopPoint[] loopPathRight; // path if entering from left
    public LoopPoint[] loopPathLeft;  // path if entering from right
    public float requiredSpeed = 8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        float speed = rb.linearVelocity.x;

        if (Mathf.Abs(speed) >= requiredSpeed)
        {
            bool goingRight = speed > 0;
            var loopMotion = other.GetComponent<LoopMotionController>();

            if (loopMotion != null)
            {
                loopMotion.StartLoop(goingRight ? loopPathRight : loopPathLeft);
            }
        }
    }
}
