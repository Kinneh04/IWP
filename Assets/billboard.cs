using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour
{
    private void Update()
    {
        // Ensure the object faces the camera
        transform.LookAt(Camera.main.transform);
        // Optionally, you can also rotate the object 180 degrees around the up axis to face the camera correctly
        transform.Rotate(0, 180, -90);
    }
}
