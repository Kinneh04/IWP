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
    public int maxEnemies = 10;
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

    public bool HitmaxEnemies()
    {
        int NoOfEnemies = 0;
        foreach(EnemyScript E in SpawnedEnemyScripts)
        {
            if (E != null) NoOfEnemies++;
        }
        if (NoOfEnemies >= maxEnemies)
            return true;
        return false;
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
                GameObject GO = Instantiate(Enemy, spawnPosition, Quaternion.identity);
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
            if(!AllowedToSpawn || HitmaxEnemies())
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            Vector3 spawnPosition;
            spawnPosition = GetRandomPositionWithinRange(target.position);
            spawnPosition.y = E.DistanceFromFloor;


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

    Vector3 GetRandomPositionWithinRange(Vector3 playerPosition)
    {
        Vector3 randomPosition = GetRandomPosition(playerPosition, minDistance, maxDistance);
        int tries = 10;
        while (!IsOnValidSurface(randomPosition) && tries > 0 || !IsClearPath(playerPosition, randomPosition) &&tries > 0) 
        {
            randomPosition = GetRandomPosition(playerPosition, minDistance, maxDistance);
            tries--;
        }

        return randomPosition;
    }


    Vector3 GetRandomPosition(Vector3 playerPosition, float minDistance, float maxDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);
        return playerPosition + randomDirection * randomDistance;
    }

    bool IsOnValidSurface(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, -Vector3.up, out hit) && hit.transform.CompareTag("Floor"))
        {
            return true;
        }
        return false;
    }

    bool IsClearPath(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, to - from, out hit, Vector3.Distance(from, to)))
        {
            if (hit.collider.tag != "Player" && hit.collider.tag != "PlayerHitbox")
            {
                return false;
            }
        }
        return true;
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
