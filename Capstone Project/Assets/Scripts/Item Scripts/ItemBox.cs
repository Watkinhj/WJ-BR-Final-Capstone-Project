using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBox : MonoBehaviour
{
    public List<GameObject> commonItems = new List<GameObject>();
    public List<GameObject> uncommonItems = new List<GameObject>();
    public List<GameObject> legendaryItems = new List<GameObject>();
    public Canvas ItemBoxPopup;
    public TMP_Text ItemBoxText;
    public int ItemBoxCost;
    private bool playerInsideTrigger = false;

    void Start()
    {
        ItemBoxText.text = "$" + ItemBoxCost.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable the UI canvas
            if (ItemBoxPopup != null)
            {
                ItemBoxPopup.enabled = true;
            }
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the UI canvas when the player exits the trigger
            if (ItemBoxPopup != null)
            {
                ItemBoxPopup.enabled = false;
            }
            playerInsideTrigger = false;
        }
    }

    private void Update()
    {
        // Check if the player is inside the trigger area and pressed the "E" key
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerStats.playerStats.credits >= ItemBoxCost)
            {
                Debug.Log("Player pressed 'E' inside the trigger area.");
                SpawnRandomItem();
                PlayerStats.playerStats.credits -= ItemBoxCost;
                PlayerStats.playerStats.UpdateCurrency();
                Destroy(gameObject);
            }
        }
    }

    private void SpawnRandomItem()
    {
        float roll = Random.Range(0f, 100f);
        List<GameObject> selectedList;

        // Choose the list based on rarity probability
        if (roll < 63f) // 63% chance for Common items
            selectedList = commonItems;
        else if (roll < 99f) // 36% chance for Uncommon items
            selectedList = uncommonItems;
        else // 1% chance for Legendary items
            selectedList = legendaryItems;

        // Check if there are items in the selected list
        if (selectedList.Count > 0)
        {
            // Choose a random item from the selected list
            GameObject itemToSpawn = selectedList[Random.Range(0, selectedList.Count)];
            // Spawn the chosen item at the player's position
            Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Selected rarity list is empty. Cannot spawn any items.");
        }
    }
}
