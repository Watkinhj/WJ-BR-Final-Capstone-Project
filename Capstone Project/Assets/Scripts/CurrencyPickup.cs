using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public enum PickupObject {COIN};
    public PickupObject currentObject;
    public int pickupQuantity;

    private void OnTriggerEnter2D(Collider2D other)
    {
        print(other);
        if(other.name == "Player")
        {
            PlayerStats.playerStats.AddCurrency(this);
            Destroy(gameObject);
        }
    }
}
