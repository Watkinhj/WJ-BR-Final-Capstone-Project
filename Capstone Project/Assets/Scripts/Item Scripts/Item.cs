using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private const float burnChance = 0.2f; // 20% chance to burn enemies

    public override string GiveName()
    {
        return "Microwave Soup";
    }

    public override string GiveDescription()
    {
        return "Gives a chance to burn enemies.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        if (Random.value <= burnChance) // Check if the burn chance is activated
        {
            Debug.Log("burn proc!");
            player.StartCoroutine(BurnEnemy(enemy, player, stacks));
        }
    }

    // Coroutine for the burn effect
    private IEnumerator BurnEnemy(EnemyReceiveDamage enemy, PlayerStats player, int stacks)
    {
        float damageOverTime = player.damage * 0.6f; // 60% of player's melee damage
        int duration = 5 + (2 * stacks); // Duration of the burn effect in seconds
        int hits = duration; // Number of hits required

        while (hits > 0)
        {
            enemy.DealDamage(damageOverTime); // Deal damage to the enemy
            yield return new WaitForSeconds(1f); // Wait for 1 second
            hits--;
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
        //Makes it so that it adds 20 to player's health on pickup. Then, it should only add 20 more health when another of that same item is picked up.
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
