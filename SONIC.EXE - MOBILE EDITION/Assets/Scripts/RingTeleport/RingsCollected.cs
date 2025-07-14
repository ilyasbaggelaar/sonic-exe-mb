using System.Collections;
using UnityEngine;

public class RingsCollected : MonoBehaviour
{
    public PlayerController player;

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

                    if (animator.GetBool("haveTeleported") == true) {
                        
                    }
                }

                yield return new WaitForSeconds(2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (specialRingsCount == 4)
        {
            animator.SetBool("haveTeleported", true);
        }
    }



}
