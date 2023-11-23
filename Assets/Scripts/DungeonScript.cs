using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScript : MonoBehaviour
{
    [Header("DungeonDetails")]
    public DungeonDoor EnterDoor, ExitDoor;
    public int RoomIndex;

    public List<EnemyScript> EnemiesInDungeon = new List<EnemyScript>();
    
}
