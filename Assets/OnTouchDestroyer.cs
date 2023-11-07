using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchDestroyer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
