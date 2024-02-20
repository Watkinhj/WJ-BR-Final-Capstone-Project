using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject meleeHitbox;
    public float minDamage;
    public float maxDamage;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Melee Attack!");
        }
    }
}
