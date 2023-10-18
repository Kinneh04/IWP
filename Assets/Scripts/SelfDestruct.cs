using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float SelfDestructAfterSeconds;

    private void Awake()
    {
        Destroy(gameObject, SelfDestructAfterSeconds);
    }
}
