using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 targetPos;
    public float speed;
    public Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    public float dashRange;
    private enum Facing { UP, DOWN, LEFT, RIGHT};
    //private Facing FacingDir = Facing.DOWN;
    public static bool isDashing;
    public static bool canDash = true;
    public float dashSpeed = 10f;
    public float dashDuration = 1f;
    public static float dashCooldown = 1f;
    public static float dashStartTime;


    private void Start()
    {
        animator = GetComponent<Animator>();
        canDash = true;
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(direction.x * speed, direction.y * speed);

        direction = new Vector2(moveX, moveY).normalized;

        TakeInput();
        SetAnimatorMovement(direction);
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
        }
        */
    }

    private void FixedUpdate()
    {
        //put stuff in here to optimize for release
    }

    private void TakeInput()
    {
        direction = Vector2.zero;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.up;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.left;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.down;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.right;
            isMoving = true;
        }

        if (!isMoving && !Input.GetMouseButton(0))
        {
            animator.SetBool("isIdle", true);
        }

        if (!isDashing && Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
            animator.SetBool("isIdle", false);
        }

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("isAttacking", true); // Start attacking
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("isAttacking", false); // Stop attacking
        }
    }

    private void SetAnimatorMovement(Vector2 direction)
    {
        animator.SetFloat("xDir", direction.x);
        animator.SetFloat("yDir", direction.y);
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashStartTime = Time.time;
        rb.velocity = new Vector2(direction.x * dashSpeed, direction.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
