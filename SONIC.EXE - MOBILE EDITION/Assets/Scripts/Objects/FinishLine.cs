using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishLine : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;
    private Rigidbody2D rb;
    public AudioSource endSound;

    public int levelIndex = 1;

    private PlayerFollower playerFollower;
    void Start()
    {
        animator = GetComponent<Animator>();


        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            playerFollower = mainCamera.GetComponent<PlayerFollower>();
        }

        endSound.Stop();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            

            if (playerFollower != null)
            {
                playerFollower.isFollowing = false;
            }

            animator.SetBool("isFinished", true);

            if (player != null)
            {
                player.enabled = false;
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

                playerRb.linearVelocity = new Vector2(5f, playerRb.linearVelocityY);

                Animator playerAnimator = player.GetComponent<Animator>();

                playerAnimator.SetBool("isRunning", true);
                            StartCoroutine(player.VolumefadeOut());
            }
            

            endSound.Play();



            StartCoroutine(SwitchScene());
        }

    }

    IEnumerator SwitchScene()
    {

        SaveManager.UnlockNextLevel(levelIndex);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainMenu");
    }
}
