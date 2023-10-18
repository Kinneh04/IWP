using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairFadeBPM : MonoBehaviour
{
    public float Speed;

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Speed * Time.deltaTime);
        if (transform.localScale == Vector3.zero) Destroy(gameObject);
    }
}
