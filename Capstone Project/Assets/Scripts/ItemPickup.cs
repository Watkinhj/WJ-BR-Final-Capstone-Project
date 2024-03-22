using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static MicrowaveSoup;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public Items itemDrop;
    public PlayerStats gm;
    public ItemUIPopup itemUI;
    public string itemDescription;

    void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        itemUI = FindObjectOfType<ItemUIPopup>();
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
            Sprite objectSprite = GetComponent<SpriteRenderer>().sprite;

            itemUI.SetItemInfo(item);
            itemUI.SetSprite(objectSprite);
            itemUI.UpdateItemPopup();
            itemUI.RunItemPopup();

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
            case Items.GreenDrink:
                return new GreenDrink();
            case Items.MicrowaveSoup:
                return new MicrowaveSoup();
            case Items.SackLunch:
                return new SackLunch();
            case Items.PackOfStaples:
                return new PackOfStaples();
            default:
                return new GreenDrink();

        }
    }
}

public enum Items
{
    GreenDrink,
    MicrowaveSoup,
    SackLunch,
    PackOfStaples,
}
