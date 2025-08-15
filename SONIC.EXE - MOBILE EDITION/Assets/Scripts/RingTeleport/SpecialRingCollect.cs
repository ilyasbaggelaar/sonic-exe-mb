using UnityEngine;

public class SpecialRingCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        KnucklesPlayerController knucklesPlayer = collision.GetComponent<KnucklesPlayerController>();

        if (player != null)
        {
            Debug.Log("PICKED UPL PLAYER SPECIAL RING");
            player.specialRings();
            Destroy(gameObject);

        }

        else if (knucklesPlayer != null)
        {
            knucklesPlayer.specialRings();
            Debug.Log("PICKED UPL KNUCKLES PLAYER SPECIAL RING");
            Destroy(gameObject);
        }

        
        
    }
}
