using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public PlayerStats homeScreenGM;

    // Array of text strings
    public string[] textOptions = new string[]
    {
        "Text 1",
        "Text 2",
        "Text 3",
        "Text 4",
        "Text 5",
        "Text 6",
        "Text 7",
        "Text 8",
        "Text 9",
        "Text 10",
        "Text 11",
        "Text 12"
    };

    void Start()
    {
        //DELETE THE OLD PLAYERSTATS TO RESET
        homeScreenGM = GameObject.FindObjectOfType<PlayerStats>();
        if (homeScreenGM != null )
        {
            Destroy(homeScreenGM.gameObject);
        }

        //ASSIGN TIP TEXT
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component is not assigned!");
            return;
        }

        // Pick a random index from the array
        int randomIndex = Random.Range(0, textOptions.Length);

        // Set the text of TextMeshPro component to the randomly chosen string
        textMeshPro.text = textOptions[randomIndex];
        StartCoroutine(SwapToLevel());
    }

    private IEnumerator SwapToLevel()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(2);
    }
}
