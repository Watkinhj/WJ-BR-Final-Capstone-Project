using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]

public enum ItemRarity
{
    Common,
    Uncommon,
    Legendary
}
public abstract class Item
{
    public ItemRarity rarity;
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
    public GreenDrink()
    {
        rarity = ItemRarity.Common; 
    }
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

    public MicrowaveSoup()
    {
        rarity = ItemRarity.Uncommon;
    }
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
        if (player.CheckChanceWithLuckyPen(burnChance)) // Check if the burn chance is activated
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
            initialEnemy.BurnEffect(initialEnemy.burnEffectPrefab);

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
                nearbyEnemy.BurnEffect(nearbyEnemy.burnEffectPrefab);
            }
        }
    }
}

public class SackLunch : Item
{
    private const int maxHealthIncreasePerStack = 20; // Fixed amount of maximum health increase per stack

    public SackLunch()
    {
        rarity = ItemRarity.Common;
    }
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

    public PackOfStaples()
    {
        rarity = ItemRarity.Common;
    }
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

    public ShinedShoes()
    {
        rarity = ItemRarity.Common;
    }
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

    public ReadingGlasses()
    {
        rarity = ItemRarity.Common;
    }
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
        if (player.CheckChanceWithLuckyPen(critChance))
        {
            Debug.Log("crit proc! " + critChance);
            enemy.ShowEffect(enemy.critHitEffectPrefab);
            bool hasSensitiveFiles = false;
            int sensitiveFilesStacks = 0;
            int critMultiplier = 0;
            foreach (ItemList item in player.items)
            {
                if (item.item.GetType() == typeof(SensitiveFiles))
                {
                    hasSensitiveFiles = true;
                    sensitiveFilesStacks += item.stacks;
                    critMultiplier += item.stacks * 2;
                }
            }
            if (hasSensitiveFiles)
            {
                Debug.Log("Sensitive files proc!");
                enemy.DealDamage(player.damage); //2nd hit of 2x hit
                enemy.DealDamage(player.damage * critMultiplier); //remaining hits on critmultiplier
            }
            else
            {
                enemy.DealDamage(player.damage); //2X damage hit
            }

            // Check if the player has SugaredSoda for additional healing
            bool hasSugaredSoda = false;
            int sugaredSodaStacks = 0;
            foreach (ItemList item in player.items)
            {
                if (item.item.GetType() == typeof(SugaredSoda))
                {
                    hasSugaredSoda = true;
                    sugaredSodaStacks += item.stacks;
                }
            }

            if (hasSugaredSoda)
            {
                player.HealCharacter(6 + (1 * sugaredSodaStacks));
            }
        }
    }
}

public class PaperCutter: Item
{
    private float bleedChance;

    public PaperCutter()
    {
        rarity = ItemRarity.Common;
    }
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
        if (player.CheckChanceWithLuckyPen(bleedChance)) // Check if the burn chance is activated
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
    public BrandNewStapler()
    {
        rarity = ItemRarity.Common;
    }
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
    public CupOfCoffee()
    {
        rarity = ItemRarity.Common;
    }
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
    public MarksChiliPowder()
    {
        rarity = ItemRarity.Legendary;
    }
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

public class DoubleSidedTape : Item
{
    private float slowChance;
    private float originalSpeed;

    public DoubleSidedTape()
    {
        rarity = ItemRarity.Common;
    }
    public override string GiveName()
    {
        return "Double Sided Tape";
    }

    public override string GiveDescription()
    {
        return "Chance to temporarily slow enemies on hit.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        slowChance = 0.05f + (0.1f * stacks);
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null)
        {
            // Only set originalSpeed if it hasn't been set yet
            if (originalSpeed == 0)
            {
                originalSpeed = enemyBehavior.moveSpeed;
            }

            if (player.CheckChanceWithLuckyPen(slowChance))
            {
                player.StartCoroutine(SlowEnemy(player, enemyBehavior, stacks));
            }
                
        }
        else
        {
            Debug.Log("enemyBehavior not found!");
        }
    }

private IEnumerator SlowEnemy(PlayerStats player, EnemyBehavior enemy, int stacks)
    {
        enemy.moveSpeed /= 2;
        Debug.Log("Setting enemy move speed to half");

        //Wait 5 seconds
        yield return new WaitForSeconds(5f);

        // Return the enemy's speed to normal
        Debug.Log("Setting enemy move speed to original");
        enemy.moveSpeed = originalSpeed;
    }
}

public class BubbleWrap : Item
{
    private float damageReductionPercentage = 0.05f;

    public BubbleWrap()
    {
        rarity = ItemRarity.Common;
    }
    public override string GiveName()
    {
        return "Bubble Wrap";
    }

    public override string GiveDescription()
    {
        return "Increases Damage Resistance by 5%.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        damageReductionPercentage = 0.05f * stacks;
        player.damageReductionPercentage += damageReductionPercentage;
    }
}

public class FaultyHardDrive : Item
{
    private float shockChance;
    private const float shockDamagePercentage = 0.6f;

    public FaultyHardDrive()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Faulty Hard Drive";
    }

    public override string GiveDescription()
    {
        return "Chance to do shock damage that chains to nearby enemies.";
    }
    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        shockChance = (0.1f + (0.1f * stacks));
        if (player.CheckChanceWithLuckyPen(shockChance)) // Check if the shock chance is activated
        {
            Debug.Log("shock proc!");
            //shock the enemy for 60% of base damage
            enemy.ShowEffect(enemy.shockEffectPrefab);
            enemy.DealDamage(player.damage * shockDamagePercentage);
            //Find nearby enemies to the enemy
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, 5f); // Adjust radius as needed
            foreach (Collider2D col in nearbyEnemies)
            {
                EnemyReceiveDamage nearbyEnemy = col.GetComponent<EnemyReceiveDamage>();
                if (nearbyEnemy != null && nearbyEnemy != enemy) // Excludes the initial enemy
                {
                    nearbyEnemy.DealDamage(player.damage + (player.damage * shockDamagePercentage));
                }
            }
        }
    }
}

public class PerformanceBoosters : Item
{
    public PerformanceBoosters()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Performance Boosters";
    }

    public override string GiveDescription()
    {
        return "Heals a small amount on hit.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        player.HealCharacter(1 + (1 * stacks));
    }
}

public class SugaredSoda : Item
{
    public SugaredSoda()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Sugared Soda";
    }

    public override string GiveDescription()
    {
        return "Heal for every critical hit.";
    }

    //functionality is stored in the readingglasses item.
}

public class LightweightCoat : Item
{
    public LightweightCoat()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Lightweight Coat";
    }

    public override string GiveDescription()
    {
        return "Gives a chance to dodge incoming damage.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        player.UpdateLightweightCoatStatus();
    }
}

public class SavingsJar : Item
{
    public SavingsJar()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Savings Jar";
    }

    public override string GiveDescription()
    {
        return "Increases your credit gain by 15%.";
    }

    //functionality is stored in the playerstats script. 
}

public class MakeshiftSlingshot : Item
{
    private float additionalProjectileChance;

    public MakeshiftSlingshot()
    {
        rarity = ItemRarity.Uncommon;
    }
    public override string GiveName()
    {
        return "Makeshift Slingshot";
    }

    public override string GiveDescription()
    {
        return "Gives a chance to fire an additional projectile on hit.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        additionalProjectileChance = 0.1f + (0.05f * stacks);
        if (player.CheckChanceWithLuckyPen(additionalProjectileChance))
            {
            // Find the player GameObject
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                // Get the PlayerProjectile component from the player GameObject
                PlayerProjectile playerProjectile = playerObject.GetComponent<PlayerProjectile>();
                if (playerProjectile != null)
                {
                    // Call the FireAdditionalProjectile method
                    playerProjectile.FireAdditionalProjectile(playerProjectile.minDamage, playerProjectile.maxDamage, playerProjectile.projectileForce);
                }
                else
                {
                    Debug.LogWarning("PlayerProjectile component not found on player GameObject.");
                }
            }
            else
            {
                Debug.LogWarning("Player GameObject not found.");
            }
        }
    }
}

public class BigRedBinder : Item
{
    public override string GiveName()
    {
        return "Big Red Binder";
    }

    public override string GiveDescription()
    {
        return "Chance to reflect incoming damage.";
    }
}

public class LuckyPen : Item
{
    public LuckyPen()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Lucky Pen";
    }

    public override string GiveDescription()
    {
        return "You feel very lucky.";
    }

    //functionality is stored in playerstats
}

public class TimeCard : Item
{
    public TimeCard()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Time Card";
    }

    public override string GiveDescription()
    {
        return "Receive an increase to ALL STATS at 5:00.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        player.UpdateTimeCard();
    }
}

public class SensitiveFiles : Item
{
    public SensitiveFiles()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Sensitive Files";
    }

    public override string GiveDescription()
    {
        return "Your critical damage is DOUBLED.";
    }

    //functionality is stored in reading glasses item
}

public class PinkSlip : Item
{
    private float oneShotChance;

    public PinkSlip()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Pink Slip";
    }

    public override string GiveDescription()
    {
        return "Gain a small chance to INSTANTLY KILL any non-boss enemy.";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        oneShotChance = 0.05f * stacks;
        if (player.CheckChanceWithLuckyPen(oneShotChance))
        {
            if (!enemy.isBoss)
            {
                Debug.Log("YOU'RE FIRED!");
                enemy.DealDamage(999999);
            }
        }

    }
}

public class PrizeGavel : Item
{
    public PrizeGavel()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Prize Gavel";
    }

    public override string GiveDescription()
    {
        return "Increases the strength of all knockback effects.";
    }
    public override void OnPickup(PlayerStats player, int stacks)
    {

        // Apply the damage increase to the player's damage
        player.knockbackForce = 10 + (2 * stacks);
    }
}

public class FavoriteTie : Item
{
    public FavoriteTie()
    {
        rarity = ItemRarity.Legendary;
    }
    public override string GiveName()
    {
        return "Favorite Tie";
    }

    public override string GiveDescription()
    {
        return "Oh, I feel GREAT.";
    }

    public override void OnPickup(PlayerStats player, int stacks)
    {
        float damageMultiplier = 1f + (1 * stacks);

        // Apply the damage increase to the player's damage
        player.damage *= damageMultiplier;
    }
}

