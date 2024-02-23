using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public PlayerStats player;
    public float damage;
    public float knockbackForce = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player")
        {
            if (collision.GetComponent<EnemyReceiveDamage>() != null)
            {
                Debug.Log("Dealing Melee Damage");
                //Dealing Damage
                collision.GetComponent<EnemyReceiveDamage>().DealDamage(damage);
                EnemyReceiveDamage enemy = collision.GetComponent<EnemyReceiveDamage>();
                player.CallItemOnHit(enemy);

                Rigidbody2D enemyRigidbody = collision.GetComponent<Rigidbody2D>();
                if (enemyRigidbody != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
