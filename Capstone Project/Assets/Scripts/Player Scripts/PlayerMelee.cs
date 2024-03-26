using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject meleeHitbox;
    public float minDamage;
    public float maxDamage;
    public PlayerStats gm;

    //This script calculates the melee damage randomness
    private void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        PlayerStats player = gm.GetComponent<PlayerStats>();
        minDamage = player.damage;
    }

    void Update()
    {
        PlayerStats player = gm.GetComponent<PlayerStats>();
        minDamage = player.damage;
        maxDamage = player.damage * 1.5f;
        //this controls the random damage modifier
        meleeHitbox.GetComponent<MeleeHitbox>().damage = Random.Range(minDamage, maxDamage);
    }
}
