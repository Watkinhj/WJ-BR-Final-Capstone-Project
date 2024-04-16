using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBox : MonoBehaviour
{
    public List<GameObject> Items = new List<GameObject>();
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
        // Check if there are items in the list
        if (Items.Count > 0)
        {
            // Choose a random item from the list
            GameObject itemToSpawn = Items[Random.Range(0, Items.Count)];

            // Spawn the chosen item at the player's position
            Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Items list is empty. Cannot spawn any items.");
        }
    }
}
