using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 targetPos;
    public float speed;
    private Vector2 direction;
    private Animator animator;
    public float dashRange;
    private enum Facing { UP, DOWN, LEFT, RIGHT};
    private Facing FacingDir = Facing.DOWN;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        TakeInput();
        Move();
        /* PART OF THE OLD MELEE SYSTEM, DONT USE
        if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Attack());
            }
        */
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        SetAnimatorMovement(direction);
    }

    private void TakeInput()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
            FacingDir = Facing.UP;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
            FacingDir = Facing.LEFT;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
            FacingDir = Facing.DOWN;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
            FacingDir = Facing.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 currentPos = transform.position;
            targetPos = Vector2.zero;
            if(FacingDir == Facing.UP)
            {
                targetPos.y = 1;
            }
            else if (FacingDir == Facing.DOWN)
            {
                targetPos.y = -1;
            }
            else if(FacingDir == Facing.LEFT)
            {
                targetPos.x = -1;
            }
            else if(FacingDir == Facing.RIGHT)
            {
                targetPos.x = 1;
            }
            transform.Translate(targetPos);
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

    /* PART OF THE OLD MELEE SYSTEM, DONT USE
    IEnumerator Attack()
    {
        animator.SetBool("isAttacking", true); // Start attacking

        // Wait for a split second
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("isAttacking", false); // Stop attacking

        // Ensure isAttacking is set to false, since the coroutine will finish before setting it back
        //isAttacking = false;
    }
    */
}
