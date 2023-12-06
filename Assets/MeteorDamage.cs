using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorDamage : MonoBehaviour
{
    public MeteorAttack MA;
    private void OnTriggerStay(Collider other)
    {
        if(MA.canDamage)
        {
            if(other.CompareTag("Player") || other.CompareTag("PlayerHitbox"))
            {
                FirstPersonController.Instance.TakeDamage(15);
                MA.canDamage = false;
            }
        }
    }
}
