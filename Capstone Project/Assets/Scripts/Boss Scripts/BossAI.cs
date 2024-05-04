using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    public bool isRangedEnemy;
    public float damage = 1;
    public float moveSpeed = 1000;
    public Rigidbody2D rb;
    public NavMeshAgent navMeshAgent;
    public GameObject groundSlamCirclePrefab; // Prefab for the ground slam circle

    private Coroutine damageCoroutine; // Store the coroutine instance
    private bool isCharging = false; // Flag to track if boss is charging
    private bool isGroundSlamming = false; // Flag to track if boss is performing ground slam
    //private bool isMeleeAttacking = false; // Flag to track if boss is performing melee attack
    private Vector2 chargeDirection; // Direction to charge
    private Animator animator;
    private EnemyReceiveDamage bossStats;

    //PLACEHOLDER - REMOVE THIS LATER
    public TMP_Text BossTelegraphs;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        animator = GetComponent<Animator>(); // Get the Animator component
        bossStats = GetComponent<EnemyReceiveDamage>();
        // Start the boss behavior cycle
        StartCoroutine(BossBehaviorCycle());
        
        if (GameState.IsAfterFivePM)
        {
            moveSpeed *= 2;
            damage *= 2;
        }

        GameState.OnAfterFivePM += UpdateStatsForFivePM;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        GameState.OnAfterFivePM -= UpdateStatsForFivePM;
    }

    private void UpdateStatsForFivePM()
    {
        moveSpeed *= 2;
        damage *= 2;
    }
    private IEnumerator BossBehaviorCycle()
    {
        while (true)
        {
            animator.ResetTrigger("chargePrep");
            animator.ResetTrigger("charging");
            animator.ResetTrigger("stunned");
            animator.ResetTrigger("slamWindup");
            animator.ResetTrigger("slamming");
            animator.ResetTrigger("isPunching");
            animator.SetTrigger("idle");
            BossTelegraphs.text = "Idle State".ToString();
            yield return new WaitForSeconds(3f); // Wait for 5 seconds in idle state

            // Decide which attack to perform
            int attackChoice = Random.Range(0, 3); // Randomly choose between charge attack, ground slam, and melee attack
            if (attackChoice == 0)
            {
                animator.ResetTrigger("idle");
                // Charge attack
                //BossTelegraphs.text = "Preparing Charge...".ToString();
                animator.SetTrigger("chargePrep");
                Debug.Log("Boss decides to do a charge.");
                StopMovement();
                yield return new WaitForSeconds(1f);
                //BossTelegraphs.text = "Charging!".ToString();
                StartChargeAttack();
                yield return new WaitForSeconds(3f);
                StopChargeAttack();
            }
            else if (attackChoice == 1)
            {
                animator.ResetTrigger("idle");
                // Ground slam attack
                //BossTelegraphs.text = "Preparing Slam...".ToString();
                Debug.Log("Boss decides to do a ground slam.");
                StartGroundSlamAttack();
                yield return new WaitForSeconds(1.1f); // Wait for telegraphing duration
                //BossTelegraphs.text = "Slamming!".ToString();
                animator.SetTrigger("slamming");
                yield return new WaitForSeconds(0.2f);
                if (isGroundSlamming) // Check if ground slam attack is still ongoing
                {
                    PerformGroundSlamAttack(); // Perform ground slam attack if player is inside AOE
                }
                StopGroundSlamAttack();
            }
            else
            {
                animator.ResetTrigger("idle");
                // Melee attack
                Debug.Log("Boss decides to do a melee attack.");
                StartMeleeAttack();
                BossTelegraphs.text = "Melee Attack!".ToString();
                yield return new WaitForSeconds(3f); // Wait between each dash
                Debug.Log("Melee Attack Finished.");
                StopMeleeAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isCharging)
        {
            MoveTowardsPlayer();
        }
        if (bossStats.isDead)
        {
            StopAllCoroutines();
            navMeshAgent.isStopped = true;
            animator.SetBool("isDead", true);
        }
    }

    private void MoveTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            navMeshAgent.SetDestination(playerPosition);

            navMeshAgent.speed = moveSpeed;

            // Update the animator with the movement direction
            Vector2 velocity = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.y);
            animator.SetFloat("xDir", velocity.x);
            animator.SetFloat("yDir", velocity.y);
        }
    }

    private void StartChargeAttack()
    {
        StopMovement();
        isCharging = true;

        // Face the player
        Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        chargeDirection = (playerPosition - (Vector2)transform.position).normalized;

        // Reset the animator parameters for charging if needed
        animator.SetFloat("xDir", chargeDirection.x);
        animator.SetFloat("yDir", chargeDirection.y);
        animator.SetTrigger("charging");

        StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine()
    {
        while (isCharging)
        {
            // Move in the charge direction with a high speed
            rb.velocity = chargeDirection * moveSpeed * 6;

            yield return null;
        }
    }

    private IEnumerator ChargeReset()
    {
        StopMovement();
        yield return new WaitForSeconds(3f);
        Debug.Log("ChargeReset: Waiting 3 Seconds");
        ResumeMovement();
    }

    private void StopChargeAttack()
    {
        isCharging = false;
        animator.SetTrigger("stunned");
        rb.velocity = Vector2.zero;
        StartCoroutine(ChargeReset());
    }

    private void StopMovement()
    {
        navMeshAgent.isStopped = true;
    }

    private void ResumeMovement()
    {
        navMeshAgent.isStopped = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            if (other.CompareTag("PlayerHurtbox"))
            {
                BossTelegraphs.text = "Stunned!".ToString();
                PlayerStats.playerStats.DealDamage(damage);
                StopChargeAttack(); // Stop the charge attack if collided with player

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging)
        {

            Collider2D collider = collision.collider;
            if (collider.CompareTag("Wall"))
            {
                BossTelegraphs.text = "Stunned!".ToString();
                Debug.Log("Hit a wall.");
                StopChargeAttack(); // Stop the charge attack if collided with a wall

            }
        }
    }
    private void StartGroundSlamAttack()
    {
        StopMovement();
        isGroundSlamming = true;
        animator.SetTrigger("slamWindup");
        // Instantiate the ground slam circle prefab
        GameObject groundSlamCircle = Instantiate(groundSlamCirclePrefab, transform.position, Quaternion.identity);
        Destroy(groundSlamCircle, 1.3f); // Destroy the circle after 3 seconds (telegraphing duration)
    }

    private void PerformGroundSlamAttack()
    {
        // Check if the player is inside the AOE
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(player.transform.position, transform.position) <= 7f) // Adjust 5f as necessary for the AOE radius
        {
            // Player is inside the AOE, deal damage
            PlayerStats.playerStats.DealDamage(damage);
        }
    }

    private void StopGroundSlamAttack()
    {
        ResumeMovement();
        animator.SetTrigger("idle");
        isGroundSlamming = false;

    }

    private void StartMeleeAttack()
    {
        StopMovement();
        //isMeleeAttacking = true;
        StartCoroutine(MeleeAttackCoroutine()); // Start coroutine for the melee attack
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        // Perform three dashes
        for (int i = 0; i < 3; i++)
        {
            // Get the current direction towards the player
            Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;

            // Set the Rigidbody's velocity to move towards the player
            rb.velocity = direction * moveSpeed * 5; // Dash towards the player

            // Set animator
            animator.SetTrigger("isPunching");
            animator.SetFloat("xDir", direction.x);
            animator.SetFloat("yDir", direction.y);
            animator.SetInteger("Punch", i);

            PerformMeleeAttack();
            yield return new WaitForSeconds(0.35f); // Adjust delay between each dash
            rb.velocity = Vector2.zero; // Reset velocity after each dash
            yield return new WaitForSeconds(0.35f); // Wait before performing the next dash
        }

        // Reset melee attack after all dashes are performed
        StopMeleeAttack();
    }

    private void PerformMeleeAttack()
    {
        // Check if the player is within melee range
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(player.transform.position, transform.position) <= 3.5f) // Adjust range as necessary
        {
            // Player is within melee range, deal damage
            PlayerStats.playerStats.DealDamage(damage);
        }
    }

    private void StopMeleeAttack()
    {
        animator.ResetTrigger("isPunching");
        animator.SetTrigger("idle");
        ResumeMovement();
        //isMeleeAttacking = false;
    }
}


