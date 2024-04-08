using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedItemsUI : MonoBehaviour
{
    public Transform itemsParent; // Assign the parent transform of your equipped items UI
    public GameObject equippedItemPrefab; // Prefab for displaying each equipped item

    private List<Sprite> equippedItemSprites = new List<Sprite>(); // List to store equipped item sprites

    // Add an item to the equipped items list and update the UI
    public void AddEquippedItem(Sprite itemSprite)
    {
        equippedItemSprites.Add(itemSprite);
        UpdateEquippedItemsUI();
    }

    // Update the equipped items UI to display all equipped item sprites
    private void UpdateEquippedItemsUI()
    {
        // Clear existing equipped items
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate and setup UI elements for each equipped item
        foreach (Sprite itemSprite in equippedItemSprites)
        {
            GameObject newItemUI = Instantiate(equippedItemPrefab, itemsParent);
            Image itemImage = newItemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = itemSprite;
            }
        }
    }
}