using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    private bool movingRight = true;
    public Transform groundDetection;
    public Transform wallDetection;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("IsWalking", true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        RaycastHit2D wallInfo;
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, .5f);
        
        if (movingRight){
            wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, .5f);
        }
        else
        {
            wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.left, .5f);
        }
  
        
        if (groundInfo.collider == null || wallInfo.collider.gameObject.tag == "Ground" || wallInfo.collider.gameObject.tag == "Enemy")
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }
}
