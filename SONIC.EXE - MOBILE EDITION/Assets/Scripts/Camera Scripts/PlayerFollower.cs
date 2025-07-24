using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform player;

    public bool isFollowing = true;

    void Update()
    {
        if (isFollowing == true)
        {
                    transform.position = player.transform.position + new Vector3(0, 0, -10);
        }
    }
}
