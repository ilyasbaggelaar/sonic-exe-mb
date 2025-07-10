using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(transform.position);
        }
    }
}
