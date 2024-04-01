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
    public Slider dashSlider; // Assign this in the Unity Editor
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
        yield return new WaitForSeconds(1); //One second per item update. We may need to change this, maybe 0.1f?
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

    public void AddCurrency(CurrencyPickup currency)
    {
        if (currency.currentObject == CurrencyPickup.PickupObject.COIN)
        {
            credits += currency.pickupQuantity;
            currencyValue.text = "Credits: " + credits.ToString();
        }
    }

    private void SetAmmoUI()
    {
        ammoText.text = "Ammo: " + PlayerProjectile.currentAmmo + "/" + maxAmmo;

    }

    private void SetMeleeSpeed()
    {
        animator.SetFloat("attackSpeed", meleeSpeed);
    }
}
