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
    public float atkRange = 1f;
    public float atkCoolDown = 2f;
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
        if(isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= atkRange)
        {
            StartCoroutine(Attack());
        }
        else if(distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("isRunning", false);
            isChasing = false;

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
        isChasing = true;
        animator.SetBool("isRunning", true);

        if(player.position.x > transform.position.x)
        {
            rb.velocity = new Vector2(chaseSpd, rb.velocity.y);
            if(!isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            rb.velocity = new Vector2(-chaseSpd, rb.velocity.y);
            if(isFacingRight)
            {
                Flip();
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // to stop any movement to able to atk
        // animator.SetBool("Attack");
        
        // Wait for the attack animation and cooldown to finish
         yield return new WaitForSeconds(atkCoolDown);
        isAttacking = false;
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
        Debug.Log("Enemy Die");
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
