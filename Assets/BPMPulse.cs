using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMPulse : MonoBehaviour
{
    public Vector3 OriginalScaleTransform;
    public float DecreaseTime;
    private void Start()
    {
        OriginalScaleTransform = transform.localScale;
    }
    public void Pulse()
    {
        transform.localScale = OriginalScaleTransform * 1.2f;

    }

    private void FixedUpdate()
    {
        if (transform.localScale != OriginalScaleTransform)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, OriginalScaleTransform, DecreaseTime * Time.deltaTime);
        }
    }
}
