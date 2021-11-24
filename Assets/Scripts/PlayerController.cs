using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    SceneManagement sceneManagement;


    public float moveSpeed;
    public float jumpForce;
    public bool onGround;

    //public float horizontal;

    //private Vector3 localScale;

    private void Start()
    {
        sceneManagement = SceneManagement.Instance;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = true;
            anim.SetBool("Grounded", onGround);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = false;
            anim.SetBool("Grounded", onGround);



        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Goal"))
        {
            sceneManagement.goToWorldMap();
        }
    }

    private void FixedUpdate()
    {
        //do stuff
        float horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxisRaw("Jump");
        float vertical = Input.GetAxisRaw("Vertical");

        if(horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        } else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (vertical > 0.1f || jump > 0.1f)
        {
            if (onGround)
            {
                movement.y = jumpForce;
            }
            
        }

        rb.velocity = movement;


    }


    /*public GameManager theGM;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        moveSpeed = 5f;
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        dirY = Input.GetAxisRaw("Vertical") * moveSpeed;

        AnimationControl();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(dirX, dirY);
    }
    private void LateUpdate()
    {
        if (rb.velocity.x>0)
        {
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
        }
    }
    private void AnimationControl()
    {
        if(rb.velocity.y == 0 && rb.velocity.x == 0)
        {
            anim.Play("IdleAnim");
        }
        if(rb.velocity.x !=0 || rb.velocity.y !=0)
        {
            anim.Play("RunAnim");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }
    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        theGM.GameOver();
    }*/

}
