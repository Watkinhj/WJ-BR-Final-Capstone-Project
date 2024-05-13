using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public enum PickupObject {COIN};
    public PickupObject currentObject;
    public int pickupQuantity;
    private AudioSource pickupSound;

    private void Start()
    {
        //Grabs the audio source to make shit work
        pickupSound = GetComponent<AudioSource>();
        if (!pickupSound)
        {
            Debug.LogError("missing audio source on pickup");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            PlayerStats.playerStats.AddCurrency(this);

            //PLAY SOUND
            pickupSound.Play();

            //DESTROY SOUND AFTER DELAY
            Destroy(gameObject, pickupSound.clip.length);
        }
    }
}
