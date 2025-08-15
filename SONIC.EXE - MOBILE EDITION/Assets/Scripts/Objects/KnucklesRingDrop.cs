using UnityEngine;
using System.Collections;

public class KnucklesRingDrop : MonoBehaviour
{

    public float pickupDelay = 2f;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {


            if (Time.time < spawnTime + pickupDelay)
                return;

                            KnucklesPlayerController player = other.gameObject.GetComponent<KnucklesPlayerController>();

            player.CollectRing();
            player.UpdateRingUI();
            Destroy(gameObject);
        }
    }
    public IEnumerator destroyItem()
    {

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
