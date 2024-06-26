using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats gm;
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
    public MeleeHitbox meleeHitbox;
    public bool isDead;



    private void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        animator = GetComponent<Animator>();
        canDash = true;
        speed = gm.moveSpeed;
    }

    private void Update()
    {
        if(isDead)
        {
            speed = 0;
        }
        else
        {
            speed = gm.moveSpeed;
        }

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
        HandleAttacks();
    }


    private void TakeInput()
    {
        direction = Vector2.zero;
        bool isMoving = false;
        bool isRunningDiagonally = false;

        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.up;
            isMoving = true;

            
            if (Input.GetKey(KeyCode.A)) //Up and Left
            {
                isRunningDiagonally = true;
            }

            if (Input.GetKey(KeyCode.D)) //Up and Right
            {
                isRunningDiagonally = true;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.left;
            isMoving = true;

            if (Input.GetKey(KeyCode.W)) //Left and Up
            {
                isRunningDiagonally = true;
            }

            if (Input.GetKey(KeyCode.S)) //Left and Down
            {
                isRunningDiagonally = true;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.down;
            isMoving = true;

            if (Input.GetKey(KeyCode.A)) //Down and Left
            {
                isRunningDiagonally = true;
            }

            if (Input.GetKey(KeyCode.D)) //Down and Right
            {
                isRunningDiagonally = true;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isIdle", false);
            direction += Vector2.right;
            isMoving = true;

            if (Input.GetKey(KeyCode.W)) //Right and Up
            {
                isRunningDiagonally = true;
            }

            if (Input.GetKey(KeyCode.S)) //Right and Down
            {
                isRunningDiagonally = true;
            }
        }

        //diagonal movement
        animator.SetBool("isRunningDiagonally", isRunningDiagonally);

        if (!isMoving && !Input.GetMouseButton(0))
        {
            animator.SetBool("isIdle", true);
        }

        if (!isDashing && Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
            animator.SetBool("isIdle", false);
        }
        //attack method moved below
    }

    private void HandleAttacks()
    {
        // Check if the attack button (mouse button) is being pressed or released
        if (Input.GetMouseButton(0))
        {
            animator.SetBool("isAttacking", true); // Start attacking
        }
        else if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
        {
            animator.SetBool("isAttacking", false); // Ensure attacking stops when button is released or not pressed
        }
    }

    public void EndAttack()
    {
        meleeHitbox.ResetDamagedEnemies();
        animator.SetBool("isAttacking", false); // Ensure animation stops when attack ends
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

        rb.velocity = direction * dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
