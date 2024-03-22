using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public Canvas BossDoorPopup;

    private bool playerInsideTrigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable the UI canvas
            if (BossDoorPopup != null)
            {
                BossDoorPopup.enabled = true;
            }
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the UI canvas when the player exits the trigger
            if (BossDoorPopup != null)
            {
                BossDoorPopup.enabled = false;
            }
            playerInsideTrigger = false;
        }
    }

    private void Update()
    {
        // Check if the player is inside the trigger area and pressed the "E" key
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed 'E' inside the trigger area.");
            Destroy(gameObject);
        }
    }
}
