using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats playerStats;

    public float moveSpeed = 5;
    public GameObject player;
    public TMP_Text healthText;
    public Slider healthSlider;
    public Slider dashSlider; 
    public float health;
    public float maxHealth;
    public int credits;
    public TMP_Text currencyValue;
    public TMP_Text ammoText;
    public float damage = 20; 
    public List<ItemList> items = new List<ItemList>();

    //melee attack stats
    public float meleeSpeed = 1;
    public Animator animator;

    //ranged attack stats
    public int maxAmmo = 20;
    public float rangedCooldown = 0.5f;

    //thing that makes the chili powder work
    public float burnMultiplier = 1;
    public bool isBurnSpreadable;

    //thing that makes bubble wrap work
    public float damageReductionPercentage = 0f;

    //thing that makes the lightweight coat work
    private const float baseDodgeChance = 0.1f;
    private const float additionalDodgePerStack = 0.1f;
    private bool hasLightweightCoat = false;

    //thing that makes the prive gavel work
    public float knockbackForce = 10;

    //thing that makes the time card work
    public bool is5PM;
    public bool hasTimeCard;

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
        //grabs the animator
        animator = player.GetComponent<Animator>();

        //Health Stuff
        health = maxHealth;
        //healthSlider.value = 1;
        SetHealthUI();

        //Begins the item shenanigans
        StartCoroutine(CallTimedItemUpdate());
    }

    //COMMENT OUT SetHealthUI IN UPDATE BY DEFAULT, USE ONLY FOR TESTING PURPOSES. FIGURE OUT A WAY TO MAKE IT UPDATE ON ITEM USE LATER
    void Update()
    {
        SetHealthUI();
        CheckOverheal();
        SetDashUI();
        SetAmmoUI();
        SetMeleeSpeed();
        //AddCurrency();
        //CallItemStatsUpdate();

    }
    //Making sure that items actually WORK
    IEnumerator CallTimedItemUpdate()
    {
        foreach (ItemList i in items)
        {
            i.item.Update(this, i.stacks);
        }
        yield return new WaitForSeconds(3); //One second per item update. We may need to change this, maybe 0.1f?
        StartCoroutine(CallTimedItemUpdate());

    }

    public void CallItemOnHit(EnemyReceiveDamage enemy)
    {
        foreach (ItemList i in items)
        {
            
            i.item.OnHit(this, enemy, i.stacks);
        }
    }

    public void CallItemOnPickup()
    {
        foreach (ItemList i in items)
        {
            i.item.OnPickup(this, i.stacks);
            SetMeleeSpeed(); //maybe this will work lmfao
        }
    }

    public void CallRangedStatsUpdate(PlayerProjectile ranged)
    {
        foreach (ItemList i in items)
        {
            //i.item.UpdateRanged(ranged, i.stacks);
        }
    }

    public void DealDamage(float damage)
    {
        if (hasLightweightCoat)
        {
            // Calculate dodge chance based on Lightweight Coat stacks
            float dodgeChance = baseDodgeChance + (additionalDodgePerStack * GetLightweightCoatStacks());

            // Check if the player dodges the damage
            if (Random.value <= dodgeChance)
            {
                Debug.Log("Player dodges the incoming damage!");
                return; // Exit the method without taking damage
            }
        }

        // Player takes damage if not dodged
        float fullDamage = damage * (1 - damageReductionPercentage);
        health -= fullDamage;
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
        if(healthSlider != null)
        {
            healthSlider.value = CalculateHealthPercentage();
            healthText.text = Mathf.Ceil(health).ToString() + " / " + Mathf.Ceil(maxHealth).ToString();
        }
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

    public void AddCurrency(CurrencyPickup currency)
    {
        float baseCreditIncreasePercentage = 1.0f; 
        float additionalCreditIncreasePercentage = 0.10f; 

        float totalCreditIncreasePercentage = baseCreditIncreasePercentage; 

        bool firstSavingsJarStack = true; // Flag to track the first SavingsJar stack
        foreach (ItemList item in items)
        {
            if (item.item.GetType() == typeof(SavingsJar))
            {
                if (firstSavingsJarStack)
                {
                    totalCreditIncreasePercentage += 0.15f;
                    firstSavingsJarStack = false; 
                }
                else
                {
                    totalCreditIncreasePercentage += additionalCreditIncreasePercentage;
                }
            }
        }

        if (currency.currentObject == CurrencyPickup.PickupObject.COIN)
        {
            // Apply credit increase percentage to the received credits
            int increasedCredits = Mathf.RoundToInt(currency.pickupQuantity * totalCreditIncreasePercentage);
            credits += increasedCredits;
            currencyValue.text = "Credits: " + credits.ToString();
        }
    }

    public void UpdateCurrency()
    {
        currencyValue.text = "Credits: " + credits.ToString();
    }

    private void SetAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + PlayerProjectile.currentAmmo + "/" + maxAmmo;
        }
    }

    private void SetMeleeSpeed()
    {
        animator.SetFloat("attackSpeed", meleeSpeed);
    }

    private int GetLightweightCoatStacks()
    {
        int coatStacks = 0;
        foreach (ItemList item in items)
        {
            if (item.item.GetType() == typeof(LightweightCoat))
            {
                coatStacks += item.stacks;
            }
        }
        return coatStacks;
    }

    public void UpdateLightweightCoatStatus()
    {
        hasLightweightCoat = true;
    }

    public void UpdateTimeCard()
    {
        hasTimeCard = true;
    }

    public void TimeCardCheck()
    {
        if (hasTimeCard)
        {
            moveSpeed = moveSpeed * 1.5f;
            maxHealth = maxHealth * 1.5f;
            damage = damage * 1.5f;
            meleeSpeed = meleeSpeed * 1.5f;
            maxAmmo = maxAmmo * 2;
            rangedCooldown = rangedCooldown / 1.5f;
        }
        else
        {
            return;
        }
    }

    // This method checks for the Lucky Pen and calculates the chance
    public bool CheckChanceWithLuckyPen(float baseChance)
    {
        int luckyPenStacks = 0;
        foreach (ItemList item in items)
        {
            if (item.item.GetType() == typeof(LuckyPen))
            {
                luckyPenStacks += item.stacks;
            }
        }

        // If the player does not have any Lucky Pen, return the base chance result
        if (luckyPenStacks == 0)
        {
            
            return Random.value <= baseChance;
        }

        // If the player has Lucky Pen, allow additional rolls for each stack
        for (int i = 0; i <= luckyPenStacks; i++)
        {
            
            if (Random.value <= baseChance)
                return true;
        }
        return false;
    }

}
