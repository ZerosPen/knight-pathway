using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float JumpHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask stairLayer;
    // private bool IsOnStair = false;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float WallJumpDelay;
    private float horizontalInput;


    // Start is called before the first frame update
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        if (Input.GetKey(KeyCode.Space) && !isGrounded())
        {
            Jump();
        }
        //Flip char
        if (horizontalInput > 0.1f)
        {
            transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        }
        else if (horizontalInput < -0.1f)
        {
            transform.localScale = new Vector3(-3.5f, 3.5f, 3.5f);
        }
        //Wall
        if (WallJumpDelay > 0.02f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if (OnWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 1;
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
            WallJumpDelay += Time.deltaTime;

        //Stairs
        // if (OnStair() != null)
        // {
        //     IsOnStair = true;
        //     if (Input.GetKey(KeyCode.D))
        //     {
        //         body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //     }
        //     else if (Input.GetKey(KeyCode.A))
        //     {
        //         body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //     }
        //}

        //Set Animation Parameters
        anim.SetBool("walk", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());


    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, JumpHeight);
            anim.SetTrigger("Jump");
        }
        else if (OnWall() && isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector2(-Mathf.Sign(transform.localScale.x), transform.localScale.y);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            WallJumpDelay = 0;
        } 
    }
    //onTheGround?
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    //onTheWall??
    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    //onTheStair??
    private bool OnStair()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, stairLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !OnWall();
    }
}
