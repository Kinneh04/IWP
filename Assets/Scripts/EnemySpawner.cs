using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform target;
    public List<Enemy> TypesOfEnemies = new List<Enemy>();
    public float minDistance = 10.0f, maxDistance = 10.0f;
    public float minSpawnInterval = 2.0f;
    public float maxSpawnInterval = 5.0f;
    public float groupSpacing = 1.0f; // Spacing between enemies in a group
    public int difficulty = 1; // Adjust difficulty level
    public bool AllowedToSpawn;
    private static EnemySpawner _instance;
    public PlayerRatingController PRC;
    public List<GameObject> SpawnedEnemies = new List<GameObject>();
    public List<EnemyScript> SpawnedEnemyScripts = new List<EnemyScript>();
    public FirstPersonController FPC;
    public Transform player;
    public void CheckForBehindPlayer()
    {
       
        foreach(EnemyScript ES in SpawnedEnemyScripts)
        {
            if (!ES || ES.enemyType == EnemyScript.EnemyType.Small) continue;
            Vector3 direction = transform.position - player.position;
            float angle = Vector3.Angle(direction, player.forward);

            if (angle < 35f && Vector3.Distance(ES.transform.position, player.position) < 5) // Adjust this angle to fit your needs
            {
                FPC.BehindIndicator.SetActive(true);
                return;
            }
            
        }
        FPC.BehindIndicator.SetActive(false);
    }
    public void Cleanup()
    {
        RemoveAllEnemies();
        AllowedToSpawn = false;
        StopAllCoroutines();
    }

    private void Awake()
    {
      //  player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
        // Ensure there's only one instance of this object
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

  
    }

    public void StartEnemySpawning()
    {
        foreach (Enemy E in TypesOfEnemies)
        {
            StartCoroutine(SpawnObjectPeriodically(E));
        }
        AllowedToSpawn = true;
    }
    public static EnemySpawner Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemySpawner>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MainMenuManager");
                    _instance = singletonObject.AddComponent<EnemySpawner>();
                }
            }

            return _instance;
        }
    }

    //private void Update()
    //{
    //    if(AllowedToSpawn && SpawnedEnemyScripts.Count > 0)
    //        CheckForBehindPlayer();
    //}

    public void RemoveAllEnemies()
    {
        foreach(GameObject GO in SpawnedEnemies)
        {
            if(GO)
            {
                Destroy(GO);
            }
        }
        SpawnedEnemyScripts.Clear();
        SpawnedEnemies.Clear();
    }


    IEnumerator SpawnEnemyGroup(GameObject Enemy, int numEnemies, Vector3 SpawnPosition)
    {
        float angleStep = 360f / numEnemies;
        float radius = 2f; // Adjust the radius as needed

        for (int i = 0; i < numEnemies; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 spawnPosition = SpawnPosition + new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);
            GameObject GO =Instantiate(Enemy, spawnPosition, Quaternion.identity);
            GO.GetComponent<EnemyScript>().ratingController = PRC;
            SpawnedEnemies.Add(GO);
            SpawnedEnemyScripts.Add(GO.GetComponent<EnemyScript>());
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator SpawnObjectPeriodically(Enemy E)
    {
        while (true)
        {
            if(!AllowedToSpawn)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            Vector3 spawnPosition;


            do
            {
                spawnPosition = GetRandomSpawnPosition();
                spawnPosition.y += E.DistanceFromFloor;
            } while (RaycastToTarget(spawnPosition));

            if (Random.Range(0, 10) < difficulty && E.canSpawninGroup)
            {
                int numEnemiesInGroup = Random.Range(3, difficulty); // Adjust the range as needed
                StartCoroutine(SpawnEnemyGroup(E.EnemyGO, numEnemiesInGroup, spawnPosition));
            }
            else
            {
                GameObject GO = Instantiate(E.EnemyGO, spawnPosition, Quaternion.identity);
                GO.GetComponent<EnemyScript>().ratingController = PRC;
                SpawnedEnemies.Add(GO);
                SpawnedEnemyScripts.Add(GO.GetComponent<EnemyScript>());
            }

            float randomInterval = Random.Range(E.minSpawnInterval, E.maxSpawnInterval);
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
            float randomDistance = Random.Range(minDistance, maxDistance); // Ensure at least 10 units away

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
}

[System.Serializable]
public class Enemy
{
    public GameObject EnemyGO;
    public float SpawnChance;
    public bool canSpawninGroup = false;
    public float minSpawnInterval, maxSpawnInterval;
    public float DistanceFromFloor;
}
