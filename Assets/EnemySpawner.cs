using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab you want to spawn
    public GameObject targetObject; // The target GameObject
    public float minInterval, maxInterval;


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
        if (objectToSpawn != null && targetObject != null)
        {
            Vector3 randomPosition = Random.insideUnitSphere * 10f; // Generate a random position within a 10 unit sphere
            randomPosition += targetObject.transform.position; // Offset by the target position
            randomPosition.y = 0; // Ensure the object spawns at the same Y level as the target

            Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Please assign the object to spawn and target object in the inspector.");
        }
    }
}
