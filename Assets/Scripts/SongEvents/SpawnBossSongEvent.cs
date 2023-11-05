using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBossSongEvent : SongEvent
{
    public BossManager bossController;
    public string bossName;
    public override void CastEvent()
    {
        if (bossController == null)
        {
            bossController = GameObject.FindObjectOfType<BossManager>();
        }
        bossController.SpawnBossByName(bossName);
        base.CastEvent();
    }
}
