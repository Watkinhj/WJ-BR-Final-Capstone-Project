using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemyBehavior : MonoBehaviour
{
    public GameObject projectile;
    public Transform player;
    public float damage;
    public float projectileForce;
    public float cooldown;

    void Start()
    {
        StartCoroutine(ShootPlayer());
    }

    IEnumerator ShootPlayer()
    {
        yield return new WaitForSeconds(cooldown);
        if (player != null )
        {
            GameObject enemyProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            Vector2 myPos = transform.position;
            Vector2 targetPos = player.position;
            Vector2 direction = (targetPos - myPos).normalized; 
            enemyProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
            enemyProjectile.GetComponent<EnemyProjectile>().damage = damage;
            StartCoroutine(ShootPlayer());
        }
    }
}
