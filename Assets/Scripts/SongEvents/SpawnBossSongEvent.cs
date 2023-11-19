using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBossSongEvent : SongEvent
{
    public BossManager bossController;
    public string bossName;
    public float TimeInSecondsLeftToKill;
    public override void CastEvent()
    {
        if (bossController == null)
        {
            bossController = GameObject.FindObjectOfType<BossManager>();
        }
        bossController.SpawnBossByName(bossName, TimeInSecondsLeftToKill);
        MusicController.Instance.isDrop = true;
        base.CastEvent();
    }
}
