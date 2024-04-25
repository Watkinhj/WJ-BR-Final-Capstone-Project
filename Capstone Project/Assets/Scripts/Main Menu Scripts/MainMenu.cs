using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Animator fadeToBlack;
    public Canvas gameStartPopup;

    // Start the game coroutine
    public void StartGame()
    {
        StartCoroutine(GameStart());
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    private IEnumerator GameStart()
    {
        
        gameStartPopup.enabled = true;

        // Wait for a few seconds before fading out
        yield return new WaitForSeconds(3);

        // Trigger the fade out animation
        fadeToBlack.SetTrigger("fadeToBlack");

        // Load the next scene after the fade out animation completes
        yield return new WaitForSeconds(1); // Adjust this delay based on your fade out animation duration
        SceneManager.LoadScene(1);
    }
}
