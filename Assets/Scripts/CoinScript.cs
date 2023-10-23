using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyScript;

public class CoinScript : MonoBehaviour
{
    public GameObject linePrefab; // Reference to the LineRenderer prefab
    public float maxRicochetDistance = 50f;

    private LineRenderer lineRenderer;
    public GameObject SparksParticles;

    void Start()
    {
    
    }

    public void Ricochet()
    {
        // Find the nearest enemy
        GameObject nearestEnemy = FindNearestEnemy();
        Debug.Log("PEW!");
        if (nearestEnemy != null)
        {
            lineRenderer = Instantiate(linePrefab, transform.position, Quaternion.identity, transform).GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false; // Initially, the line is not visible
            lineRenderer.transform.SetParent(null, false);

            // Draw a line from the coin to the nearest enemy
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, nearestEnemy.transform.position);
            lineRenderer.enabled = true; // Show the line
            nearestEnemy.GetComponent<EnemyScript>().TakeDamage(100);
            Destroy(lineRenderer, 3.0f);
        }
        Instantiate(SparksParticles,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestMediumEnemy = null;
        float nearestMediumDistance = float.MaxValue;
        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

            if (enemyScript != null)
            {
                if (enemyScript.enemyType == EnemyType.Medium)
                {
                    if (distance < nearestMediumDistance && distance <= maxRicochetDistance)
                    {
                        nearestMediumDistance = distance;
                        nearestMediumEnemy = enemy;
                    }
                }

                if (distance < nearestDistance && distance <= maxRicochetDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestMediumEnemy != null)
        {
            return nearestMediumEnemy;
        }
        else
        {
            return nearestEnemy;
        }
    }

}
