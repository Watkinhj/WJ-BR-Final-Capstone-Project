using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staple : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player")
        {
            if(collision.GetComponent<EnemyReceiveDamage>() != null)
            {
                collision.GetComponent<EnemyReceiveDamage>().DealDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
