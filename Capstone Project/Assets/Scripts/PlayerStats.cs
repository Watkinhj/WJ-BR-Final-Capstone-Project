using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats playerStats;

    public GameObject player;
    public TMP_Text healthText;
    public Slider healthSlider;
    public Slider dashSlider; // Assign this in the Unity Editor
    public float health;
    public float maxHealth;

    public List<ItemList> items = new List<ItemList>();
    private void Awake()
    {
        if (playerStats != null)
        {
            Destroy(playerStats);
        }
        else
        {
            playerStats = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //Health Stuff
        health = maxHealth;
        //healthSlider.value = 1;
        SetHealthUI();

        //Begins the item shenanigans
        StartCoroutine(CallItemUpdate());

    }

    //COMMENT OUT SetHealthUI IN UPDATE BY DEFAULT, USE ONLY FOR TESTING PURPOSES. FIGURE OUT A WAY TO MAKE IT UPDATE ON ITEM USE LATER
    void Update()
    {
        SetHealthUI();
        CheckOverheal();
        SetDashUI();

    }
    //Making sure that items actually WORK
    IEnumerator CallItemUpdate()
    {
        foreach (ItemList i in items)
        {
            i.item.Update(this, i.stacks);
        }
        yield return new WaitForSeconds(1); //One second per item update. We may need to change this, maybe 0.1f?
        StartCoroutine(CallItemUpdate());

    }

    public void CallItemOnHit(EnemyReceiveDamage enemy)
    {
        foreach (ItemList i in items)
        {
            i.item.OnHit(this, enemy, i.stacks);
        }
    }

    public void DealDamage(float damage)
    {
        health -= damage;
        CheckDeath();
        SetHealthUI();
    }

    public void HealCharacter(float heal)
    {
        health += heal;
        CheckOverheal();
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        healthSlider.value = CalculateHealthPercentage();
        healthText.text = Mathf.Ceil(health).ToString() + " / " + Mathf.Ceil(maxHealth).ToString();
    }


    private void SetDashUI()
    {
        if (dashSlider != null)
        {
            // If the player is dashing, show cooldown on the slider, otherwise, set it to the maximum value
            if (!PlayerMovement.canDash)
            {
                dashSlider.value = Mathf.Clamp01((Time.time - PlayerMovement.dashStartTime) / PlayerMovement.dashCooldown);

            }
            if (PlayerMovement.canDash)
            {
                dashSlider.value = 1;
            }
        }
    }


    private void CheckOverheal()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    private void CheckDeath()
    {
        if (health <= 0)
        {
            health = 0;
            Destroy(player);
        }
    }

    float CalculateHealthPercentage()
    {
        return health / maxHealth;
    }
}
