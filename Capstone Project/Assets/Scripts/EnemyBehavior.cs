using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    //As of now, there are two enemy types. Melee types and ranged types. We'll differentiate them via tags. 
    //If an enemy is tagged as a melee type, it will rush down the player and melee attack them until they die. 
    //If an enemy is a ranged type, it will approach the player, slow down to fire, then continue to approach. 
    //if thats too complicated for now, then we'll have the enemy stop to shoot, then continue moving.

    public float damage = 1;

    public float moveSpeed = 1000;

    public DetectionZone detectionZone;

    public Rigidbody2D rb;

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

    //Damaging the player
    //Right now, the enemies only damage the player ONCE per collision.
    //I need to find a way to get enemies to continue attacking them when the player is in range.
    //Maybe make a hurtbox for the player that's a trigger?
    //That could work...

    /* WIP CODE
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHurtbox")
        {
            Debug.Log("Entered PlayerHurtbox");
            PlayerStats.playerStats.DealDamage(damage);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHurtbox")
        {
            Debug.Log("Exited PlayerHurtbox");
            //PlayerStats.playerStats.StopDamage();
        }
    }
    */

    //Old code, just in case
    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.gameObject.tag == "Player")
            {
                PlayerStats.playerStats.DealDamage(damage);
            }
    }
    
}
