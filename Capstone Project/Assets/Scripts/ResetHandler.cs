using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetHandler : MonoBehaviour
{
    public Animator fadeToBlack;

    public void ResetGame()
    {
        StartCoroutine(ResetCoroutine());
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(2f);
        fadeToBlack.SetTrigger("fadeToBlack");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(5);
    }
}
