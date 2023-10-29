using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform target;
    public GameObject prefabToSpawn;
    public float minDistance = 10.0f;
    public float minSpawnInterval = 2.0f;
    public float maxSpawnInterval = 5.0f;
    public float groupSpacing = 1.0f; // Spacing between enemies in a group
    public int difficulty = 1; // Adjust difficulty level
    IEnumerator SpawnEnemyGroup(int numEnemies, Vector3 SpawnPosition)
    {
        float angleStep = 360f / numEnemies;
        float radius = 2f; // Adjust the radius as needed

        for (int i = 0; i < numEnemies; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 spawnPosition = SpawnPosition + new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }
    }
    void Start()
    {
        StartCoroutine(SpawnObjectPeriodically());
    }

    IEnumerator SpawnObjectPeriodically()
    {
        while (true)
        {
            Vector3 spawnPosition;

            do
            {
                spawnPosition = GetRandomSpawnPosition();
            } while (RaycastToTarget(spawnPosition));

            if (Random.Range(0, 10) < difficulty)
            {
                int numEnemiesInGroup = Random.Range(3, 2 * difficulty); // Adjust the range as needed
                StartCoroutine(SpawnEnemyGroup(numEnemiesInGroup, spawnPosition));
            }
            else Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isValid = false;

        while (!isValid)
        {
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(minDistance, minDistance * 2); // Ensure at least 10 units away

            float radians = randomAngle * Mathf.Deg2Rad;
            spawnPosition = new Vector3(Mathf.Cos(radians) * randomDistance, 0, Mathf.Sin(radians) * randomDistance) + target.position;

            if (Vector3.Distance(spawnPosition, target.position) >= minDistance)
            {
                isValid = true;
            }
        }

        return spawnPosition;
    }

    bool RaycastToTarget(Vector3 spawnPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, (target.position - spawnPosition).normalized, out hit, minDistance))
        {
            return true; // Hit something along the way
        }

        return false; // Didn't hit anything
    }

    public void SpawnBossByName(string name)
    {
        // Conduct boss spawning behaviour here;
    }
}
