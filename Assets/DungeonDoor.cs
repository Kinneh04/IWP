using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public bool hasHit = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox") || other.gameObject.CompareTag("Player"))
        {
            if (MunninsTrialManager.Instance.hasClearedThisDungeon && !hasHit)
            {
                hasHit = true;
                MunninsTrialManager.Instance.TransitionToNewDungeon();
            }
        }
    }
}
