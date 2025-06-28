using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform player;
    public float parallaxFactor = 0.5f;
    public float spriteWidth = 1.27f;

    private Transform[] sprites;
    private float lastPlayerX;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned to ParallaxLayer.");
            enabled = false;
            return;
        }

        int count = transform.childCount;
        sprites = new Transform[count];

        for (int i = 0; i < count; i++)
            sprites[i] = transform.GetChild(i);

        lastPlayerX = player.position.x;
    }

    void Update()
    {
        float deltaX = player.position.x - lastPlayerX;
        lastPlayerX = player.position.x;

        // Move the layer
        transform.position += Vector3.right * deltaX * parallaxFactor;

        // Looping logic
        foreach (Transform sprite in sprites)
        {
            float distance = player.position.x - sprite.position.x;

            if (distance > spriteWidth * sprites.Length / 2f)
            {
                sprite.position += Vector3.right * spriteWidth * sprites.Length;
            }
            else if (distance < -spriteWidth * sprites.Length / 2f)
            {
                sprite.position -= Vector3.right * spriteWidth * sprites.Length;
            }
        }
    }
}
