using UnityEngine;

public class SideScrollerLevel : MonoBehaviour
{

    public float speed = 16f;
   public  bool pushLevel = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    
    }

    void FixedUpdate()
    {
        if (pushLevel)
        {
                    transform.position += Vector3.left * speed * Time.deltaTime;
        }

    }

}
