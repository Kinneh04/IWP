using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int Damage;
    public float speed;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FirstPersonController FPC = other.GetComponent<FirstPersonController>();
            FPC.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
