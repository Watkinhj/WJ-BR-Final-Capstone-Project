using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyReceiveDamage : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public GameObject healthBar;
    public Slider healthBarSlider;

    private EnemyBehavior enemyBehavior;

    public bool isDead;

    public GameObject lootDrop;
    public GameObject healthDrop;

    public bool isBurning;

    private bool isDamaged = false;

    public bool isBossEnemy;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Rigidbody2D rb;
    private RangedEnemyBehavior rangedEnemyBehavior;
    public bool isBoss;

    //stuff for boss death
    public delegate void DeathAction();
    public event DeathAction OnDeath;

    public GameObject critHitEffectPrefab;
    public GameObject burnEffectPrefab;
    public GameObject shockEffectPrefab;


    public bool IsDamaged()
    {
        return isDamaged;
    }

    public void SetDamaged(bool value)
    {
        isDamaged = value;
    }

    private void Start()
    {
        health = maxHealth;
        enemyBehavior = GetComponent<EnemyBehavior>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        isDead = false;

        if (enemyBehavior.isRangedEnemy)
        {
            rangedEnemyBehavior = GetComponent<RangedEnemyBehavior>();
        }
    }

    public void DealDamage(float damage)
    {
        if (healthBar != null)
        {
            healthBar.SetActive(true);
        }
        health -= damage;
        healthBarSlider.value = CalculateHealthPercentage();
        CheckDeath();
    }

    public void HealCharacter(float heal)
    {
        health += heal;
        CheckOverheal();
        healthBarSlider.value = CalculateHealthPercentage();
    }

    private void CheckOverheal()
    {
        if (health < maxHealth)
        {
            health = maxHealth;
        }
    }

    private void CheckDeath()
    {
        if (health <= 0)
        {
            isDead = true;
            if (isDead)
            {
                animator.SetBool("isDead", true);
                if (navMeshAgent != null)
                {
                    navMeshAgent.isStopped = true;
                }
                Destroy(healthBar);
                Destroy(GetComponent<EnemyBehavior>());
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }
                
                if (enemyBehavior.isRangedEnemy)
                {
                    Debug.Log("Stopping ranged enemy cooldown");
                    Destroy(GetComponent<RangedEnemyBehavior>());
                }
                OnDeath?.Invoke();
                StartCoroutine(DespawnEnemy());
            }
            //Destroy(gameObject);
            if (lootDrop != null) //If they can drop stuff
            {
                Instantiate(lootDrop, transform.position, Quaternion.identity);
                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.4f)
                {
                    Instantiate(healthDrop, transform.position, Quaternion.identity);
                }
                lootDrop = null;
            }
        }
    }

    private float CalculateHealthPercentage()
    {
        return (health / maxHealth);
    }

    private IEnumerator DespawnEnemy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public void ShowEffect(GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(effect, 0.75f); // Adjust time based on how long you want the effect to show
        }
    }

    public void BurnEffect(GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(effect, 5.0f); // Adjust time based on how long you want the effect to show
        }
    }
}
