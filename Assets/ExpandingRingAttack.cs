using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingRingAttack : MonoBehaviour
{
    public float expandFactor = 1.5f; // Factor by which the object expands each time
    public int expandCap = 7; // Maximum number of expansions allowed
    public bool canDamage = false; // Flag to indicate if the object can cause damage
    public Intervals recordedInterval;
    private int expandCount = 0; // Counter to keep track of the number of expansions
    private Vector3 targetScale; // Target scale for lerping

    public void ExpandObject()
    {
        if (expandCount < expandCap)
        {
            Vector3 ogscale = transform.localScale;
            ogscale.x *= expandFactor;
            ogscale.z *= expandFactor;
            targetScale = ogscale; // Calculate the target scale

            // Smoothly lerp to the target scale
            StartCoroutine(ScaleOverTime(transform.localScale, targetScale, 0.35f));

            expandCount++;
            canDamage = true; // Set the canDamage flag to true after expansion

            if (expandCount >= expandCap)
            {
                recordedInterval.ToBeDeleted = true;
                Destroy(gameObject);
            }
        }
    }

    private System.Collections.IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, (Time.time - startTime) / duration);
            yield return null;
        }
        transform.localScale = endScale; // Ensure the final scale is set accurately
    }
}
