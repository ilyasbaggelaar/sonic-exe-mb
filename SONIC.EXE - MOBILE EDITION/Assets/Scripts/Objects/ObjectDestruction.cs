using UnityEngine;
using System.Collections;

public class ObjectDestruction : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D objectBox;
    public GameObject[] plankPrefabs;

    public AudioSource woodBreak;


    public int planksToSpawn = 4;

    public float torque = 5f;

    public float pieceLifeTime = 3f;
    public float force = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        objectBox = GetComponent<BoxCollider2D>();
        woodBreak.Stop();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BoxCollider2D box = collision.gameObject.GetComponent<BoxCollider2D>();

            if (box != null && box.enabled)
            {

                StartCoroutine(BoxDestroyed());
            }
        }
    }

    private IEnumerator BoxDestroyed()
    {
        PlayWoodBreakSound();
       // AudioSource.PlayClipAtPoint(woodBreak.clip, transform.position);
        objectBox.enabled = false;
        animator.SetBool("destroyed", true);
        SpawnPlanks();
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
    private void PlayWoodBreakSound()
    {
        GameObject temp = new GameObject("TempAudio");
        AudioSource tempAudio = temp.AddComponent<AudioSource>();

        tempAudio.clip = woodBreak.clip;
        tempAudio.volume = woodBreak.volume;
        tempAudio.pitch = woodBreak.pitch;
        tempAudio.spatialBlend = woodBreak.spatialBlend;

        tempAudio.Play();
        Destroy(temp, tempAudio.clip.length);
    }

    private void SpawnPlanks()
    {
        for (int i = 0; i < planksToSpawn; i++)
        {
            if (plankPrefabs.Length == 0) return;

            GameObject plankPrefab = plankPrefabs[Random.Range(0, plankPrefabs.Length)];

            GameObject plank = Instantiate(plankPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = plank.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection * force, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
            }

            Destroy(plank, pieceLifeTime);
        }
    }
}

