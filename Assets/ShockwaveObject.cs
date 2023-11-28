using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveObject : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Rigidbody otherObjectRB = other.GetComponent<Rigidbody>(); // Replace OtherGameObject with the target GameObject
            if (otherObjectRB != null)
            {
                Vector3 direction = otherObjectRB.transform.position - transform.position;
                direction.Normalize();
                otherObjectRB.AddForce(direction * 25.0f, ForceMode.Impulse);
            }
            other.GetComponent<EnemyScript>().TakeDamage(25);
        }
    }
}
