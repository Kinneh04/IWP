using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public Vector3 spinAxis = Vector3.up; // The axis around which the object will spin
    public float spinSpeed = 5.0f; // The speed at which the object will spin (in degrees per second)

    void Update()
    {
        // Rotate the object around the specified axis
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.World);
    }
}
