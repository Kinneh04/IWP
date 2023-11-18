using System.Collections;
using System.Collections.Generic;
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
            float radius = 1f; // Adjust the radius as needed

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
        if (MunninsTrialManager.Instance.isCurrentlyRunningDungeon) yield break;
        while (true)
        {
            if(!AllowedToSpawn || HitmaxEnemies())
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            Vector3 spawnPosition;
            spawnPosition = GetRandomPositionWithinRange(target.position);
            if(spawnPosition == Vector3.zero)
            {
                yield return new WaitForSeconds(2);
                continue;
            }
            spawnPosition.y = E.DistanceFromFloor;


            if (Random.Range(0, 10) < difficulty && E.canSpawninGroup)
            {
                int numEnemiesInGroup = Random.Range(2, difficulty); // Adjust the range as needed
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
            yield return new WaitForSeconds(randomInterval * ((11 - difficulty) / 3));
        }
    }

    public Vector3 GetRandomPositionWithinRange(Vector3 playerPosition)
    {
        int tries = 10;
        while (tries > 0)
        {
            tries--;
            // Get a random direction
            Vector3 randomDirection = Random.onUnitSphere;

            // Calculate random distance within the range
            float randomDistance = Random.Range(minDistance, maxDistance);

            // Calculate the target position
            Vector3 targetPosition = playerPosition + randomDirection * randomDistance;

            // Raycast downwards from the target position

            if(isOnValidSurface(targetPosition))
            {
                return targetPosition;
            }
        }
        return Vector3.zero;
      
    }

    public bool isOnValidSurface(Vector3 targetPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, Vector3.down, out hit, 100))
        {
            // Check if the hit surface is not tagged "ClearFloor"
            if (hit.collider.CompareTag("ClearFloor"))
            {
            //    Debug.Log("Hit the invalid surface!");
                return false;
            }
            else
            {
              //  Debug.Log("Hit:  " + hit.collider.name);
            }
        }
        return true;
    }

    public List<EnemyScript> InstantiateRandomDungeonEnemies()
    {
        List<EnemyScript> DungeonEnemies = new List<EnemyScript>();
        int EnemyCount = Random.Range(3, 10);
        for(int i = 0; i < EnemyCount; i++)
        {
            Enemy E = TypesOfEnemies[Random.Range(0, TypesOfEnemies.Count)];

            Vector3 spawnPosition;
            spawnPosition = GetRandomPositionWithinRange(target.position);
            spawnPosition.y = E.DistanceFromFloor;

            GameObject GO = Instantiate(E.EnemyGO, spawnPosition, Quaternion.identity);
            GO.GetComponent<EnemyScript>().ratingController = PRC;
            DungeonEnemies.Add(GO.GetComponent<EnemyScript>());
        }
        return DungeonEnemies;
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
