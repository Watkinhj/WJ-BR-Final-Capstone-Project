using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    // This function is called when another object enters the collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Log the name of the colliding object
        Debug.Log(collision.gameObject.name + " has collided with " + gameObject.name);
    }
}