using UnityEngine;

public class ParallaxFollower : MonoBehaviour
{
    public float parallaxSpeed = 5f; // How fast the background scrolls (relative to level)
    public float loopThresholdX = -15f; // X position at which to loop
    public int spriteCount = 8; // Number of child sprites
    public float spriteWidth = 3.8f; // Width of each sprite (match spacing in world units)

    private Transform[] sprites;

    void Start()
    {
        // Cache all child sprite transforms
        sprites = new Transform[spriteCount];
        for (int i = 0; i < spriteCount; i++)
        {
            sprites[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        // Scroll the entire parent left
        transform.position += Vector3.left * parallaxSpeed * Time.deltaTime;

        // Check if the first (leftmost) sprite is beyond the threshold
        Transform first = sprites[0];
        if (first.position.x + spriteWidth < loopThresholdX)
        {
            // Move it to the end of the last sprite
            Transform last = sprites[spriteCount - 1];
            first.position = new Vector3(last.position.x + spriteWidth, first.position.y, first.position.z);

            // Shift array: move first to last
            for (int i = 0; i < spriteCount - 1; i++)
            {
                sprites[i] = sprites[i + 1];
            }
            sprites[spriteCount - 1] = first;
        }
    }
}
