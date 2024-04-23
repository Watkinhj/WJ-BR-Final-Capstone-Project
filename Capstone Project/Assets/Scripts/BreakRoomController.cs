using UnityEngine;
using System.Collections;

public class BreakRoomController : MonoBehaviour
{
    public Inventory playerInventory;
    public BreakRoomInventory breakRoomInventory;

    // Temporarily store current selection here
    public Sprite currentItemIcon;
    public int currentItemStacks;

    // Call this method when an item is selected in the UI
    public void SetCurrentItem(Sprite icon, int stacks)
    {
        currentItemIcon = icon;
        currentItemStacks = stacks;
    }

    public void TransferItemToBreakRoom()
    {
        if (currentItemIcon != null && playerInventory.RemoveItem(currentItemIcon, currentItemStacks))
        {
            breakRoomInventory.AddItem(currentItemIcon, currentItemStacks);
        }
        else
        {
            Debug.Log("Failed to transfer item to BreakRoom.");
        }
    }

    public void RetrieveItemFromBreakRoom()
    {
        if (currentItemIcon != null && breakRoomInventory.RemoveItem(currentItemIcon, currentItemStacks))
        {
            playerInventory.AddItem(currentItemIcon, currentItemStacks);
        }
        else
        {
            Debug.Log("Failed to retrieve item from BreakRoom.");
        }
    }
}
