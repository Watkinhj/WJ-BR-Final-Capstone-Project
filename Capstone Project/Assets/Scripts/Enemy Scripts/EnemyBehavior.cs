using System.Collections;
using System.Collections.Generic;
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

    private Coroutine damageCoroutine; // Store the coroutine instance

    public NavMeshAgent navMeshAgent;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void FixedUpdate()
    {
        if (detectionZone.detectedObjs.Count > 0)
        {
            if (!inHitStun) //If Enemy is not stunned, move towards them
            {
                Transform playerTransform = detectionZone.detectedObjs[0].transform; // Assuming the first detected object is the player

                Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
                navMeshAgent.SetDestination(playerPosition);

                navMeshAgent.speed = moveSpeed;

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

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            if (isInRange)
            {
                PlayerStats.playerStats.DealDamage(damage);
            }
            yield return new WaitForSeconds(1f);
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
                //StopCoroutine(damageCoroutine);
                //damageCoroutine = null;
            }
        }
    }

    public IEnumerator StopAndStartMovement()
    {
        //Debug.Log("Starting stun");
        // Store the current move speed
        float currentMoveSpeed = moveSpeed;

        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //Debug.Log("Setting movespeed to zero");
        inHitStun = true;
        navMeshAgent.enabled = false;
        moveSpeed = 0;

        yield return new WaitForSeconds(moveStopDuration);

        
        moveSpeed = currentMoveSpeed;
        if (!EnemyReceiveDamage.isDead)
        {
            navMeshAgent.enabled = true;
        }
        inHitStun = false;
    }

}
