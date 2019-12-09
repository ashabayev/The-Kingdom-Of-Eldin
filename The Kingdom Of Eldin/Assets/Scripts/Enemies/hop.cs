using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hop : MonoBehaviour
{
    bool isGrounded;
    Rigidbody2D rb2d;
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    private float jumpSpeed = 5;
    private bool movingRight = true;
    public float speed = 1;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
    }



    // Update is called once per frame
    void Update()
    {
        if (rb2d.velocity.x >= 0.01f)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1f, 1f, 1f));
        }
        else if (rb2d.velocity.x <= -0.01f)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1f, 1f, 1f));
        }

    }

    void FixedUpdate()
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, .1f);
        //if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        if (groundInfo.collider.gameObject.tag == "Ground" || groundInfo.collider.gameObject.tag == "Enemy")
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            //animator.Play("Enemy_jump");
        }


           
        //jumping
        if ((isGrounded))
        {
            rb2d.velocity = new Vector2(0, 0);
            rb2d.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            //animator.Play("Player_jump");
        }
    }


}
