using UnityEngine;

public class KnucklesSpikeDamage : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        KnucklesPlayerController player = collision.GetComponent<KnucklesPlayerController>();

        if (player != null)
        {
            player.TakeDamage2(transform.position);
        }
    }
}
