using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public GameObject projectile;
    public float minDamage;
    public float maxDamage;
    public float projectileForce;
    public PlayerStats gm;
    // public float cooldown = 0.5f; COOLDOWN IS STORED IN PLAYERSTATS as rangedCooldown
    //public int maxAmmo = 20; MAXAMMO IS STORED IN PLAYERSTATS
    public float refillTime = 3f;

    private bool canFire = true;
    public static int currentAmmo;
    public static bool isReloading = false;

    void Start()
    {
        gm = FindObjectOfType<PlayerStats>();
        PlayerStats player = gm.GetComponent<PlayerStats>();
        currentAmmo = player.maxAmmo;
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && canFire && currentAmmo > 0)
        {
            StartCoroutine(FireProjectile());
        }
        minDamage = gm.damage / 2;
        maxDamage = (gm.damage * 1.5f) / 2;
    }

    IEnumerator FireProjectile()
    {
        PlayerStats player = gm.GetComponent<PlayerStats>();
        GameObject staple = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = staple.GetComponent<Rigidbody2D>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPos = transform.position;
        Vector2 direction = (mousePos - myPos).normalized;
        rb.velocity = direction * projectileForce;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        staple.transform.rotation = Quaternion.Euler(0, 0, angle);

        staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);
        canFire = false;
        currentAmmo--;
        UpdateAmmoText();

        yield return new WaitForSeconds(player.rangedCooldown);

        canFire = true;
        RefillAmmoIfNeeded();
    }

    public void FireAdditionalProjectile(float minDamage, float maxDamage, float projectileForce)
    {
        GameObject staple = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = staple.GetComponent<Rigidbody2D>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPos = transform.position;
        Vector2 direction = (mousePos - myPos).normalized;
        rb.velocity = direction * projectileForce;
        staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);
    }


    void UpdateAmmoText()
    {
        if (!isReloading)  // Only update if not currently reloading
        {
            if (currentAmmo > 0)
            {
                gm.ammoText.text = "Ammo: " + currentAmmo + "/" + gm.maxAmmo;
            }
            else
            {
                gm.ammoText.text = "Reloading!";
            }
        }
    }

    void RefillAmmoIfNeeded()
    {
        if (currentAmmo <= 0)
        {
            UpdateAmmoText(); // Update text to "Reloading!"
            StartCoroutine(RefillAmmo());
        }
    }

    IEnumerator RefillAmmo()
    {
        PlayerStats player = gm.GetComponent<PlayerStats>();
        isReloading = true;  // Start reloading
        int countdown = Mathf.CeilToInt(refillTime);

        while (countdown > 0)
        {
            gm.ammoText.text = "Reloading... " + countdown;
            yield return new WaitForSeconds(1);
            countdown--;
        }

        currentAmmo = player.maxAmmo;
        isReloading = false;  // End reloading
        UpdateAmmoText();
    }
}
