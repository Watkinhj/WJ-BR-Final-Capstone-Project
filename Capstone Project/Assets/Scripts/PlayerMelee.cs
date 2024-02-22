using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject meleeHitbox;
    public float minDamage;
    public float maxDamage;

    void Update()
    {
        //controls for melee are stored in the player controller
        meleeHitbox.GetComponent<MeleeHitbox>().damage = Random.Range(minDamage, maxDamage);
    }
}
