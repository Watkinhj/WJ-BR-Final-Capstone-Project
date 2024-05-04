using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FinalBossAI : MonoBehaviour
{
    public enum BossState { Melee, Ranged }
    private BossState currentState;
    public float projectileDamage = 15;
    public float speed = 5f;
    public Rigidbody2D rb;
    public GameObject projectilePrefab;
    public float dashSpeed = 20f;
    public float dashDuration = 0.5f;  // Duration of the dash
    public float dashCooldown = 2f;
    private float lastDashTime = float.MinValue;  // Initialize to allow dashing immediately
    public float projectileCooldown = 1f;
    private float lastProjectileTime = float.MinValue;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(BossBehaviorCycle());
        if (GameState.IsAfterFivePM)
        {
            speed *= 2;
            projectileDamage *= 2;
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
        speed *= 2;
        projectileDamage *= 2;
    }


    IEnumerator BossBehaviorCycle()
    {
        while (true)
        {
            if (currentState == BossState.Melee)
            {
                if (Vector2.Distance(transform.position, player.position) < 2f)
                {
                    PerformMeleeAttack();
                }
            }
            yield return new WaitForSeconds(Random.Range(2, 5));  // Random delay before changing states
            ToggleState();
        }
    }

    void Update()
    {
        HandleMovement();
        TryDash();  // Check for dash opportunities in Update for more frequent checks
        HandleFiring();  // Manage firing of projectiles based on cooldown
    }

    void HandleMovement()
    {
        if (!player) return;

        if (!IsDashing())  // Only move if not currently dashing
        {
            Vector2 moveDirection;
            if (currentState == BossState.Melee)
            {
                Vector2 targetPosition = player.position;
                moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            }
            else
            {
                float circleRadius = 5f;
                float angle = Time.time * 0.5f; // Adjust angle to control circling speed
                Vector2 circlePosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;
                Vector2 desiredPosition = (Vector2)player.position + circlePosition;
                moveDirection = (desiredPosition - (Vector2)transform.position).normalized;
            }
            rb.velocity = moveDirection * speed;
        }
    }

    void ToggleState()
    {
        currentState = (currentState == BossState.Melee) ? BossState.Ranged : BossState.Melee;
    }

    void HandleFiring()
    {
        if (currentState == BossState.Ranged && Time.time > lastProjectileTime + projectileCooldown && player)
        {
            FireProjectile();
            lastProjectileTime = Time.time;
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 10;
    }

    void PerformMeleeAttack()
    {
        // Melee attack logic or animations
    }

    void TryDash()
    {
        if (Time.time > lastDashTime + dashCooldown && Random.value < 0.5f)  // Adjust probability to increase/decrease dashing frequency
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        Vector2 dashDirection = (currentState == BossState.Melee) ? (player.position - transform.position).normalized : (transform.position - player.position).normalized;
        rb.velocity = dashDirection * dashSpeed;
        lastDashTime = Time.time;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;  // Reset velocity after dash
    }

    bool IsDashing()
    {
        return Time.time < lastDashTime + dashDuration;
    }
}
