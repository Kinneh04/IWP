using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGlobuleCollider : MonoBehaviour
{
    public int fireGlobDamage = 25;
    public GameObject FirePrefab;
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("HIT!");
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerHitbox"))
        {
            FirstPersonController.Instance.TakeDamage(fireGlobDamage);
        }
        else
        {
            GameObject GO =Instantiate(FirePrefab, transform.position, Quaternion.identity);
            Quaternion Q = GO.transform.rotation;
            Q.x = -90;
            GO.transform.rotation = Q;
        }
        Destroy(gameObject);
    }
}
