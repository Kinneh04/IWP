using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRing : MonoBehaviour
{
    public ExpandingRingAttack ERA;

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player") || (other.transform.CompareTag("PlayerHitbox")))
        {
         //   Debug.Log("OW!!");
            if (ERA.canDamage)
            {
                FirstPersonController.Instance.TakeDamage(25);
                ERA.canDamage = false;
            }
        }
    }
}
