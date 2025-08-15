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

    public string nextLevelString;

    public int levelIndex = 1;

    private PlayerFollower playerFollower;

    private KnucklesPlayerController knucklesPlayer;
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

            knucklesPlayer = collision.gameObject.GetComponent<KnucklesPlayerController>();


            if (playerFollower != null)
            {
                playerFollower.isFollowing = false;
            }



            animator.SetBool("isFinished", true);

            if (player != null)
            {
                Debug.Log("finishing level from Sonic's player controller...");
                player.enabled = false;
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

                playerRb.linearVelocity = new Vector2(5f, playerRb.linearVelocityY);

                Animator playerAnimator = player.GetComponent<Animator>();

                playerAnimator.SetBool("isRunning", true);
                StartCoroutine(player.VolumefadeOut());
            }

            else if (knucklesPlayer != null)
            {
                Debug.Log("finishing level from Knuckle's player controller...");

                knucklesPlayer.enabled = false;
                Rigidbody2D knucklesRb = knucklesPlayer.GetComponent<Rigidbody2D>();
                knucklesRb.linearVelocity = new Vector2(5f, knucklesRb.linearVelocityY);

                Animator knucklesAnimator = knucklesPlayer.GetComponent<Animator>();

                knucklesAnimator.SetBool("isRunning", true);
                StartCoroutine(knucklesPlayer.VolumefadeOut());
            }


            endSound.Play();



            StartCoroutine(SwitchScene());
        }

    }

    IEnumerator SwitchScene()
    {

        SaveManager.UnlockNextLevel(levelIndex);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(nextLevelString);
    }
}
