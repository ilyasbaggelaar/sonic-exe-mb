using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform player; // Reference to the camera
    public float parallaxFactor = 0.5f; // Default value, can be adjusted per layer
    public float spriteWidth = 1.27f;
    public float smoothing = 5f; // Smoothing factor for background movement

    private Transform[] sprites;
    private float lastPlayerX;
    private float totalWidth;
    private Vector3 targetPosition; // Target position for smooth movement

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Camera not assigned to ParallaxLayer.");
            enabled = false;
            return;
        }

        int count = transform.childCount;
        sprites = new Transform[count];

        for (int i = 0; i < count; i++)
            sprites[i] = transform.GetChild(i);

        lastPlayerX = player.position.x;

        // Calculate the total width of all sprites, including the extra ones
        totalWidth = spriteWidth * count;

        Debug.Log($"Total width of parallax layer: {totalWidth}");
        Debug.Log($"Sprite width: {spriteWidth}");
        Debug.Log($"Number of sprites: {count}");

        // Initialize target position
        targetPosition = transform.position;
    }

    void Update()
    {
        float deltaX = player.position.x - lastPlayerX;
        lastPlayerX = player.position.x;

        // Calculate target position for smooth movement
        float backgroundMovement = deltaX * parallaxFactor;
        targetPosition += Vector3.right * backgroundMovement;

        // Smoothly move the layer towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothing);

        // Looping logic using total width
        float loopThreshold = totalWidth / 2f; // Adjust threshold for looping
        foreach (Transform sprite in sprites)
        {
            float spriteLeftEdge = sprite.position.x - spriteWidth / 2f;
            float spriteRightEdge = sprite.position.x + spriteWidth / 2f;

            if (player.position.x - spriteRightEdge > loopThreshold)
            {
                sprite.position += Vector3.right * totalWidth;
            }
            else if (spriteLeftEdge - player.position.x > loopThreshold)
            {
                sprite.position -= Vector3.right * totalWidth;
            }
        }
    }
}