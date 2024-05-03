using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossFightController : MonoBehaviour
{
    public GameObject BossPrefab;
    public GameObject BossHealthBar;
    public GameObject DoorLock;
    private EnemyReceiveDamage BossStats;
    public BossExitDoor ExitDoor;

    private void Start()
    {
        DoorLock.SetActive(false);
        BossHealthBar.SetActive(false);
        ExitDoor = FindObjectOfType<BossExitDoor>();
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
        if (BossPrefab != null)
        {
            BossHealthBar.SetActive(true);
            Debug.Log("Spawning Boss...");
            GameObject bossInstance = Instantiate(BossPrefab, transform.position, Quaternion.identity);
            BossStats = bossInstance.GetComponent<EnemyReceiveDamage>();
            if (BossStats != null)
            {
                BossStats.OnDeath += HandleBossDeath;
            }
        }
        else
        {
            Debug.LogError("No Boss prefab assigned in the BossFightController!");
        }
    }

    private void HandleBossDeath()
    {
        Debug.Log("Boss has died.");
        ExitDoor.bossIsDead = true;
        if (BossStats != null)
        {
            BossStats.OnDeath -= HandleBossDeath; // Unsubscribe to avoid memory leaks
        }
    }

    private void DespawnTrigger()
    {
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
}
