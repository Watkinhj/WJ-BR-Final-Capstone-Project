using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuDoor : MonoBehaviour
{
    public Animator animator;
    //public Animator fadeoutanimator;
    public Canvas DoorPopup;
    public TMP_Text DoorText;
    private bool playerInsideTrigger = false;
    public MainMenu MainMenuScript; // Corrected variable type

    private void Start()
    {
        // Get the MainMenu script component from the MainMenu GameObject
        MainMenuScript = MainMenu.FindObjectOfType<MainMenu>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable the UI canvas
            if (DoorPopup != null)
            {
                DoorPopup.enabled = true;
            }
            playerInsideTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the UI canvas when the player exits the trigger
            if (DoorPopup != null)
            {
                DoorPopup.enabled = false;
            }
            playerInsideTrigger = false;
        }
    }

    void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            DoorPopup.enabled = false;
            animator.SetTrigger("OpenDoor");
            // Start the game through MainMenuScript
            MainMenuScript.StartGame();
        }
    }
}
