using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject item;
    public bool empty;
    public Sprite icon;
    public TMP_Text itemStacksText;
    public int itemStacks;

    public void Start()
    {
        itemStacksText = transform.GetChild(0).GetComponent<TMP_Text>();
        itemStacksText.gameObject.SetActive(false);
    }

    public void UpdateSlot()
    {
        this.GetComponent<Image>().sprite = icon;
        itemStacksText.gameObject.SetActive(true);
        itemStacksText.text = "x" + itemStacks;
    }
}
