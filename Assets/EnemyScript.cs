using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public ParticleSystem BloodspurtFX;

    [Header("ForMediumAndBoss")]
    public int Health = 100;
    public enum EnemyType
    {
        Small, Medium, Boss
    }

    public enum EnemyState
    {
        Charge, Flank, Distance
    }
    public EnemyType enemyType;

    //public void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Bullet"))
    //    {
    //        TakeDamage();
    //    }
    //} // Dont need trigger enter as we using raycast only;


     public float chargeSpeed = 5f;
    public float flankSpeed = 3f;

    private EnemyState currentState;
    private Transform player;

    void Start()
    {
        currentState = EnemyState.Charge;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Charge:
                ChargeBehavior();
                break;
            case EnemyState.Flank:
                FlankBehavior();
                break;
        }
    }

    void ChargeBehavior()
    {
        transform.LookAt(player.transform);
        transform.Translate(Vector3.forward * chargeSpeed * Time.deltaTime, Space.Self);

        // If the player is looking at the enemy, switch to Flank behavior
        if (IsPlayerLookingAt())
        {
            currentState = EnemyState.Flank;
        }
    }

    void FlankBehavior()
    {
        // Calculate the direction to circle around the player
        Vector3 circleDirection = (player.position - transform.position).normalized;
        transform.LookAt(circleDirection);
        // Rotate around the player at flankSpeed
        transform.RotateAround(player.position, Vector3.up, flankSpeed * Time.deltaTime);

        // If the enemy is behind the player, charge straight towards him
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        if (dotProduct > 0.5f) // Adjust the threshold as needed
        {
            currentState = EnemyState.Charge;
        }
    }

    bool IsPlayerLookingAt()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        return dotProduct > 0.5f; // Adjust the threshold as needed
    }
    public void TakeDamage(int damage)
    {
        if(enemyType == EnemyType.Small)
        {
            Die();
        }
    }

    public void Die()
    {
        Instantiate(BloodspurtFX,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
