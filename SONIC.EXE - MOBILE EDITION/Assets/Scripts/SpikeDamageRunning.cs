using UnityEngine;

public class SpikeDamageRunning : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControllerRunning player = collision.GetComponent<PlayerControllerRunning>();

        if (player != null)
        {
            StartCoroutine(player.HandleDeathAnimationForRunning());
        }
    }
}
