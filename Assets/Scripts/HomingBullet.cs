using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public Transform target;    // The target (player) to home towards
    public float rotationSpeed = 2f;    // Speed at which the missile rotates towards the target
    public float movementSpeed = 5f;    // Speed at which the missile moves forward
    public float lockOnTime = 3f;       // Time to lock on before moving forward without tracking

    private float lockOnTimer = 0f;
    private bool isLockedOn = true;

    void Update()
    {
        if (target != null)
        {
            if (isLockedOn)
            {
                // Rotate towards the target using lerp for smooth rotation
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
                // Check if we've locked on for enough time
                if (lockOnTimer < lockOnTime)
                {
                    lockOnTimer += Time.deltaTime;
                }
                else
                {
                    // Stop further locking on and start moving forward
                    isLockedOn = false;
                }
            }

            // Move forward after locking on
            if (!isLockedOn)
            {
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
            Debug.LogWarning("No target assigned to the missile.");
        }
    }
}
