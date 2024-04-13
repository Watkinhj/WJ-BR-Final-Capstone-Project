using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public bool isRangedEnemy;
    public float damage = 1;
    public float moveSpeed = 1000;
    public DetectionZone detectionZone;
    public Rigidbody2D rb;
    public float moveStopDuration = 1f;
    bool inHitStun = false;
    bool isInRange;
    bool finishedAttack;

    private Animator animator;

    private Coroutine damageCoroutine; // Store the coroutine instance

    public NavMeshAgent navMeshAgent;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void FixedUpdate()
    {
        if (navMeshAgent != null)
        {
            if (detectionZone.detectedObjs.Count > 0)
            {
                if (!inHitStun) //If Enemy is not stunned, move towards them
                {
                    Transform playerTransform = detectionZone.detectedObjs[0].transform; // Assuming the first detected object is the player

                    Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
                    if (navMeshAgent != null)
                    {
                        navMeshAgent.SetDestination(playerPosition);

                        navMeshAgent.speed = moveSpeed;
                    }

                    float horizontalMovement = navMeshAgent.velocity.x;
                    animator.SetFloat("xDir", horizontalMovement);

                    if (isRangedEnemy) //if it's a ranged enemy, activate the firing projectile coroutine from RangedEnemyBehavior
                    {
                        RangedEnemyBehavior rangedEnemyBehavior = GetComponent<RangedEnemyBehavior>();
                        if (rangedEnemyBehavior != null)
                        {
                            StartCoroutine(rangedEnemyBehavior.ShootCooldown());
                        }
                        else
                        {
                            Debug.LogError("RangedEnemyBehavior component not found on this GameObject!");
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            if (isInRange)
            {
                if (animator != null)
                {
                    Debug.Log("Running attack anim");
                    animator.SetBool("isAttacking", true);
                }
                PlayerStats.playerStats.DealDamage(damage);
                yield return new WaitForSeconds(0.25f);
                if (animator != null)
                {
                    animator.SetBool("isAttacking", false);
                }
            }
            yield return new WaitForSeconds(1f);
            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isRangedEnemy) //check and see if the enemy is a ranged enemy
        {
            if (collision.tag == "PlayerHurtbox")
            {
                //Debug.Log("Entered PlayerHurtbox")

                isInRange = true;
                // Start the coroutine if it is not running
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(DamageCoroutine());

                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHurtbox")
        {

            isInRange = false;

            if (damageCoroutine != null)
            {
                isInRange = false;

            }
        }
    }

    public IEnumerator StopAndStartMovement()
    {
        if (inHitStun) // Check if already in hit stun, return if true
            yield break;
        if (finishedAttack)
            yield break;

        // Store the current move speed
        float currentMoveSpeed = moveSpeed;

        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //Debug.Log("Setting movespeed to zero");
        inHitStun = true;
        if (animator != null)
        {
            animator.SetBool("inHitStun", true);
        }
        if (navMeshAgent != null)
        {
     
            navMeshAgent.enabled = false;
            //navMeshAgent.isStopped = true;
        }
        
        moveSpeed = 0;

        yield return new WaitForSeconds(moveStopDuration);
        if (animator != null)
        {
            animator.SetBool("inHitStun", false);
        }

        moveSpeed = currentMoveSpeed;
        if (!EnemyReceiveDamage.isDead)
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                //navMeshAgent.isStopped = false;
            }
        }
        inHitStun = false; 
    }

}
