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
    private int jobID;
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

    [SerializeField]
    private float knightBasicAttackSpeed = .3f;
    [SerializeField]
    private float knightBasicAttackRange = 1f;
    [SerializeField]
    private float rogueBasicAttackSpeed = .2f;
    [SerializeField]
    private float rogueBasicAttackRange = .7f;

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
        jobID = 1;
        SetArrowCountText();
        SetManaCountText();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        EndGamePanel.SetActive(false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
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
        animator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));



        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
        else
        {
            isGrounded = false;
        }

        //horizontal movement
        if ((Input.GetKey("d") || Input.GetKey("right")))
        {
            if (canMove)
            {
                rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                }
            }
            spriteRenderer.flipX = true;
            attackRotation.transform.eulerAngles = new Vector3(0, 0, 0);

        }
        else if ((Input.GetKey("a") || Input.GetKey("left")))
        {
            if (canMove)
            {
                rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                }
            }
            spriteRenderer.flipX = false;
            attackRotation.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            if (isGrounded)
            {
            }
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        //jumping
        if ((Input.GetKey("space") && isGrounded) & !isBlocking)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            animator.SetBool("IsJumping", true);
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
            animator.SetBool("IsBlocking", false);
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

        //job changing
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            jobID = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            jobID = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            jobID = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            jobID--;
            if (jobID < 1)
            {
                jobID = 3;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            jobID++;
            if (jobID > 3)
            {
                jobID = 1;
            }
        }
        animator.SetInteger("JobID", jobID);

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
            spriteRenderer.gameObject.SetActive(false);
            animator.gameObject.SetActive(false);
            rb2d.gameObject.SetActive(false);
        }
    }

    string getCurrentJob()
    {
        switch (jobID)
        {
            case 1:
                Debug.Log("job set to knight");
                return "knight";
            case 2:
                Debug.Log("job set to rogue");
                return "rogue";
            case 3:
                Debug.Log("job set to ranger");
                return "ranger";
            default:
                Debug.Log("current job error - jobID currently set to: " + jobID);
                return "knight";
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
        switch (getCurrentJob())
        {
            case "knight":
                IEnumerator knightAttack = MeleeBasicAttack(knightBasicAttackSpeed, knightBasicAttackSpeed);
                StartCoroutine(knightAttack);
                break;
            case "rogue":
                IEnumerator rogueAttack = MeleeBasicAttack(rogueBasicAttackSpeed, rogueBasicAttackSpeed);
                StartCoroutine(rogueAttack);
                break;
            case "ranger":
                break;
            default:
                Debug.Log("current job error - basic attack");
                break;
        }
    }

    IEnumerator MeleeBasicAttack(float attackSpeed, float attackRange)
    {
        //if we're cool we wait until the windup is done here
        animator.SetBool("IsAttacking", true);
        //check for enemies to damage all at once because we're lazy
        Collider2D[] damagedEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, 1 << LayerMask.NameToLayer("Enemy"));
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
        animator.SetBool("IsAttacking", false);
        Debug.Log("attack successful");
    }

    void SpecialAbility()
    {
        switch (getCurrentJob())
        {
            case "knight":
                if (isBlocking == false)
                {
                    isBlocking = true;
                    canMove = false;
                    animator.SetBool("IsBlocking", true);
                }
                break;
            case "rogue":
                if (DashOnCooldown == false)
                {
                    isDashing = true;
                    DashOnCooldown = true;
                    IEnumerator rogueDash = Dash(dashSpeed, dashDuration, dashCoolDown);
                    StartCoroutine(rogueDash);
                }
                break;
            case "ranger":
                break;
            default:
                Debug.Log("current job error - special ability");
                break;
        }
    }

    IEnumerator Dash(float speed, float duration, float cooldown)
    {
        canMove = false;
        animator.SetBool("IsDodging", true);

        float direction = 1f;
        if (spriteRenderer.flipX == false)
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
        animator.SetBool("IsDodging", false);

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
