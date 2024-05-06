using System.Collections;
using UnityEngine;

public enum AttackPattern
{
    FocusedFire,
    SnipingShot,
    WavePattern
}

public class BossAI2 : MonoBehaviour
{
    public Transform[] dashPositions;
    public GameObject projectilePrefab;
    public GameObject lineRendererPrefab; // Prefab with a LineRenderer component
    public float dashSpeed = 10f;
    public float shootingDuration = 5f;
    public float waitTime = 1f;
    public float projectileForce = 20f; // Force to apply to projectiles
    public float damage = 1f; // Damage each projectile deals
    private Animator animator;
    private int currentPosIndex = -1;
    private EnemyReceiveDamage bossStats;
    private Vector3 lastPosition; // To calculate movement direction for xDir
    private GameObject activeLineObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bossStats = GetComponent<EnemyReceiveDamage>();
        lastPosition = transform.position;
        LoadDashPositions();
        StartCoroutine(BossBehaviorCycle());

        if (GameState.IsAfterFivePM)
        {
            dashSpeed *= 2;
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
        dashSpeed *= 2;
        damage *= 2;
    }

    private void LoadDashPositions()
    {
        GameObject[] dashObjects = GameObject.FindGameObjectsWithTag("DashTransform");
        dashPositions = new Transform[dashObjects.Length];
        for (int i = 0; i < dashObjects.Length; i++)
        {
            dashPositions[i] = dashObjects[i].transform;
        }
    }

    private void FixedUpdate()
    {
        if (bossStats.isDead)
        {
            StopAllCoroutines();
            animator.SetBool("isDead", true);
            if (lineRendererPrefab != null)
            {
                Destroy(activeLineObject);
            }
        }
        else
        {
            Vector3 movementDirection = (transform.position - lastPosition).normalized;
            animator.SetFloat("xDir", movementDirection.x);
            lastPosition = transform.position;
        }
    }

    private IEnumerator BossBehaviorCycle()
    {
        while (!bossStats.isDead)
        {
            yield return StartCoroutine(DashToPosition());
            yield return new WaitForSeconds(waitTime);
            AttackPattern chosenPattern = (AttackPattern)Random.Range(0, 3);
            StartCoroutine(ShootProjectiles(shootingDuration, chosenPattern));
            yield return new WaitForSeconds(shootingDuration + waitTime);
            animator.SetBool("isAttacking", false);
        }
    }

    private IEnumerator DashToPosition()
    {
        animator.SetBool("isDashing", true);

        int newPosIndex;
        do
        {
            newPosIndex = Random.Range(0, dashPositions.Length);
        } while (newPosIndex == currentPosIndex);

        currentPosIndex = newPosIndex;
        Transform targetPos = dashPositions[currentPosIndex];

        while (Vector2.Distance(transform.position, targetPos.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos.position, dashSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("isDashing", false);
    }

    private IEnumerator ShootProjectiles(float duration, AttackPattern pattern)
    {
        
        float startTime = Time.time;
        switch (pattern)
        {
            case AttackPattern.FocusedFire:
                while (Time.time - startTime < duration)
                {
                    animator.SetBool("isAttacking", true);
                    ShootFocusedFire();
                    yield return new WaitForSeconds(0.2f);
                    animator.SetBool("isAttacking", false);
                }
                break;
            case AttackPattern.SnipingShot:
                yield return StartCoroutine(AimAndShoot());
                break;
            case AttackPattern.WavePattern:
                while (Time.time - startTime < duration)
                {
                    animator.SetBool("isAttacking", true);
                    ShootWavePattern();
                    yield return new WaitForSeconds(0.2f);
                    animator.SetBool("isAttacking", false);
                }
                break;
        }
    }

    private void ShootFocusedFire()
    {
        Debug.Log("Focused Fire");
        ShootProjectile(GetPlayerDirection());
    }

    private IEnumerator AimAndShoot()
    {
        GameObject player = GetPlayer();
        if (player != null)
        {
            activeLineObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
            LineRenderer line = activeLineObject.GetComponent<LineRenderer>();
            line.SetPosition(0, transform.position);

            float aimDuration = 1.5f;
            float startTime = Time.time;

            while (Time.time - startTime < aimDuration)
            {
                if (player != null)
                {
                    line.SetPosition(1, player.transform.position);
                    yield return null;
                }
            }

            Destroy(activeLineObject); // Remove aim line after use
            activeLineObject = null; // Clear the reference
            ShootProjectile(GetPlayerDirection() * 3); // Increased velocity for the sniping shot
            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.2f);
            animator.SetBool("isAttacking", false);
        }
    }

    private void ShootWavePattern()
    {
        Debug.Log("Wave pattern started");

        float angleStep = 15f;  // Adjust the angle between shots
        int numberOfProjectiles = 5;  // Total number of projectiles in a wave
        float startAngle = -angleStep * (numberOfProjectiles - 1) / 2;  // Calculating the starting angle
        Vector2 baseDirection = GetPlayerDirection();  // Base direction towards the player

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Vector2 direction = Quaternion.Euler(0, 0, startAngle + angleStep * i) * baseDirection;
            Debug.Log($"Shooting wave projectile {i}, direction: {direction}, magnitude: {direction.magnitude}");
            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector2 direction, float speedMultiplier = 1f)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileForce * speedMultiplier;
        projectile.GetComponent<EnemyProjectile>().damage = damage;
    }

    private Vector2 GetPlayerDirection()
    {
        GameObject player = GetPlayer();
        if (player != null)
        {
            return (player.transform.position - transform.position).normalized;
        }
        return Vector2.up; // Default direction if no player found
    }

    private GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
