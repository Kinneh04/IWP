using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
   public bool hasHit = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !hasHit)
        {
            EnemyScript ES = other.GetComponent<EnemyScript>();
            if (ES.enemyType != EnemyScript.EnemyType.Boss)
            {
                ES.TakeDamage(50);
               
            }
            else
            {
                ES.TakeDamage(25);
            }
            hasHit = true;
        }
    }
}
