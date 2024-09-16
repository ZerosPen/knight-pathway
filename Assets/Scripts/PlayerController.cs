using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [SerializeField] private int maxHitpoints = 100;
    public int currentHealt;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;
    [SerializeField]private int jumpCount;

    //attack settings
     public Transform attackPoint;
     [SerializeField] private float attackRange;
     public int attackDamage = 5; 
     public LayerMask enemyLayers;

      // Run Attack Cooldown
    public float runAttackCooldown = 2f; // Cooldown time in seconds
    private float lastRunAttackTime;
     
     // Blocking settings
    private bool isBlocking = false;
    public float blockDuration = 1.0f; // Duration the player can block for
    private float blockEndTime;

     //Sprint Settings
    [SerializeField] private float runSpd = 1.5f;
    private bool isRunning = false;

    private Animator animator; // For animations

     // Customizable input keys
     public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode dodgeKey = KeyCode.E;
    public KeyCode blockKey = KeyCode.Q;
    public KeyCode SprintKey = KeyCode.F;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpCount = maxJumps;
        currentHealt = maxHitpoints;
    }

    void Update()
    {
        Move();
        Jump();
        CloseCombat();
        Dodge();
        Block();
    }

//For movement character right and left
    void Move()
    {
       if (isBlocking) return;

        float moveInput = 0f;
        if (Input.GetKey(moveLeftKey))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(moveRightKey))
        {
            moveInput = 1f;
        }

        if (Input.GetKey(SprintKey) && isGrounded && moveInput != 0)
        {
            isRunning = true;
            rb.velocity = new Vector2(moveInput * moveSpeed * runSpd, rb.velocity.y);
            animator.SetBool("isRunning", true);
        }
        else
        {
            isRunning = false;
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            animator.SetBool("isRunning", false);
        }

        animator.SetFloat("walk", Mathf.Abs(moveInput));

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

//For Healpoint Character
    public void PlayertakeDamage(int damage)
    {
        Debug.Log("damage " + damage);
        currentHealt -= damage;

        animator.SetTrigger("hit");

        if (currentHealt <= 0)
        {
            playerDefeat();
        }
    }

//Fliping Character to other side
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

//To able to jumping
    void Jump()
{
    if (isBlocking) return;

    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    if (isGrounded)
    {
        if (jumpCount != maxJumps) // Check if jumpCount needs to be reset
        {
            jumpCount = maxJumps; // Reset jump count when grounded
            Debug.Log("Grounded. Resetting jumpCount."); // Debugging
        }
        animator.SetBool("isGrounded", true);
    }
    else
    {
        animator.SetBool("isGrounded", false);
    }

    if (jumpCount > 0 && Input.GetKeyDown(jumpKey))
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpCount--; // Decrease jump count on each jump
        animator.SetTrigger("Jumping");
        Debug.Log("Jump Trigger Set. JumpCount: " + jumpCount); // Debugging jump count
    }
}

    void CloseCombat()
    {
        if (isBlocking) return;

        if (Input.GetKeyDown(attackKey))
        {
            if (isRunning && Time.time >= lastRunAttackTime + runAttackCooldown)
            {
                animator.SetTrigger("RunAttack");
                PerformRunAttack();
                lastRunAttackTime = Time.time; // Reset cooldown timer
            }
            else
            {
                animator.SetTrigger("Attack");
                PerformAttack();
            }
        }
    }

    void PerformAttack()
    {
        if (isBlocking) return;
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (attackPoint == null)
        {
            Debug.LogError("Attack Point is not assigned!");
            return;
        }

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            
            // Assume the enemy has a script with a method TakeDamage(int damage)
             enemy.GetComponent<EnemyController>().EnemyTakeDamage(attackDamage);
        }
    }

    void PerformRunAttack()
    {
        // Special logic for the run attack (e.g., more damage)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Run Attack Hit "+ enemy.name + attackDamage);
            enemy.GetComponent<EnemyController>().EnemyTakeDamage(attackDamage * 2); // Double damage for a run attack, for example
        }
    }

    void Dodge()
    {
        if (isBlocking) return; // Prevent dodging while blocking

        if (Input.GetKeyDown(dodgeKey))
        {
            animator.SetTrigger("Dodge");
            // Add your dodge logic here (e.g., temporary invincibility or quick dash)
        }
    }

    void Block()
    {
        if (Input.GetKeyDown(blockKey))
        {
            isBlocking = true;
            blockEndTime = Time.time + blockDuration;
            animator.SetBool("isBlocking", true);
        }

        if (isBlocking && Time.time > blockEndTime)
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
        }
    }

    void playerDefeat()
    {
        Debug.Log("player is Die");
        animator.SetBool("IsDead", true);
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
