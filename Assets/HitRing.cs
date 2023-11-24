using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRing : MonoBehaviour
{
    public ExpandingRingAttack ERA;
    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.CompareTag("Player") || (collision.transform.CompareTag("PlayerHitbox")))
        {
            if(ERA.canDamage)
                FirstPersonController.Instance.TakeDamage(25);
        }

    }
}
