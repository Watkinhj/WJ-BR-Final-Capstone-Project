using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakRoomInventory : MonoBehaviour
{
    public GameObject inventory;
    private bool inventoryEnabled;

    private int allSlots;
    private int enabledSlots;
    private GameObject[] slot;

    public GameObject SlotHolder;

    private void Start()
    {
        allSlots = 25;
        slot = new GameObject[allSlots];

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = SlotHolder.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            inventoryEnabled = !inventoryEnabled;
        if (inventoryEnabled == true)
        {
            inventory.SetActive(true);
        } else
        {
            inventory.SetActive(false);
        }
    }

    public void AddItem(Sprite itemIcon, int itemStacks)
    {
        for (int i = 0; i < allSlots; i++)
        {
            Slot currentSlot = slot[i].GetComponent<Slot>();

            if (currentSlot.empty)
            {
                currentSlot.icon = itemIcon;
                currentSlot.itemStacks = itemStacks;
                currentSlot.UpdateSlot();
                currentSlot.empty = false;
                return;
            }
            else if (currentSlot.icon == itemIcon)
            {
                currentSlot.itemStacks += itemStacks;
                currentSlot.UpdateSlot();
                return;
            }
        }
    }

    public bool RemoveItem(Sprite itemIcon, int itemStacks)
    {
        for (int i = 0; i < allSlots; i++)
        {
            Slot currentSlot = slot[i].GetComponent<Slot>();
            if (!currentSlot.empty && currentSlot.icon == itemIcon)
            {
                if (currentSlot.itemStacks >= itemStacks)
                {
                    currentSlot.itemStacks -= itemStacks;
                    if (currentSlot.itemStacks == 0)
                    {
                        currentSlot.icon = null;
                        currentSlot.empty = true;
                    }
                    currentSlot.UpdateSlot();
                    return true;
                }
            }
        }
        return false; // Not enough items to remove or item not found
    }
}
