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

    private int currentPosIndex = -1;

    private void Start()
    {
        LoadDashPositions();
        StartCoroutine(BossBehaviorCycle());
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

    private IEnumerator BossBehaviorCycle()
    {
        while (true)
        {
            yield return StartCoroutine(DashToPosition());
            yield return new WaitForSeconds(waitTime);
            AttackPattern chosenPattern = (AttackPattern)Random.Range(0, 3);
            StartCoroutine(ShootProjectiles(shootingDuration, chosenPattern));
            yield return new WaitForSeconds(shootingDuration + waitTime);
        }
    }

    private IEnumerator DashToPosition()
    {
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
    }

    private IEnumerator ShootProjectiles(float duration, AttackPattern pattern)
    {
        float startTime = Time.time;
        switch (pattern)
        {
            case AttackPattern.FocusedFire:
                while (Time.time - startTime < duration)
                {
                    ShootFocusedFire();
                    yield return new WaitForSeconds(0.2f);
                }
                break;

            case AttackPattern.SnipingShot:
                yield return StartCoroutine(AimAndShoot());
                break;

            case AttackPattern.WavePattern:
                while (Time.time - startTime < duration)
                {
                    ShootWavePattern();
                    yield return new WaitForSeconds(0.2f); // Slower fire rate for wave pattern
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
            var lineObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
            LineRenderer line = lineObject.GetComponent<LineRenderer>();
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

            Destroy(lineObject); // Remove aim line
            ShootProjectile(GetPlayerDirection() * 3); // Increased velocity for the sniping shot
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