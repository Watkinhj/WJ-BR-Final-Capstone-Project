using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public GameObject projectile;
    public float minDamage;
    public float maxDamage;
    public float projectileForce;
    public float cooldown = 0.5f;
    //public float defaultCooldown = 0.5f;

    private bool canFire = true; 
    //private float currentCooldown;

    void Start()
    {
        /* POTENTIAL SCALING CODE, USE IF NEEDED
        currentCooldown = defaultCooldown;
        */
    }

    void Update()
    {
        // Check if the player is holding down the right mouse button and if the cooldown has expired
        if (Input.GetMouseButton(1) && canFire)
        {
            StartCoroutine(FireProjectile());
        }
    }

    IEnumerator FireProjectile()
    {
        // Create a new projectile and get its Rigidbody2D component
        GameObject staple = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = staple.GetComponent<Rigidbody2D>();

        // Calculate the direction to shoot
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPos = transform.position;
        Vector2 direction = (mousePos - myPos).normalized;

        // Apply force to the projectile
        rb.velocity = direction * projectileForce;

        // Assign a random damage value to the projectile
        staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);

        // Set the canFire flag to false to prevent shooting until the cooldown has expired
        canFire = false;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(cooldown);

        // Set the canFire flag to true to allow shooting again
        canFire = true;
    }

    /* POTENTIAL SCALING CODE, USE IF NEEDED
    // Method to update the cooldown value
    public void SetCooldown(float newCooldown)
    {
        currentCooldown = newCooldown;
    }
    */
}