using UnityEngine;

public class SpikeDamageRunning : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControllerRunning player = collision.GetComponent<PlayerControllerRunning>();
        KnuckesRunning knuckles = collision.GetComponent<KnuckesRunning>();

        if (player != null)
        {
            StartCoroutine(player.HandleDeathAnimationForRunning());
        }

        else if (knuckles != null)
        {
            StartCoroutine(knuckles.HandleDeathAnimationForRunning());
        }
    }
}
