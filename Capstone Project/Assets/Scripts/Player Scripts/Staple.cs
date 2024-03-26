using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staple : MonoBehaviour
{
    public PlayerStats gm;
    public float damage;

    private void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        PlayerStats player = gm.GetComponent<PlayerStats>();
    }

    //Full disclosure, this is the jankiest script ever, but it works perfectly lmao
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player")
        {
            if (collision.name != "MeleeHitbox")
            {
                if (collision.name != "PlayerHurtbox")
                {
                    if (collision.name != "Detection Zone")
                    {
                        if (collision.GetComponent<EnemyReceiveDamage>() != null)
                        {
                            collision.GetComponent<EnemyReceiveDamage>().DealDamage(damage);
                            EnemyReceiveDamage enemy = collision.GetComponent<EnemyReceiveDamage>();
                            gm.CallItemOnHit(enemy);
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
