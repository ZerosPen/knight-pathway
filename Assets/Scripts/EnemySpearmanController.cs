using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpearmanController : MonoBehaviour
{
    //Settings for Enemy spearman status
    public int maxHealth = 100;
    public float moveSpd = 2f;
    public float stopDuration = 2f;
    public float chaseSpd = 3.5f;
    public float chaseRange = 8f;
    public float atkRange = 4f;
    public float atkCoolDown = 2.5f;
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;
    public LayerMask PlayerLayer;
    public int currentHealt;

    private Rigidbody2D rb;
    private Animator animator;
    public Transform player;
    private bool isStopped;
    private bool isFacingRight = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private float lastAttackTime;
    [SerializeField] private Transform  waypointsA;
    [SerializeField] private Transform waypointsB;
    private Vector2 targetWaypoint; // The next position to move towards


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currentHealt = maxHealth;
        SetNewWaypointPos();
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
            isChasing = false;
            if(!isStopped)
            {
                MoveToWaypoint();
            }
        }
        FlipSprites();
    }


    void SetNewWaypointPos()
    {
        float randomX = Random.Range(waypointsA.position.x, waypointsB.position.x);
        targetWaypoint = new Vector2(randomX, transform.position.y);

        if((randomX > transform.position.x && !isFacingRight) || (randomX < transform.position.x && isFacingRight))
        {
            Flip();
        } 
    }

    void MoveToWaypoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint, moveSpd * Time.deltaTime);

        if(Vector2.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            isStopped = true;
            Invoke("ResumePartol", stopDuration);
        }
    }

    void ChasePlayer()
    {
        isChasing = true;
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
        rb.velocity = Vector2.zero; //stop any movement

        //wait for atk anim and CoolDown to finish
        yield return new WaitForSeconds(atkCoolDown);
        isAttacking = false;
    }

    void ResumePartol()
    {
        SetNewWaypointPos();
        isStopped = false;
    }

    void FlipSprites()
    {
        if(!isFacingRight && rb.velocity.x < 0)
        {
            Flip();
        }
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
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
