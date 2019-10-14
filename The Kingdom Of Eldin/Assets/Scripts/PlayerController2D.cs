using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController2D : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    private int arrowCount;
    public Text arrowCountText;
    SpriteRenderer spriteRenderer;

    bool isGrounded;
    bool isAttacking;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    Transform groundCheckL;
    [SerializeField]
    Transform groundCheckR;
    [SerializeField]
    Transform attackPos;

    [SerializeField]
    private float runSpeed = 3;

    [SerializeField]
    private float jumpSpeed = 5;

    [SerializeField]
    private float knightBasicAttackSpeed = .3f;
    [SerializeField]
    private float knightBasicAttackRange = 1f;

    // Start is called before the first frame update
    void Start()
    {
        arrowCount = 0;
        isAttacking = false;
        SetArrowCountText();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            //animator.Play("Player_jump");
        }
        
        //horizontal movement
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
            if (isGrounded)
            {
                //animator.Play("Player_run");
            }
            spriteRenderer.flipX = false;
        }
        else if(Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
            if (isGrounded)
            {
                //animator.Play("Player_run");
            }
            spriteRenderer.flipX = true;
        }
        else
        {
            if (isGrounded)
            {
                //animator.Play("Player_idle");
            }
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        //jumping
        if (Input.GetKey("space") && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            //animator.Play("Player_jump");
        }

        //basic attack
        //x is a temporary mapping
        if (Input.GetKey("x"))
        {
            if(isAttacking == false)
            {
                isAttacking = true;
                BasicAttack();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            arrowCount++;
            SetArrowCountText();
        }
    }

    void SetArrowCountText()
    {
        arrowCountText.text = "Arrows: " + arrowCount.ToString();
    }

    void BasicAttack()
    {
        //determine which attack to use

        //knight
            //actual game logic
        IEnumerator attackRout = MeleeBasicAttack(knightBasicAttackSpeed, knightBasicAttackSpeed);
        StartCoroutine(attackRout);

            //play appropriate animation

        //rogue

        //ranger

        //wizard
    }

    IEnumerator MeleeBasicAttack(float attackSpeed, float attackRange)
    {
        //if we're cool we wait until the windup is done here

        //check for enemies to damage all at once because we're lazy
        Collider2D[] damagedEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, LayerMask.NameToLayer("Enemy"));
        foreach(Collider2D enemyCollider in damagedEnemies)
        {
            //tell the enemy script that the enemy takes damage

            //for testing purposes
            if(enemyCollider.gameObject.tag == "Enemy")
            {
                Debug.Log("enemy damaged");
            }
        }

        //attack recovery
        yield return new WaitForSeconds(attackSpeed);

        //wrap up
        isAttacking = false;
        Debug.Log("attack successful");
    }

    //this is to make seeing the attack hitbox easy in the inspector, but you need to have the player selected to see it
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, knightBasicAttackRange);
    }
}
