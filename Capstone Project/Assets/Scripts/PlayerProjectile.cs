using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public GameObject projectile;
    public float minDamage;
    public float maxDamage;
    public float projectileForce;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject staple = Instantiate(projectile, transform.position, Quaternion.identity);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = transform.position;
            Vector2 direction = (mousePos - myPos).normalized;
            staple.GetComponent<Rigidbody2D>().velocity = direction * projectileForce;
            staple.GetComponent<Staple>().damage = Random.Range(minDamage, maxDamage);
        }
    }
}
