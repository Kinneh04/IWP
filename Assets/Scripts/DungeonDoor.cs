using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public bool hasHit = false;
    public int TargetRoomIndex;
    public bool isExit = false;
    public Transform TeleportTransform;

    DungeonManager DM;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox") || other.gameObject.CompareTag("Player"))
        {
            if (MunninsTrialManager.Instance.hasClearedThisDungeon && !hasHit)
            {
                //hasHit = true;
                //MunninsTrialManager.Instance.TransitionToNewDungeon();
                DM.TeleportPlayerToRoomViaIndex(TargetRoomIndex, isExit);
            }
        }
    }

    private void Awake()
    {
        hasHit = false;
        DM = GameObject.FindObjectOfType<DungeonManager>();
    }

    //private void Update()
    //{
    //    if(!MunninsTrialManager.Instance.hasClearedThisDungeon)
    //    {
    //        hasHit = false;
    //    }
    //}
}
