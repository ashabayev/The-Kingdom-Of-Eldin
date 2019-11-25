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
    private float regenMana;
    private float manaPerSecond;
    private int maxManaCount;
    public Text manaCountText;
    public Vector3 startPosition;
    SpriteRenderer spriteRenderer;
    PlayerHealth health;
    public GameObject EndGamePanel;

    //can only jump if grounded
    bool isGrounded;
    //can't attack again while attacking
    bool isAttacking;
    //can't move but will still rotate
    public static bool isBlocking;
    //can't move normally for the duration
    public static bool isDashing;
    //dash currently not useable
    bool DashOnCooldown;
    //can move normally, would be false to prohibit normal movement during something like a dash or block
    bool canMove;

    bool isDead;
    int damagePerBasicHit = 50;

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

    //special ability values
    [SerializeField]
    private float dashSpeed = 6f;
    [SerializeField]
    private float dashDuration = .6f;
    [SerializeField]
    private float dashCoolDown = .3f;

    // Start is called before the first frame update
    void Start()
    {
        arrowCount = 0;
        regenMana = 0;
        maxManaCount = 100;
        manaPerSecond = 1f;
        manaCount = 100;
        isAttacking = false;
        canMove = true;
        SetArrowCountText();
        SetManaCountText();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        EndGamePanel.SetActive(false);
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
        if ((Input.GetKey("d") || Input.GetKey("right")))
        {
            if (canMove)
            {
                rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                    //animator.Play("Player_run");
                }
            }
            spriteRenderer.flipX = false;
            attackRotation.transform.eulerAngles = new Vector3(0, 0, 0);

        }
        else if ((Input.GetKey("a") || Input.GetKey("left")))
        {
            if (canMove)
            {
                rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                    //animator.Play("Player_run");
                }
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
        if ((Input.GetKey("space") && isGrounded) & !isBlocking)
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
            canMove = true;
            Debug.Log("release block");
        }
        //if (Input.GetKeyUp("c") && isCharging)
        //{
        //    //release charge
        //}
        if (Input.GetKey("q"))
        {
            depleteMana();
            dropArrows();
        }
        else
        {
            replenishMana();
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
    void depleteMana()
    {
        if (manaCount > 0)
        {
            manaCount--;
            SetManaCountText();
        }
    }
    void replenishMana()
    {
        regenMana += Time.deltaTime * manaPerSecond;
        if (regenMana >= 1)
        {
            int floor = Mathf.FloorToInt(regenMana);
            regenMana = 0;
            if (manaCount >= 0 && manaCount < maxManaCount)
            {
                manaCount += floor;
                Mathf.Clamp(manaCount + floor, 0, maxManaCount);
                SetManaCountText();
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
        if (other.gameObject.CompareTag("Door"))
        {
            EndGamePanel.SetActive(true);
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
        foreach (Collider2D enemyCollider in damagedEnemies)
        {
            //tell the enemy script that the enemy takes damage

            //for testing purposes
            if (enemyCollider.gameObject.tag == "Enemy")
            {
                Debug.Log("enemy damaged");
                EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();

                enemyHealth.TakeDamage(damagePerBasicHit);
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
            canMove = false;
            Debug.Log("block start");
        }

        //if rogue - dodge
        //if (DashOnCooldown == false)
        //{
        //    isDashing = true;
        //    DashOnCooldown = true;
        //    IEnumerator rogueDash = Dash(dashSpeed, dashDuration, dashCoolDown);
        //    StartCoroutine(rogueDash);
        //}

        //if ranger - charge

        //if wizard - spell

    }

    IEnumerator Dash(float speed, float duration, float cooldown)
    {
        canMove = false;

        float direction = 1f;
        if (spriteRenderer.flipX == true)
        {
            direction = -1f;
        }

        float timePassed = 0;
        while (timePassed < duration)
        {
            rb2d.AddForce(new Vector2(speed * direction, 0));
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //dash over
        canMove = true;
        isDashing = false;

        //cooldown period
        yield return new WaitForSeconds(cooldown);
        DashOnCooldown = false;
    }

    //this is to make seeing the attack hitbox easy in the inspector, but you need to have the player selected to see it
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, knightBasicAttackRange);
    }
}
