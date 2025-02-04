using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSyatem : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int enemyCount;
        public float spawnRate;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    public int currentWaveIndex = 0;
    private bool isSpawning = false;
    public int enemiesRemaining;

    void Start()
    {
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log($"Starting Wave: {wave.waveName}");
        isSpawning = true;
        enemiesRemaining = wave.enemyCount;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        isSpawning = false;

        // Wait until all enemies in the wave are defeated before starting the next wave
        while (enemiesRemaining > 0)
        {
            yield return null;
        }

        currentWaveIndex++;
        StartNextWave();
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No spawn points or enemy prefabs assigned!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject EnemyClone = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Attach a simple listener to decrease the remaining enemy count when an enemy is destroyed
      //  EnemyClone.GetComponent<Enemy>().OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    public void HandleEnemyDestroyed()
    {
        enemiesRemaining--;
    }
}
