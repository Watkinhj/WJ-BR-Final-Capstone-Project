using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        if (animator != null)
        {
            int animationIndex = Random.Range(0, 3); // Randomly pick an animation index
            animator.SetInteger("AnimationIndex", animationIndex); // Set the animation index
            Debug.Log(animationIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" ||
            collision.name == "EnemyProjectile" ||
            collision.name == "Detection Zone" ||
            collision.name == "PlayerHurtbox" ||
            collision.name == "MeleeHitbox" ||
            collision.tag == "Walkable")
        {
            // If the collision is with any of the above, just return without doing anything.
            return;
        }

        // If the collision is with the player, deal damage.
        if (collision.tag == "Player")
        {
            PlayerStats.playerStats.DealDamage(damage);
        }

        // Destroy the game object in all other cases
        Destroy(gameObject);
    }
}
