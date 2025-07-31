using UnityEngine;

public class PlayerFollowerRunning : MonoBehaviour
{
    public Transform player;

    public bool isFollowing = true;

    public float offAxis = 5f;

    void Update()
    {
        if (isFollowing == true)
        {
                    transform.position = player.transform.position + new Vector3(offAxis, 0, -10);
        }
    }
}
