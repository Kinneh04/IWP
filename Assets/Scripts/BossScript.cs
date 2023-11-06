using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public bool canStartAttacking = false;

    public Transform player;

    public float dashSpeed = 5.0f;
    public float minDistance = 3.0f;
    public float maxDistance = 10.0f;

    private Vector3 targetPosition;
    private bool isDashing = false;

    void Start()
    {
        // Call Dash method when you want the boss to dash
        // Example: DashToRandomLocation();
    }

    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player);
        }
        if (isDashing)
        {
            float step = dashSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);

            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isDashing = false;
            }
        }
    }

    public void DashToLocation(Vector3 destination)
    {
        targetPosition = destination;
        isDashing = true;
    }

    public void DashToRandomLocation()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep it on the same plane as the boss
        randomDirection.Normalize();

        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 destination = transform.position + randomDirection * randomDistance;

        DashToLocation(destination);
    }
}
