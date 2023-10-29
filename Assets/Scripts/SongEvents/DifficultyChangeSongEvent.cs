using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyChangeSongEvent : SongEvent
{
    public EnemySpawner enemyController;
    public int newDifficulty;
    public override void CastEvent()
    {
        if (enemyController == null)
        {
            enemyController = GameObject.FindObjectOfType<EnemySpawner>();
        }
        enemyController.difficulty = newDifficulty;
        base.CastEvent();
    }
}
