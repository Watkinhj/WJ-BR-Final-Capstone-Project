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

    public virtual void UpdateStats(PlayerStats player, int stacks)
    {

    }

    public virtual void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {

    }

    public virtual void UpdateRanged(PlayerProjectile player, int stacks)
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
            player.StartCoroutine(BurnEnemy(enemy, player, stacks));
        }
    }

    // Coroutine for the burn effect
    private IEnumerator BurnEnemy(EnemyReceiveDamage enemy, PlayerStats player, int stacks)
    {
        float damageOverTime = player.damage * 0.6f; // 60% of player's damage
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

    public override void UpdateStats(PlayerStats player, int stacks)
    {
        player.maxHealth += 20 + (maxHealthIncreasePerStack * stacks);
    }
}

public class PackOfStaples : Item
{
    public override string GiveName()
    {
        return "Pack of Staples";
    }

    public override string GiveDescription()
    {
        return "Increases firing speed.";
    }

    public override void UpdateRanged(PlayerProjectile player, int stacks)
    {
        player.cooldown -= 0.075f + (0.075f + stacks);
    }
}
