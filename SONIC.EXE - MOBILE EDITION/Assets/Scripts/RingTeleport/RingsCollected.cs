using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RingsCollected : MonoBehaviour
{
    public PlayerController player;

    public PlayerFollower cameraFollow;

    public string sceneToLoad;

    private Animator animator;

    int specialRingsCount;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(specialRingChecker());
    }

    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator specialRingChecker()
    {

        while (true)
        {

            if (player != null)
            {
                specialRingsCount = player.specialRing;

                if (specialRingsCount == 4)
                {
                    animator.SetBool("ringsCollected", true);

                    if (animator.GetBool("haveTeleported") == true)
                    {

                        yield return new WaitForSeconds(2f);

                        SceneManager.LoadScene(sceneToLoad);
                        
                    }
                }

                yield return new WaitForSeconds(2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // PlayerController player = collision.GetComponent<PlayerController>();


        if (specialRingsCount == 4 && collision.CompareTag("Player"))
        {

            animator.SetBool("haveTeleported", true);

            cameraFollow.isFollowing = false;

            player.gameObject.SetActive(false);

        }
    }



}
