using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();
    public virtual void Update(PlayerStats player, int stacks)
    {

    }
    public virtual void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {

    }
}

public class HealingItem : Item
{
    public override string GiveName()
    {
        return "Healing Item";
    }

    public override void Update(PlayerStats player, int stacks)
    {
        player.health += 1 + (1 * stacks);
        
    }
}

public class FireDamage: Item
{
    public override string GiveName()
    {
        return "Fire Damage Item";
    }

    public override void OnHit(PlayerStats player, EnemyReceiveDamage enemy, int stacks)
    {
        enemy.health -= 10 * stacks;
    }
}