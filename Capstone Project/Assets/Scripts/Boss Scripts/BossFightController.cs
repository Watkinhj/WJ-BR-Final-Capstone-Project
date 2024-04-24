using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossFightController : MonoBehaviour
{
    public GameObject Boss;
    public GameObject BossHealthBar;
    public GameObject DoorLock;
    public EnemyReceiveDamage BossStats;
    public BossExitDoor ExitDoor;

    private void Start()
    {
        DoorLock.SetActive(false);
        BossHealthBar.SetActive(false);
        ExitDoor = BossExitDoor.FindObjectOfType<BossExitDoor>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered the boss room. Sealing door...");
            StartCoroutine(StartBossFight());
        }
    }

    private IEnumerator StartBossFight()
    {
        DoorLock.SetActive(true);
        DespawnTrigger();
        yield return new WaitForSeconds(1f);
        if (Boss != null)
        {
            BossHealthBar.SetActive(true);
            Instantiate(Boss, transform.position, Quaternion.identity);
            BossStats = Boss.GetComponent<EnemyReceiveDamage>();
        }
        else
        {
            Debug.LogError("No Boss assigned in the BossFightController!");
        }
    }

    private void DespawnTrigger()
    {
        //This should convert the collider into a
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogError("No Collider2D found on the object!");
        }
    }

    private void TurnOnHealthBar()
    {
        Debug.Log("Turning on the boss health bar");
        BossHealthBar.SetActive(true);
    }

    private void Update()
    {
        if (BossStats != null)
        {
            if (EnemyReceiveDamage.isDead)
            {
                ExitDoor.bossIsDead = true;
            }
        }
    }
}

