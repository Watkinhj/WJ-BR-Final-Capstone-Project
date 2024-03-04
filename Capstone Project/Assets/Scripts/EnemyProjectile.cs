using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        if (collision.tag != "Enemy")
        {
            if (collision.name != "EnemyProjectile")
            {
                if (collision.name != "Detection Zone")
                {
                    if (collision.name != "PlayerHurtbox")
                    {
                        if (collision.name != "MeleeHitbox")
                        {
                            if (collision.tag == "Player")
                            {
                                PlayerStats.playerStats.DealDamage(damage);
                            }
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}
