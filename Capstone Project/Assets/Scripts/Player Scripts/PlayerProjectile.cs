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
        staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);
        canFire = false;
        currentAmmo--;

        yield return new WaitForSeconds(player.rangedCooldown);

        canFire = true;
        RefillAmmoIfNeeded();
    }

    IEnumerator RefillAmmo()
    {
        PlayerStats player = gm.GetComponent<PlayerStats>();
        yield return new WaitForSeconds(refillTime);
        currentAmmo = player.maxAmmo;
    }

    void RefillAmmoIfNeeded()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(RefillAmmo());
        }
    }
}
