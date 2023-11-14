using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialDungeon : MonoBehaviour
{
    public Transform Startposition;
    public GameObject Arrow;
    public List<EnemyScript> Enemies = new List<EnemyScript>();
    public DungeonDoor ExitDoor;
}
