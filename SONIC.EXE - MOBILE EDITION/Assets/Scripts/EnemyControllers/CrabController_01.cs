using UnityEngine;

public class CrabController_01 : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.isGrounded)
                {
                    player.TakeDamage();
                }
                else {
                    Destroy(gameObject);
                }
            }
        }
    }
}
