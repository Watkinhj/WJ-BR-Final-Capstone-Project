using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventory;
    private bool inventoryEnabled;

    private int allSlots;
    private int enabledSlots;
    private GameObject[] slot;

    public GameObject slotHolder;

    private void Start()
    {
        allSlots = 40;
        slot = new GameObject[allSlots];

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;

            // Set the slot as empty if it doesn't have an icon
            if (slot[i].GetComponent<Slot>().icon == null)
            {
                slot[i].GetComponent<Slot>().empty = true;
            }
        }
    }

    private void Update()
    {
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

}
