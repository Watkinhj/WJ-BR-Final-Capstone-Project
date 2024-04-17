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
    public bool inHitStun = false;
    bool isInRange;
    bool finishedAttack;

    private Animator animator;

    private Coroutine damageCoroutine; // Store the coroutine instance

    public NavMeshAgent navMeshAgent;

    private RangedEnemyBehavior rangedEnemyBehavior; // Reference to RangedEnemyBehavior script


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        animator = GetComponent<Animator>(); // Get the Animator component

        if (isRangedEnemy)
        {
            rangedEnemyBehavior = GetComponent<RangedEnemyBehavior>(); // Get RangedEnemyBehavior component
        }
    }

    private void FixedUpdate()
    {
        if (navMeshAgent != null)
        {
            if (detectionZone != null)
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
                            if (rangedEnemyBehavior != null)
                            {
                                StartCoroutine(rangedEnemyBehavior.ShootCooldown());
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
    }

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            if (isInRange)
            {
                if (!inHitStun)
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
            }
            yield return new WaitForSeconds(1f);
            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    public bool IsInHitStun()
    {
        return inHitStun;
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

        float currentMoveSpeed = moveSpeed;

        inHitStun = true;
        if (animator != null)
        {
            animator.SetBool("inHitStun", true);
        }
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            // Stop the NavMeshAgent if it's active and placed on the NavMesh
            navMeshAgent.isStopped = true;
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
            if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                // Resume NavMeshAgent movement if it's active and placed on the NavMesh
                navMeshAgent.isStopped = false;
            }
        }
        inHitStun = false; 
    }


}
