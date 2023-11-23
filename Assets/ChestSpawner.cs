using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public List<Transform> AvailablePositions;
    public float currentChance;
    public GameObject ChestPrefab;
    public bool spawnedChest = false;
    public GameObject CurrentlySpawnedChest;
    public void IncrementChances()
    {
        if (spawnedChest) return;
        currentChance += 0.1f;
        float f = Random.Range(0f, 100f);
        if(currentChance > f)
        {
            SpawnChest();
        }
    }

    public void SpawnChest()
    {
        FirstPersonController.Instance.PopupNotif("A chest spawned");
        Transform T = AvailablePositions[Random.Range(0, AvailablePositions.Count)];
        Vector3 Pos = T.position;
        Pos.y += 0.1f;
        CurrentlySpawnedChest = Instantiate(ChestPrefab,Pos, ChestPrefab.transform.rotation);

        CurrentlySpawnedChest.GetComponent<ChestScript>().RelatedChestSpawner = this;
        spawnedChest = true;
        
    }

    public void resetValue()
    {
        currentChance = 0;
        spawnedChest = false;
    }
}
