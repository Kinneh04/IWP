using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public List<GameObject> SmallDungeons, MediumDungeons, BigDungeons, BossDungeons = new List<GameObject>();

    public bool bossRoomhasSpawned = false;
    public List<DungeonScript> SpawnedDungeonRooms = new List<DungeonScript>();
    public Transform PlayerTransform;
    public int RoomCount;
    public bool isTeleporting = false;
    public DungeonScript currentDungeon;
    public GameObject BeginningRoomGO;

    public void EnableEnemiesInRoom()
    {
        foreach(EnemyScript ES in currentDungeon.EnemiesInDungeon)
        {
            ES.gameObject.SetActive(true);
        }
    }
    public void DisableEnemiesInRoom()
    {
        foreach (EnemyScript ES in currentDungeon.EnemiesInDungeon)
        {
            ES.gameObject.SetActive(false);
        }
    }
    public void SpawnBeginningRoom()
    {
        GameObject GO = Instantiate(BeginningRoomGO);
        DungeonScript dungeonScript = BeginningRoomGO.GetComponent<DungeonScript>();
        dungeonScript.ExitDoor.TargetRoomIndex = 1;
        dungeonScript.ExitDoor.isExit = true;
        currentDungeon = dungeonScript;
    }

    public void TeleportPlayerToRoomViaIndex(int index, bool isExit)
    {

        foreach(DungeonScript DS in SpawnedDungeonRooms)
        {
            if(DS.RoomIndex == index)
            {
                if (!isExit)
                {
                    PlayerTransform.position = DS.ExitDoor.TeleportTransform.position;
                    // Back from enter
                }
                else
                {
                    PlayerTransform.position = DS.EnterDoor.TeleportTransform.position;
                    // to entrance
                }
                currentDungeon = DS;
                return;
            }
        }
       
    }

    public IEnumerator TeleportToRoomIndex(int index, bool isExit)
    {
        if (isTeleporting) yield break;
        isTeleporting = true;
        BlackscreenController.Instance.fadeSpeed = 5;
        BlackscreenController.Instance.FadeOut();
        if (currentDungeon) DisableEnemiesInRoom();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        TeleportPlayerToRoomViaIndex(index, isExit);   
        yield return new WaitForSeconds(.1f);
        BlackscreenController.Instance.FadeIn();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        BlackscreenController.Instance.fadeSpeed = 3;
        isTeleporting = false;
    }

    public enum RoomSize
    {
        small, medium, large, random
    }

    public enum EnemyCount
    {
        little, normal, horde
    }
    public RoomSize SelectedRoomSize;
    public EnemyCount SelectedEnemyCount;

    public void SpawnDungeonRooms()
    {

       SpawnBeginningRoom();
        switch(SelectedRoomSize)
        {
            case RoomSize.small:
                for(int i = 0; i < RoomCount; i++)
                {
                    GameObject GO = Instantiate(SmallDungeons[Random.Range(0, SmallDungeons.Count)]);
                    Vector3 Position = GO.transform.position;
                    Position.x += 100 * (i + 1);
                    GO.transform.position = Position;
                    DungeonScript dungeonScript = GO.GetComponent<DungeonScript>();
                    SpawnedDungeonRooms.Add(dungeonScript);
                    dungeonScript.RoomIndex = i+1;
                    dungeonScript.EnterDoor.TargetRoomIndex = i;
                    dungeonScript.ExitDoor.TargetRoomIndex = i + 2;
                    dungeonScript.ExitDoor.isExit = true;
                }
                return;
            case RoomSize.medium:
                for (int i = 0; i < RoomCount; i++)
                {
                    GameObject GO = Instantiate(SmallDungeons[Random.Range(0, MediumDungeons.Count)]);
                    Vector3 Position = GO.transform.position;
                    Position.x += 100 * (i + 1);
                    GO.transform.position = Position;
                    DungeonScript dungeonScript = GO.GetComponent<DungeonScript>();
                    SpawnedDungeonRooms.Add(dungeonScript);
                    dungeonScript.RoomIndex = i + 1;
                    dungeonScript.EnterDoor.TargetRoomIndex = i;
                    dungeonScript.ExitDoor.TargetRoomIndex = i + 2;
                    dungeonScript.ExitDoor.isExit = true;
                }
                return;
            case RoomSize.large:
                for (int i = 0; i < RoomCount; i++)
                {
                    GameObject GO = Instantiate(SmallDungeons[Random.Range(0, BigDungeons.Count)]);
                    Vector3 Position = GO.transform.position;
                    Position.x += 100 * (i + 1);
                    GO.transform.position = Position;
                    DungeonScript dungeonScript = GO.GetComponent<DungeonScript>();
                    SpawnedDungeonRooms.Add(dungeonScript);
                    dungeonScript.RoomIndex = i + 1;
                    dungeonScript.EnterDoor.TargetRoomIndex = i;
                    dungeonScript.ExitDoor.TargetRoomIndex = i + 2;
                    dungeonScript.ExitDoor.isExit = true;
                }
                return;
            case RoomSize.random:
                for (int i = 0; i < RoomCount; i++)
                {
                    GameObject GO = null;
                    int y = Random.Range(0, 3);
                    if (y == 0)
                    {
                        GO = Instantiate(SmallDungeons[Random.Range(0, SmallDungeons.Count)]);
                    }
                    else if (y == 1)
                    {
                        GO = Instantiate(SmallDungeons[Random.Range(0, MediumDungeons.Count)]);
                    }
                    else if (y == 2)
                    {
                        GO = Instantiate(SmallDungeons[Random.Range(0, BigDungeons.Count)]);
                    }
                    Vector3 Position = GO.transform.position;
                    Position.x += 100 * (i + 1);
                    GO.transform.position = Position;
                    DungeonScript dungeonScript = GO.GetComponent<DungeonScript>();
                    SpawnedDungeonRooms.Add(dungeonScript);
                    dungeonScript.RoomIndex = i + 1;
                    dungeonScript.EnterDoor.TargetRoomIndex = i;
                    dungeonScript.ExitDoor.TargetRoomIndex = i + 2;
                    dungeonScript.ExitDoor.isExit = true;
                }
                return;
            default: return;
        }
    }
}
