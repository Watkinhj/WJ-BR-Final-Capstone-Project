using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public float damage = 1;
    public float moveSpeed = 1000;
    public DetectionZone detectionZone;
    public Rigidbody2D rb;
    public float moveStopDuration = 1f;
    bool inHitStun = false;

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
            if (!inHitStun)
            {
                Transform playerTransform = detectionZone.detectedObjs[0].transform; // Assuming the first detected object is the player

                Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
                navMeshAgent.SetDestination(playerPosition);

                navMeshAgent.speed = moveSpeed;
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
            // Deal damage to the player every second
            PlayerStats.playerStats.DealDamage(damage);
            yield return new WaitForSeconds(1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHurtbox")
        {
            Debug.Log("Entered PlayerHurtbox");

            // Start the coroutine if it is not running
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageCoroutine());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHurtbox")
        {
            Debug.Log("Exited PlayerHurtbox");

            // Stop the coroutine if it is running
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    public IEnumerator StopAndStartMovement()
    {
        Debug.Log("Starting stun");
        // Store the current move speed
        float currentMoveSpeed = moveSpeed;

        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Debug.Log("Setting movespeed to zero");
        inHitStun = true;
        navMeshAgent.enabled = false;
        moveSpeed = 0;

        yield return new WaitForSeconds(moveStopDuration);

        
        moveSpeed = currentMoveSpeed;
        navMeshAgent.enabled = true;
        inHitStun = false;
    }

}
