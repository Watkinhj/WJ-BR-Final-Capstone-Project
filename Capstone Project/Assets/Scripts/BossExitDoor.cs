using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BossExitDoor : MonoBehaviour
{
    public bool boss1;
    public bool boss2;
    public bool boss3;

    private Animator animator;
    public Animator fadeToBlack;

    private BoxCollider2D trigger;
    private bool playerInsideTrigger = false;
    public Canvas DoorPopup;

    public bool bossIsDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossIsDead)
            {
                // Enable the UI canvas
                if (DoorPopup != null)
                {
                    DoorPopup.enabled = true;
                }
                playerInsideTrigger = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossIsDead)
            {
                if (DoorPopup != null)
                {
                    DoorPopup.enabled = false;
                }
                playerInsideTrigger = false;
            }
        }
    }

    void Update()
    {
        if (playerInsideTrigger && bossIsDead && Input.GetKeyDown(KeyCode.E))
        {
            DoorPopup.enabled = false;
            animator.SetTrigger("OpenDoor");
            fadeToBlack.SetTrigger("fadeToBlack");
            StartCoroutine(fadeOut());
        }
    }

    private IEnumerator fadeOut()
    {
        fadeToBlack.SetTrigger("fadeToBlack");
        yield return new WaitForSeconds(2);
        if (boss1)
        {
            SceneManager.LoadScene(3);
        }
        if (boss2)
        {
            SceneManager.LoadScene(4);
        }
        if (boss3)
        {
            //implement later
            yield break;
        }
    }
}
