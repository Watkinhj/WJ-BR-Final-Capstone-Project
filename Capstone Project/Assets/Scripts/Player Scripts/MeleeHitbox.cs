using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public PlayerStats player;
    public GameObject playerTransform;
    public float damage;
    //public float knockbackForce = 1;

    private void Update()
    {
        if (player == null)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null)
            {
                player = gameManager.GetComponent<PlayerStats>();
            }
        }
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // List to keep track of enemies already damaged in the current attack
    private List<EnemyReceiveDamage> damagedEnemies = new List<EnemyReceiveDamage>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyReceiveDamage enemy = collision.GetComponent<EnemyReceiveDamage>();
            EnemyBehavior enemyBehavior = collision.GetComponent<EnemyBehavior>();

            // Check if the enemy has already been damaged in this attack
            if (enemy != null && enemyBehavior != null && !damagedEnemies.Contains(enemy))
            {
                enemy.DealDamage(damage);
                damagedEnemies.Add(enemy);

                player.CallItemOnHit(enemy);

                Rigidbody2D enemyRigidbody = collision.GetComponent<Rigidbody2D>();
                if (enemyRigidbody != null)
                {
                    if (!enemy.isBoss)
                    {
                        Vector2 knockbackDirection = (collision.transform.position - playerTransform.transform.position).normalized;
                        enemyRigidbody.AddForce(knockbackDirection * player.knockbackForce, ForceMode2D.Impulse);
                    }
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
