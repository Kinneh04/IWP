using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBossSongEvent : SongEvent
{
    public EnemySpawner enemyController;
    public string bossName;
    public override void CastEvent()
    {
        if (enemyController == null)
        {
            enemyController = GameObject.FindObjectOfType<EnemySpawner>();
        }
        enemyController.SpawnBossByName(bossName);
        base.CastEvent();
    }
}
