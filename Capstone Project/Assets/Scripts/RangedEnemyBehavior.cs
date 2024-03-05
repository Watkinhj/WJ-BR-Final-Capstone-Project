using System.Collections;
using UnityEngine;

public class RangedEnemyBehavior : MonoBehaviour
{
    public GameObject projectile;
    public Transform player;
    public float damage;
    public float projectileForce;
    public float cooldown;

    private bool canShoot = true; // Track if the enemy can shoot

    void Start()
    {
        //StartCoroutine(ShootCooldown());
    }

    public IEnumerator ShootCooldown()
    {
        while (true)
        {
            if (canShoot && player != null)
            {
                Shoot();
                canShoot = false; // Prevent shooting during cooldown
                yield return new WaitForSeconds(cooldown);
                canShoot = true; // Allow shooting again after cooldown
            }
            yield return null;
        }
    }

    void Shoot()
    {
        GameObject enemyProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        Vector2 myPos = transform.position;
        Vector2 targetPos = player.position;
        Vector2 direction = (targetPos - myPos).normalized;
        enemyProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
        enemyProjectile.GetComponent<EnemyProjectile>().damage = damage;
    }
}
