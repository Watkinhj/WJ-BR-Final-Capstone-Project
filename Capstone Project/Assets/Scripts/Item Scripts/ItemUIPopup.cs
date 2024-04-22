using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUIPopup : MonoBehaviour
{
    // The stuff we need
    public string itemName;
    public Image newSprite;
    public string itemDesc;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;
    public Image itemSprite;
    public Canvas itemUIPopupCanvas;

    public GameObject targetItem;

    private void Start()
    {
        itemUIPopupCanvas.enabled = false;
    }

    public void RunItemPopup()
    {
        StartCoroutine(ItemPopupAnimation());
    }
    public void UpdateItemPopup()
    {
        UpdateItemInfo();
    }
    

    public void SetItemInfo(Item item)
    {
        itemName = item.GiveName();
        itemDesc = item.GiveDescription();

        //setting color based on rarity
        switch (item.rarity)
        {
            case ItemRarity.Common:
                itemNameText.color = Color.white; // Color for common items
                break;
            case ItemRarity.Uncommon:
                itemNameText.color = Color.green; // Color for uncommon items
                break;
            case ItemRarity.Legendary:
                itemNameText.color = Color.red; // Color for legendary items
                break;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        // Assign the sprite to the itemSprite Image component
        itemSprite.sprite = sprite;
    }

    private void UpdateItemInfo()
    {
        // Update UI elements with item information
        itemNameText.text = itemName;
        itemDescText.text = itemDesc;
    }

    public IEnumerator ItemPopupAnimation()
    {
        if (itemUIPopupCanvas != null)
        {
            itemUIPopupCanvas.enabled = true;
            yield return new WaitForSeconds(5f);
            itemUIPopupCanvas.enabled = false;

        }
    }
}
