using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staple : MonoBehaviour
{
    public float damage;

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
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
