using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Turret Settings")]
    public float detectionRange = 15f; // Detection range for enemies
    public float rotationSpeed = 5f;   // Rotation speed when facing enemies
    public int damage = 10;            // Damage dealt to enemies
    public int hitPoints = 100;

    [Header("Shooting Settings")]
    public float fireRate = 1f;        // Time between shots
    private float fireCooldown = 0f;   // Timer to handle fire rate

    private Transform targetEnemy;     // Current target
    private bool isDestroyed = false;  // Flag to check if turret is destroyed

    void Update()
    {
        if (isDestroyed) return;

        // Find the closest enemy in range
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            // Rotate turret to face the enemy
            RotateTurretToEnemy();

            // Fire at enemy
            HandleShooting();
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Dummie");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance && distanceToEnemy <= detectionRange)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        // Update target enemy
        targetEnemy = closestEnemy;
    }

    void RotateTurretToEnemy()
    {
        Vector3 direction = targetEnemy.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            ShootAtEnemy();
            fireCooldown = 1f / fireRate; // Reset cooldown based on fire rate
        }
    }

    void ShootAtEnemy()
    {
       // Placeholder for dealing damage

       Debug.Log("Turret shooting at " + targetEnemy.name + " for " + damage + " damage.");

        //Simulate enemy taking damage

       Enemy enemyHealth = targetEnemy.GetComponent<Enemy>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        hitPoints -= amount;
        Debug.Log("Turret took " + amount + " damage. Remaining HP: " + hitPoints);

        if (hitPoints <= 0)
        {
            DestroyTurret();
        }
    }

    void DestroyTurret()
    {
        isDestroyed = true;
        Debug.Log("Turret destroyed!");
        // Optional: Add explosion VFX or disable game object
        Destroy(gameObject);
    }

    // Draw the detection radius in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set gizmo color
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Draw the detection radius
    }
}