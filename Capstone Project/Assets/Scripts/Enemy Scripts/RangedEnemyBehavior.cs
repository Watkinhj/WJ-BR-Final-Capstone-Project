using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyBehavior : MonoBehaviour
{
    public GameObject projectile;
    public Transform player;
    public float damage;
    public float projectileForce;
    public float cooldown;
    private Animator animator;
    private Coroutine shootingCoroutine;

    private bool canShoot = true; // Track if the enemy can shoot

    void Start()
    {
        //StartCoroutine(ShootCooldown());
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // Get the Animator component
        shootingCoroutine = StartCoroutine(ShootCooldown());

        if (GameState.IsAfterFivePM)
        {
            damage *= 2;
        }

        GameState.OnAfterFivePM += UpdateStatsForFivePM;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        GameState.OnAfterFivePM -= UpdateStatsForFivePM;
    }

    private void UpdateStatsForFivePM()
    {
        damage *= 2;
    }

    public void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(ShootCooldown());
            shootingCoroutine = null;
        }
    }


    public IEnumerator ShootCooldown()
    {
        while (true)
        {
            if (canShoot && player != null)
            {
                if (!GetComponent<EnemyBehavior>().IsInHitStun())
                Shoot();
                animator.SetTrigger("isAttacking"); // Set isAttacking to true before starting the animation
                canShoot = false; // Prevent shooting during cooldown

                // Wait for a short duration to ensure the animation has completed
                yield return new WaitForSeconds(0.1f); // animation time

                animator.SetBool("isAttacking", false); // Set isAttacking to false immediately after animation completes
                yield return new WaitForSeconds(cooldown);
                canShoot = true; // Allow shooting again after cooldown
            }
            else
            {
                yield return null; // Pause the coroutine if in hit stun or player is null
            }
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
