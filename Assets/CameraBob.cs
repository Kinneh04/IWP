using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public float bobbingAmount = 0.1f; // Adjust this value to control the intensity of the bobbing effect
    public float bobbingSpeed = 0.18f; // Adjust this value to control the speed of the bobbing effect
    public float tiltAmount;
    public float originalY;
    public Quaternion Finaltilt;

    void Start()
    {
        originalY = transform.localPosition.y;
    }

    void Update()
    {
        float newY = originalY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;

        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

         float tiltAngle = Input.GetAxis("Horizontal") * tiltAmount; // Assuming left/right movement is controlled by Input.GetAxis("Horizontal")
        Quaternion targetRotation = Quaternion.Euler(0, 0, -tiltAngle) * transform.localRotation;
        Finaltilt = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 3f); // Adjust the last parameter to control the speed of the tilt
    }

}
