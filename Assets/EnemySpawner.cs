using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab you want to spawn
    public GameObject targetObject; // The target GameObject
    public float minInterval, maxInterval;
    public float minDistance;

    void SpawnWaveOfEnemies(int noToSpawn)
    {
        for(int i = 0; i < noToSpawn; i++)
        {
            SpawnRandomObject();
        }
    }

    void Start()
    {
        SpawnWaveOfEnemies(10);
    }

    void SpawnRandomObject()
    {
        // Spawn your object (assuming it has a Rigidbody component)
        GameObject spawnedObject = Instantiate(gameObject, transform.position, transform.rotation);

        // Check the distance between spawnedObject and targetObject
        float distance = Vector3.Distance(spawnedObject.transform.position, targetObject.transform.position);

        if (distance < minDistance)
        {
            // Calculate a new position that is at least minDistance away from the targetObject
            Vector3 newPosition = targetObject.transform.position + (spawnedObject.transform.position - targetObject.transform.position).normalized * minDistance;

            // Set the new position for the spawnedObject
            spawnedObject.transform.position = newPosition;
        }
    }
}
