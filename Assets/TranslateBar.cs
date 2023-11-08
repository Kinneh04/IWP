using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateBar : MonoBehaviour
{
    public Transform Target;
    float progress;
    Vector3 startPos;
    public float speed;
    [HideInInspector] public VisualMetronome relatedMetro;
    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(startPos, Target.position, progress);

        progress += Time.deltaTime * speed;
        if(progress >= 1)
        {
            relatedMetro.PulseGlow();
            Destroy(gameObject);
        }
    }
}
