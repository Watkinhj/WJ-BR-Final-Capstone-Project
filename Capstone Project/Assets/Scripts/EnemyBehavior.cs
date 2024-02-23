using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float damage = 1;
    public float moveSpeed = 1000;
    public DetectionZone detectionZone;
    public Rigidbody2D rb;

    private Coroutine damageCoroutine; // Store the coroutine instance

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (detectionZone.detectedObjs.Count > 0)
        {
            Vector2 direction = (detectionZone.detectedObjs[0].transform.position - transform.position).normalized;

            rb.AddForce(direction * moveSpeed * Time.deltaTime);
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
}
