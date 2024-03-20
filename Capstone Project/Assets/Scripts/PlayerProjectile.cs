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
    public static int maxAmmo = 20;
    public float refillTime = 3f;

    private bool canFire = true;
    public static int currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && canFire && currentAmmo > 0)
        {
            StartCoroutine(FireProjectile());
        }
    }

    IEnumerator FireProjectile()
    {
        GameObject staple = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D rb = staple.GetComponent<Rigidbody2D>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPos = transform.position;
        Vector2 direction = (mousePos - myPos).normalized;
        rb.velocity = direction * projectileForce;
        staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);
        canFire = false;
        currentAmmo--;

        yield return new WaitForSeconds(cooldown);

        canFire = true;
        RefillAmmoIfNeeded();
    }

    IEnumerator RefillAmmo()
    {
        yield return new WaitForSeconds(refillTime);
        currentAmmo = maxAmmo;
    }

    void RefillAmmoIfNeeded()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(RefillAmmo());
        }
    }
}
