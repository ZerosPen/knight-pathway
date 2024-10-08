using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //settings enemy
    public int maxHealth = 100;
    public float moveSpd = 2f;
    public float beforeFinish = 5f;
    public float stopDuration =3f;
    public float chaseSpd = 3.5f;
    public float chaseRange  = 5f;
    public int atkdamage = 50;
    public float atkRange = 1f;
    public float atkCoolDown = 2f;
    public Transform attackPoint;
    public Transform leftBoundary;
    public Transform rightBoundary;
    public LayerMask playerLayer;
    public int currentHealt;

    private Rigidbody2D rb;
    private Animator animator;
    public Transform player;
    private bool isFacingRight = true;
    private bool isChasing = false;
    private bool isStopped = false;
    private bool isAttacking = false;
    private float lastAttackTime;
    [SerializeField] private Transform  waypointsA;
    [SerializeField] private Transform waypointsB;
    private Vector2 targetPosition; // The next position to move towards


    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Optional: Attempt to find the player by tag if not set in the Inspector
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currentHealt = maxHealth;
        SetNewTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= chaseRange && distanceToPlayer >= atkRange)
        {
            
            isChasing = true;
            ChasePlayer();
        }
        if (distanceToPlayer <= atkRange && Time.time >= lastAttackTime + atkCoolDown)
        {
            //animator.SetBool("IsRunning", false);
            //animator.SetBool("walk", false);
            isAttacking = true;
            StartCoroutine(PerformAttack());
            lastAttackTime = Time.time; // Reset attack cooldown
        }
        else
        {
            animator.SetBool("isRunning", false);
            isChasing = false;
            isAttacking = false;

            if (!isStopped)
            {
                MoveTowardsTarget();
            }
        }
        FlipSprite();
    }

    //to set new waypoint to the enemy go to that position 
    void SetNewTargetPosition()
    {
        float randomX = Random.Range(waypointsA.position.x, waypointsB.position.x);
        targetPosition = new Vector2(randomX, transform.position.y);

        if ((randomX > transform.position.x && !isFacingRight) || (randomX < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }
    //make the enemy gi toward to waypoint but have random stop in range between in Range waypoint A waypoint B
    void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpd * Time.deltaTime);
        animator.SetBool("walk", true);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isStopped = true;
            animator.SetBool("walk", false);
            Invoke("ResumePatrol", stopDuration);
        }
    }
    //when in the range of chase enemy will chase the player
    void ChasePlayer()
    {
        if (isChasing == true)
        {
            animator.SetBool("isRunning", true);

            Vector2 direction = (player.position - transform.position).normalized;
            if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
            {
                Flip();
            }
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpd * Time.deltaTime);
        }
    }

    IEnumerator PerformAttack()
    {
        rb.velocity = Vector2.zero; // to stop any movement to able to atk
        if (isAttacking == true)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("walk", false);
            animator.SetTrigger("Attack");
            AtkPlayer();
            Vector2 direction = (player.position - transform.position).normalized;
            if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
            {
                Flip();
            }
        }

        // Wait for the attack animation and cooldown to finish
        yield return new WaitForSeconds(atkCoolDown);
        isAttacking = false;
    }

    public void AtkPlayer()
    {
        Collider2D[] hitplayers = Physics2D.OverlapCircleAll(attackPoint.position, atkRange, playerLayer);

        if (attackPoint == null)
        {
            Debug.LogError("Attack Point is not assigned!");
            return;
        }

        foreach (Collider2D collider in hitplayers)
        {
            player.GetComponent<PlayerController>().PlayertakeDamage(Random.Range(5, 50));
        }

    }

    void ResumePatrol()
    {
        SetNewTargetPosition();
        isStopped = false;
        animator.SetBool("walk", true);
    }
    
    void FlipSprite()
    {
        // This will flip the sprite depending on the direction the enemy is facing
        // left
        if (!isFacingRight && rb.velocity.x < 0)
        {
            Flip();
        }
        //right
        else if (isFacingRight && rb.velocity.x > 0)
        {
            Flip();
        }
    }

    void Flip()
    {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
    }


    public void EnemyTakeDamage(int damage)
    {
        currentHealt -= damage;
       
        animator.SetTrigger("Hit");

        if (currentHealt <= 0)
        {
            EnemyDefeat();
        }
    }

    void EnemyDefeat()
    {
        animator.SetBool("IsDead", true);
        //wannt make the enemy still in ground for around 5s then drop/rb is false;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
