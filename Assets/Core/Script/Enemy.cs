using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyWaveSyatem m_ews;
    public enum EnemyType { BaseTower, Tower, Player, All }

    [Header("Enemy Properties")]
    public EnemyType enemyType;
    public float health;
    public float damage;
    public float speed;
    public float stoppingDistance;
    public float attackCooldown;

    [Header("AI Navigation")]
    public NavMeshAgent navMeshAgent;
    public Transform[] targets;

    [Header("Attack Properties")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float forceMagnitude;

    private float attackTimer;
    private Transform currentTarget;

    void Start()
    {
        m_ews = GameObject.FindObjectOfType<EnemyWaveSyatem>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = stoppingDistance;

        // Initialize enemy properties based on type
        InitializeEnemyProperties();

        // Select the targets based on type
        SelectTargets();
        currentTarget = FindClosestTarget(targets);
    }

    void Update()
    {
        if (currentTarget != null)
        {
            navMeshAgent.SetDestination(currentTarget.position);

            if (Vector3.Distance(transform.position, currentTarget.position) <= stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                FaceTarget(currentTarget);

                if (attackTimer <= 0)
                {
                    Attack();
                    attackTimer = attackCooldown;
                }
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }

        attackTimer -= Time.deltaTime;
    }

    void InitializeEnemyProperties()
    {
        switch (enemyType)
        {
            case EnemyType.BaseTower:
                health = 200f;
                damage = 30f;
                attackCooldown = 2f;
                break;
            case EnemyType.Tower:
                health = 150f;
                damage = 20f;
                attackCooldown = 1.5f;
                break;
            case EnemyType.Player:
                health = 100f;
                damage = 40f;
                attackCooldown = 1f;
                break;
            case EnemyType.All:
                health = 120f;
                damage = 25f;
                attackCooldown = 1.2f;
                break;
        }
    }

    void SelectTargets()
    {
        switch (enemyType)
        {
            case EnemyType.BaseTower:
                targets = FindTargetsWithTag("Main Tower");
                break;
            case EnemyType.Tower:
                targets = CombineTargetArrays(
                    FindTargetsWithTag("Tower"),
                    FindTargetsWithTag("UltraTower"),
                    FindTargetsWithTag("UltimateTower")
                );
                break;
            case EnemyType.Player:
                targets = FindTargetsWithTag("Player");
                break;
            case EnemyType.All:
                targets = FindTargetsWithTag("Player");
                targets = FindTargetsWithTag("Main Tower");
                targets = CombineTargetArrays(
                    FindTargetsWithTag("Tower"),
                    FindTargetsWithTag("UltraTower"),
                    FindTargetsWithTag("UltimateTower")
                );
                break;
        }
    }

    Transform[] FindTargetsWithTag(string tag)
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(tag);
        Transform[] targetTransforms = new Transform[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetTransforms[i] = targetObjects[i].transform;
        }
        return targetTransforms;
    }

    Transform[] CombineTargetArrays(params Transform[][] arrays)
    {
        int totalLength = 0;
        foreach (var array in arrays)
        {
            totalLength += array.Length;
        }

        Transform[] result = new Transform[totalLength];
        int offset = 0;
        foreach (var array in arrays)
        {
            array.CopyTo(result, offset);
            offset += array.Length;
        }

        return result;
    }

    Transform FindClosestTarget(Transform[] targets)
    {
        Transform closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform target in targets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(currentPosition, target.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Attack()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * forceMagnitude, ForceMode.Impulse);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        m_ews.HandleEnemyDestroyed();
    }
}
