using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    int healthRestore;

    private void Start()
    {
        healthRestore = Random.Range(5, 10);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            Debug.Log(healthRestore);
            PlayerStats.playerStats.HealCharacter(healthRestore);
            Destroy(gameObject);
        }
    }
}
