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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //deal damage
    }
}
