using UnityEngine;

public class BeamController : MonoBehaviour
{
    public float speed = 6f;

    public float lifetime = 3f;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        Destroy(gameObject, lifetime);
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(transform.position);
            }
            Destroy(gameObject);
        }
    }
}
