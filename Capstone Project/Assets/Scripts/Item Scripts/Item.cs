using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();

    public abstract string GiveDescription();
    public virtual void Update(PlayerStats player, int stacks)
    {

    }

    public virtual void OnPickup(PlayerStats player, int stacks)
    {

    }

    public virtual void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {

    }
}

public class GreenDrink : Item
{
    public override string GiveName()
    {
        return "Green Drink";
    }

    public override string GiveDescription()
    {
        return "Increases health regeneration.";
    }

    public override void Update(PlayerStats player, int stacks)
    {
        player.health += 1 + (1 * stacks);
        
    }
}

public class MicrowaveSoup : Item
{
    private float burnChance;

    public override string GiveName()
    {
        return "Microwave Soup";
    }

    public override string GiveDescription()
    {
        return "Gain a 20% chance to burn enemies on hit.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        burnChance = (0.1f + (0.1f * stacks));
        if (Random.value <= burnChance) // Check if the burn chance is activated
        {
            Debug.Log("burn proc! Multiplier: " + player.burnMultiplier);
            enemy.isBurning = true;
            player.StartCoroutine(BurnUtility.BurnEnemy(enemy, player, stacks));
        }
    }
}

public static class BurnUtility
{
    // Coroutine for the burn effect
    public static IEnumerator BurnEnemy(EnemyReceiveDamage initialEnemy, PlayerStats player, int stacks)
    {
        float damageOverTime = player.damage * 0.6f * player.burnMultiplier; // 60% of player's melee damage, + the multiplier, if it exists
        int duration = 5; // Duration of the burn effect in seconds
        int hits = duration; // Number of hits required

        while (hits > 0 && initialEnemy != null)
        {
            if (initialEnemy.isBurning && player.isBurnSpreadable)
            {
                SpreadBurn(initialEnemy.transform.position, 2.0f, player, stacks);
            }

            initialEnemy.DealDamage(damageOverTime); // Deal damage to the initial enemy

            yield return new WaitForSeconds(1f); // Wait for 1 second
            hits--;
        }

        if (hits == 0)
        {
            initialEnemy.isBurning = false;
        }
    }

    private static void SpreadBurn(Vector3 position, float radius, PlayerStats player, int stacks)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, radius);

        foreach (Collider2D col in hitColliders)
        {
            EnemyReceiveDamage nearbyEnemy = col.GetComponent<EnemyReceiveDamage>();
            if (nearbyEnemy != null && !nearbyEnemy.isBurning)
            {
                nearbyEnemy.isBurning = true;
                player.StartCoroutine(BurnEnemy(nearbyEnemy, player, stacks)); // Spread the burn effect to nearby enemies
            }
        }
    }
}

public class SackLunch : Item
{
    private const int maxHealthIncreasePerStack = 20; // Fixed amount of maximum health increase per stack

    public override string GiveName()
    {
        return "Sack Lunch";
    }

    public override string GiveDescription()
    {
        return "Increases maximum health.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        int totalHealthIncrease = 20 + (maxHealthIncreasePerStack * (stacks - 1)); // Subtracting 1 because the first stack already adds 20 health

        player.maxHealth += totalHealthIncrease;
    }
}

public class PackOfStaples : Item
{
    private const float speedIncreasePerStack = 0.075f;
    public override string GiveName()
    {
        return "Pack of Staples";
    }

    public override string GiveDescription()
    {
        return "Increases firing speed.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        float totalSpeedIncrease = 0.075f - (speedIncreasePerStack * (stacks - 1));

        player.rangedCooldown -= totalSpeedIncrease;
    }
}

public class ShinedShoes : Item
{
    private const float moveSpeedIncrease = 0.5f;
    public override string GiveName()
    {
        return "Shined Shoes";
    }

    public override string GiveDescription()
    {
        return "Increases movement speed.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        float totalSpeedIncrease = 0.5f + (moveSpeedIncrease * (stacks - 1));

        player.moveSpeed += totalSpeedIncrease;
    }
}

public class ReadingGlasses : Item
{
    private float critChance;
    public override string GiveName()
    {
        return "Reading Glasses";
    }

    public override string GiveDescription()
    {
        return "Gain a 10% chance to deal Critical Damage (2X).";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        critChance = 0.1f + (0.1f * (stacks - 1)); // 10% chance plus an extra 10% for every stack
        if (Random.value <= critChance) 
        {
            Debug.Log("crit proc! " + critChance);
            enemy.DealDamage(player.damage);
        }
    }
}

public class PaperCutter: Item
{
    private float bleedChance;
    public override string GiveName()
    {
        return "Paper Cutter";
    }

    public override string GiveDescription()
    {
        return "Gain a 20% chance to bleed enemies on hit.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        bleedChance = 0.1f + (0.1f * stacks);
        if (Random.value <= bleedChance) // Check if the burn chance is activated
        {
            Debug.Log("bleed proc!");
            player.StartCoroutine(BleedEnemy(enemy, player, stacks));
        }
    }

    // Coroutine for the bleed effect
    private IEnumerator BleedEnemy(EnemyReceiveDamage enemy, PlayerStats player, int stacks)
    {
        float damageOverTime = player.damage * 0.3f; // 30% of player's base damage
        int duration = 10; // Duration of the bleed effect
        int hits = duration; // Number of hits required

        while (hits > 0)
        {
            enemy.DealDamage(damageOverTime); // Deal damage to the enemy
            yield return new WaitForSeconds(0.5f); // Wait for half a second
            hits--;
        }
    }
}

public class BrandNewStapler : Item
{
    public override string GiveName()
    {
        return "Brand New Stapler";
    }

    public override string GiveDescription()
    {
        return "Increases base damage by 5%.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        float damageMultiplier = 1.0f + (0.05f * stacks);

        // Apply the damage increase to the player's damage
        player.damage *= damageMultiplier;
    }
}

public class CupOfCoffee : Item
{
    public override string GiveName()
    {
        return "Cup of Coffee";
    }

    public override string GiveDescription()
    {
        return "Increases melee attack speed.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        float speedMultiplier = 1.0f + (0.15f * stacks);
        player.meleeSpeed *= speedMultiplier;
    }
}

public class MarksChiliPowder : Item
{
    
    public override string GiveName()
    {
        return "Mark's Chili Powder";
    }

    public override string GiveDescription()
    {
        return "Burns are extra spicy.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        player.burnMultiplier = player.burnMultiplier + (1 * stacks);
        player.isBurnSpreadable = true;
        Debug.Log (player.burnMultiplier);
    }
}

