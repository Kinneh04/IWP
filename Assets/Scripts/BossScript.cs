using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public bool canStartAttacking = false;

    public Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
    }

    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player);
        }
    }
}
