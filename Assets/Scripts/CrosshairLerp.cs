using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairLerp : MonoBehaviour
{
    public enum CrosshairType
    {
        Left, Right
    }
    public CrosshairType crosshairType;
    public float speed;
    public Transform target;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }
    private void Update()
    {
        transform.position = Vector3.LerpUnclamped(transform.position, target.position, speed * Time.deltaTime);
    }
}
