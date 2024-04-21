using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Search;
using UnityEngine;
using static MicrowaveSoup;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public Items itemDrop;
    public PlayerStats gm;
    public ItemUIPopup itemUI;
    public Inventory inventoryUI;
    public string itemDescription;
    public Sprite objectSprite;

    void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        itemUI = FindObjectOfType<ItemUIPopup>();
        inventoryUI = FindObjectOfType<Inventory>();
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
            objectSprite = GetComponent<SpriteRenderer>().sprite;

            //ui stuff
            itemUI.SetItemInfo(item);
            itemUI.SetSprite(objectSprite);
            itemUI.UpdateItemPopup();
            itemUI.RunItemPopup();
            
            AddItem(player);

            if (player != null)
            {
                item.OnPickup(player, 1); // Assuming 1 stack for now
            }

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
                inventoryUI.AddItem(objectSprite, 1);
                return;
            }
        }
        player.items.Add(new ItemList(item, item.GiveName(), 1));
        inventoryUI.AddItem(objectSprite, 1);
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
            case Items.ShinedShoes:
                return new ShinedShoes();
            case Items.ReadingGlasses:
                return new ReadingGlasses();
            case Items.PaperCutter:
                return new PaperCutter();
            case Items.BrandNewStapler:
                return new BrandNewStapler();
            case Items.CupOfCoffee:
                return new CupOfCoffee();
            case Items.MarksChiliPowder:
                return new MarksChiliPowder();
            case Items.DoubleSidedTape:
                return new DoubleSidedTape();
            case Items.BubbleWrap:
                return new BubbleWrap();
            case Items.FaultyHardDrive:
                return new FaultyHardDrive();
            case Items.PerformanceBoosters:
                return new PerformanceBoosters();
            case Items.SugaredSoda:
                return new SugaredSoda();
            case Items.LightweightCoat:
                return new LightweightCoat();
            case Items.SavingsJar:
                 return new SavingsJar();
            case Items.MakeshiftSlingshot:
                return new MakeshiftSlingshot();
            case Items.BigRedBinder:
                return new BigRedBinder();
            case Items.LuckyPen:
                return new LuckyPen();
            case Items.TimeCard:
                return new TimeCard();
            case Items.SensitiveFiles:
                return new SensitiveFiles();
            case Items.PinkSlip:
                return new PinkSlip();
            case Items.PrizeGavel:
                return new PrizeGavel();
            case Items.FavoriteTie:
                return new FavoriteTie();
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
    ShinedShoes,
    ReadingGlasses,
    PaperCutter,
    BrandNewStapler,
    CupOfCoffee,
    MarksChiliPowder,
    DoubleSidedTape,
    BubbleWrap,
    FaultyHardDrive,
    PerformanceBoosters,
    SugaredSoda,
    LightweightCoat,
    SavingsJar,
    MakeshiftSlingshot,
    BigRedBinder,
    LuckyPen,
    TimeCard,
    SensitiveFiles,
    PinkSlip,
    PrizeGavel,
    FavoriteTie,
}
