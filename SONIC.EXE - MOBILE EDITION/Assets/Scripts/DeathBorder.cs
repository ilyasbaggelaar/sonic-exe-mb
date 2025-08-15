using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

 void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            KnucklesPlayerController knucklesPlayer = collision.gameObject.GetComponent<KnucklesPlayerController>();

            if (player != null)
            {
                player.StartCoroutine(player.HandleDeathAnimation());
            }
            else if (knucklesPlayer != null)
            {
                
                   knucklesPlayer.StartCoroutine(knucklesPlayer.HandleDeathAnimation());   
            }
        }
    }
}
