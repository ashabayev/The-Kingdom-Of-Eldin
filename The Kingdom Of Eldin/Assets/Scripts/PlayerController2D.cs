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
    private int manaCount;
    private int increase;
    private int maxManaCount;
    public Text manaCountText;
    public Vector3 startPosition;
    SpriteRenderer spriteRenderer;
    PlayerHealth health;

    //can only jump if grounded
    bool isGrounded;
    //can't attack again while attacking
    bool isAttacking;
    //can't move but will still rotate
    bool isBlocking;
    //can't move normally for the duration
    bool isDodging;

    bool isDead;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    Transform groundCheckL;
    [SerializeField]
    Transform groundCheckR;

    [SerializeField]
    Transform attackRotation;
    [SerializeField]
    Transform attackPos;

    [SerializeField]
    private float runSpeed = 3;

    [SerializeField]
    private float jumpSpeed = 5;

    float lastRunSpeed;
    float lastJumpSpeed;

    [SerializeField]
    private float knightBasicAttackSpeed = .3f;
    [SerializeField]
    private float knightBasicAttackRange = 1f;

    // Start is called before the first frame update
    void Start()
    {
        arrowCount = 0;
        maxManaCount = 100;
        manaCount = 100;
        increase = 25;
        isAttacking = false;
        SetArrowCountText();
        SetManaCountText();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        startPosition = transform.position;
        print(startPosition);
    }

    void Death()
    {
        isDead = true;
        // playerControl.enabled = false;
        transform.position = startPosition;
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
            attackRotation.transform.eulerAngles = new Vector3(0, 0, 0);

        }
        else if(Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
            if (isGrounded)
            {
                //animator.Play("Player_run");
            }
            spriteRenderer.flipX = true;
            attackRotation.transform.eulerAngles = new Vector3(0, 180, 0);
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
            if (isAttacking == false)
            {
                isAttacking = true;
                BasicAttack();
            }
        }

        //special ability
        //c is a temporary mapping
        if (Input.GetKeyDown("c"))
        {
            SpecialAbility();
        }

        //block and charge are hold abilities, after being pressed they wait for a release here
        if (Input.GetKeyUp("c") && isBlocking)
        {
            //release block
            isBlocking = false;
            jumpSpeed = lastJumpSpeed;
            runSpeed = lastRunSpeed;
            Debug.Log("release block");
        }
        //if (Input.GetKeyUp("c") && isCharging)
        //{
        //    //release charge
        //}
        if (Input.GetKey("q"))
        {
            if (manaCount > 0)
            {
                manaCount--;
                SetManaCountText();
            }
            dropArrows();
        }
        if (rb2d.position.y < -50)
        {
            Death();
        }
        if (health.currentHealth <= 0)
        {
            Death();
            health.currentHealth = health.startingHealth;
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

    void dropArrows()
    {
        if (arrowCount > 0)
        {
            arrowCount--;
            SetArrowCountText();
        }
    }

    void SetArrowCountText()
    {
        arrowCountText.text = "Arrows: " + arrowCount.ToString();
    }
    void SetManaCountText()
    {
        manaCountText.text = "Mana: " + manaCount.ToString() + "/100";
    }

    void BasicAttack()
    {
        //determine which attack to use

        //if knight
        IEnumerator knightAttack = MeleeBasicAttack(knightBasicAttackSpeed, knightBasicAttackSpeed);
        StartCoroutine(knightAttack);

        //if rogue
        //IEnumerator rogueAttack = MeleeBasicAttack(rogueBasicAttackSpeed, rogueBasicAttackSpeed);
        //StartCoroutine(rogueAttack);

        //if ranger

        //if wizard
        //IEnumerator wizardAttack = MeleeBasicAttack(wizardBasicAttackSpeed, wizardBasicAttackSpeed);
        //StartCoroutine(wizardAttack);
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

    void SpecialAbility()
    {
        //determine which ability to use

        //check for knight

        //check for already blocking
        if (isBlocking == false)
        {
            isBlocking = true;
            lastJumpSpeed = jumpSpeed;
            lastRunSpeed = runSpeed;
            jumpSpeed = 0;
            runSpeed = 0;
            Debug.Log("block start");
        }

        //if rogue - dodge

        //if ranger - charge

        //if wizard - spell

    }

    //this is to make seeing the attack hitbox easy in the inspector, but you need to have the player selected to see it
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, knightBasicAttackRange);
    }
}
