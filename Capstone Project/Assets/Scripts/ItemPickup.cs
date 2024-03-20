using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public Items itemDrop;
    public PlayerStats gm;

    void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        if (gm == null)
        {
            Debug.LogError("PlayerStats not found in the scene. Make sure there is a GameObject with PlayerStats attached.");
        }
        item = AssignItem(itemDrop);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerStats player = gm.GetComponent<PlayerStats>();
            AddItem(player);
            Destroy(this.gameObject);
        }
    }

    public void AddItem(PlayerStats player)
    {
        foreach(ItemList i in player.items)
        {
            if(i.name == item.GiveName())
            {
                i.stacks += 1;
                return;
            }
        }
        player.items.Add(new ItemList(item, item.GiveName(), 1));
    }

    public Item AssignItem(Items itemToAssign)
    {
        switch (itemToAssign)
        {
            case Items.HealingItem:
                return new HealingItem();
            case Items.FireDamage:
                return new FireDamage();
            default:
                return new HealingItem();

        }
    }
}

public enum Items
{
    HealingItem,
    FireDamage
}
