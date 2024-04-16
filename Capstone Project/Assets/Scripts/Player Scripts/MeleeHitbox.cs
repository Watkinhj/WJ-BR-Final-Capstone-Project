using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public PlayerStats player;
    public float damage;
    public float knockbackForce = 1;

    // List to keep track of enemies already damaged in the current attack
    private List<EnemyReceiveDamage> damagedEnemies = new List<EnemyReceiveDamage>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyReceiveDamage enemy = collision.GetComponent<EnemyReceiveDamage>();

            // Check if the enemy has already been damaged in this attack
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                enemy.DealDamage(damage);
                damagedEnemies.Add(enemy);

                player.CallItemOnHit(enemy);

                Rigidbody2D enemyRigidbody = collision.GetComponent<Rigidbody2D>();
                if (enemyRigidbody != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                BossAI bossBehavior = collision.GetComponent<BossAI>();
                if (bossBehavior == null)
                {
                    StartCoroutine(collision.GetComponent<EnemyBehavior>().StopAndStartMovement());
                }
            }
        }
    }

    // Reset the list of damaged enemies at the end of the attack
    public void ResetDamagedEnemies()
    {
        damagedEnemies.Clear();
    }
}
