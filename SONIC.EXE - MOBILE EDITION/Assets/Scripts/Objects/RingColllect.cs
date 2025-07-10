using UnityEngine;
using TMPro;

public class RingColllect : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.CollectRing();
            player.UpdateRingUI();
            Destroy(gameObject);
        }
    }
}
