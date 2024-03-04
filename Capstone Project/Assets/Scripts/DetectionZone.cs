using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public string tagTarget = "Player";

    public List<Collider2D> detectedObjs = new List<Collider2D>();

    public Collider2D col;

    public Transform enemyParent;

    void Update()
    {
        // Check if the parent object exists
        if (enemyParent != null)
        {
            // Set the child object's position to match the parent object's position
            transform.position = enemyParent.position;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == tagTarget)
        {
            detectedObjs.Add(collider);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == tagTarget)
        {
            //detectedObjs.Remove(collider);
        }
    }
}
