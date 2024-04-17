using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyReceiveDamage : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public GameObject healthBar;
    public Slider healthBarSlider;

    private EnemyBehavior enemyBehavior;

    public static bool isDead;

    public GameObject lootDrop;
    public GameObject healthDrop;

    public bool isBurning;

    private bool isDamaged = false;

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
        isDead = false;
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
            Destroy(gameObject);
            if (lootDrop != null) //If they can drop stuff
            {
                Instantiate(lootDrop, transform.position, Quaternion.identity);
                float randomChance = Random.Range(0f, 1f);
                if (randomChance <= 0.4f)
                {
                    Instantiate(healthDrop, transform.position, Quaternion.identity);
                }
            }
        }
    }

    private float CalculateHealthPercentage()
    {
        return (health / maxHealth);
    }
}
