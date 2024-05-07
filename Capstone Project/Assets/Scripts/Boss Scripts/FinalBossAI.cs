using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FinalBossAI : MonoBehaviour
{
    public enum BossState { Melee, Ranged, Transforming, MeleeAttack, RangedAttack, AOEAttack }
    private BossState currentState;
    public float projectileDamage = 15;
    public float groundSlamDamage = 45;
    public float speed = 5f;
    public Rigidbody2D rb;
    public GameObject projectilePrefab;
    public GameObject projectileP2Prefab;
    public GameObject aoePrefab;
    public float dashSpeed = 20f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    private float lastDashTime = float.MinValue;
    public float projectileCooldown = 1f;
    private float lastProjectileTime = float.MinValue;
    public bool inPhase1;
    public bool inPhase2;
    

    private Transform player;
    private Animator animator;
    public Animator goopAnimator;
    public Animator ceoTentacle;
    private EnemyReceiveDamage healthScript;
    private EnemyBehavior enemyBehavior;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        healthScript = GetComponent<EnemyReceiveDamage>();
        enemyBehavior = GetComponent<EnemyBehavior>();

        GameObject goop = GameObject.FindGameObjectWithTag("Goop");
        goopAnimator = goop.GetComponent<Animator>();

        GameObject tentacle = GameObject.FindGameObjectWithTag("CEOTentacle");
        ceoTentacle = tentacle.GetComponent<Animator>();
        animator.SetBool("Transition", false);

        inPhase1 = true;
        inPhase2 = false;
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
            if (healthScript.health <= healthScript.maxHealth * 0.5f && currentState != BossState.Transforming)
            {
                inPhase1 = false;
                currentState = BossState.Transforming;
                animator.SetTrigger("Transform");
                rb.velocity = Vector2.zero; // Stop the boss from moving
                StartCoroutine(Phase2BehaviorCycle());
                yield break; // Stop the current behavior cycle
            }

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

    IEnumerator Phase2BehaviorCycle()
    {
        yield return new WaitForSeconds(1.0f);
        goopAnimator.SetTrigger("GoopTime");
        yield return new WaitForSeconds(2.0f); // Wait for transformation to complete
        animator.SetBool("Transition", true);
        inPhase2 = true;

        while (inPhase2)
        {
            currentState = ChooseRandomStatePhase2();
            switch (currentState)
            {
                case BossState.MeleeAttack:
                    if (Vector2.Distance(transform.position, player.position) < 2f)
                        PerformMeleeAttackP2();
                    break;
                case BossState.RangedAttack:
                    FireProjectileP2();
                    yield return new WaitForSeconds(0.25f);
                    ceoTentacle.SetBool("isShooting", false);
                    animator.SetBool("isShooting", false);
                    break;
                case BossState.AOEAttack:
                    PerformAOEAttack();
                    break;
            }
            yield return new WaitForSeconds(2f);  // Wait before choosing another action
            animator.ResetTrigger("tentaclePlant");
            animator.SetTrigger("isIdle");
        }
    }

    BossState ChooseRandomStatePhase2()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue < 0.33f)
            return BossState.MeleeAttack;
        else if (randomValue < 0.66f)
            return BossState.RangedAttack;
        else
            return BossState.AOEAttack;
    }

    void Update()
    {
        if (currentState == BossState.Transforming) return; // Skip updates during transformation

        if (inPhase1)
        {
            HandleMovement();
            TryDash();
            HandleFiring();
        }

        // inPhase1 will be set to false on transformation, therefore it wont move

        if (inPhase2)
        {
            ApproachPlayer();
        }
    }

    void HandleMovement()
    {
        if (!player) return;
        if (!IsDashing())
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
                float angle = Time.time * 0.5f;
                Vector2 circlePosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;
                Vector2 desiredPosition = (Vector2)player.position + circlePosition;
                moveDirection = (desiredPosition - (Vector2)transform.position).normalized;
            }
            rb.velocity = moveDirection * speed;
            animator.SetFloat("xDir", moveDirection.x);
        }
    }

    void ToggleState()
    {
        if (currentState == BossState.Transforming) return; // Prevent state toggle during transformation
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
        animator.SetBool("isShooting", true);
        ceoTentacle.SetBool("isShooting", true);
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 10;
        projectile.GetComponent<EnemyProjectile>().damage = projectileDamage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void PerformMeleeAttack()
    {
        //Logic is stored in EnemyBehavior.cs
    }

    void TryDash()
    {
        if (Time.time > lastDashTime + dashCooldown && Random.value < 0.5f)
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
        rb.velocity = Vector2.zero;
    }

    bool IsDashing()
    {
        return Time.time < lastDashTime + dashDuration;
    }

    void ApproachPlayer()
    {
        Vector2 moveDirection = (player.position - transform.position).normalized;
        rb.velocity = moveDirection * speed;
        animator.SetFloat("xDir", moveDirection.x);
    }

    void PerformMeleeAttackP2()
    {
        // Melee attack logic for Phase 2
    }

    void FireProjectileP2()
    {
        GameObject projectile = Instantiate(projectileP2Prefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 10;
        projectile.GetComponent<EnemyProjectile>().damage = projectileDamage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void PerformAOEAttack()
    {
        // Stop movement temporarily and play windup animation
        StopMovement();
        animator.SetTrigger("tentaclePlant");

        // Instantiate the ground slam circle prefab at the boss's position
        GameObject groundSlamCircle = Instantiate(aoePrefab, transform.position, Quaternion.identity);
        Destroy(groundSlamCircle, 1.3f); // Destroy the circle after 1.3 seconds, which is the telegraphing duration

        // Wait for windup to complete before executing the slam
        StartCoroutine(GroundSlamCoroutine());
    }

    IEnumerator GroundSlamCoroutine()
    {
        yield return new WaitForSeconds(1.1f); // Wait for telegraphing duration

        // Perform the slam if the boss is still in the AOE Attack state
        if (currentState == BossState.AOEAttack)
        {
            PerformGroundSlam();
        }

        // Resume movement and return to idle state or another appropriate state
        ResumeMovement();
        animator.SetTrigger("idle");
    }

    void PerformGroundSlam()
    {
        // Check if the player is within the AOE radius
        if (Vector2.Distance(player.position, transform.position) <= 3f) // Adjust 7f as necessary for the AOE radius
        {
            // Player is inside the AOE, deal damage
            PlayerStats.playerStats.DealDamage(groundSlamDamage);
        }
    }
    void StopMovement()
    {
        speed = 0;
        rb.velocity = Vector2.zero; // Immediately stop any movement by setting velocity to zero
        rb.isKinematic = true; // Optionally make the Rigidbody kinematic to ignore forces
    }

    void ResumeMovement()
    {
        rb.isKinematic = false; // Return Rigidbody to dynamic to allow movement again
        speed = 5;
    }
}
