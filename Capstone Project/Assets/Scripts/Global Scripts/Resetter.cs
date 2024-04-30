using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Resetter : MonoBehaviour
{
    public PlayerStats gameGM;
    public DontDestroy transferables;

    private void Start()
    {
        gameGM = GameObject.FindObjectOfType<PlayerStats>();
        transferables = GameObject.FindObjectOfType<DontDestroy>();

        if (gameGM != null)
        {
            Destroy(gameGM.gameObject);
        }

        if (transferables != null)
        {
            Destroy(transferables.gameObject);
        }

        StartCoroutine(GoHome());
    }

    private IEnumerator GoHome()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(0);
    }
}
