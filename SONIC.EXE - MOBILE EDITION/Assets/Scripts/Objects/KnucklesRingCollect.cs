using UnityEngine;
using TMPro;

public class KnucklesRingColllect : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            KnucklesPlayerController player = other.gameObject.GetComponent<KnucklesPlayerController>();

            player.CollectRing();
            player.UpdateRingUI();
            Destroy(gameObject);
        }
    }
}
