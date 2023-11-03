using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public ParticleSystem BloodspurtFX;

    [Header("ForMediumAndBoss")]
    public int Health = 100;
    public Rigidbody RB;
    public GameObject DeathParticles;
    public Material EnemyMat;
    private Color OGMatColor;
    public enum EnemyType
    {
        Small, Medium, Boss
    }


    public enum EnemyBehaviour
    {
        Melee, Ranged
    }
    public EnemyBehaviour enemyBehaviour;
    public EnemyType enemyType;

    [Header("For Ranged Only")]

    public Transform SpawnFrom;
    public float range;
    public GameObject Rangedball;
    public float AddToCooldown;
    public float cooldown;
    public PlayerRatingController ratingController;

    //public void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Bullet"))
    //    {
    //        TakeDamage();
    //    }
    //} // Dont need trigger enter as we using raycast only;


     public float chargeSpeed = 5f;
    public float flankSpeed = 3f;
    private Transform player;

    void Start()
    {
        OGMatColor = EnemyMat.color;
        player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ChargeBehavior();
    }

    void ChargeBehavior()
    {
        if (enemyBehaviour == EnemyBehaviour.Melee)
        {
            transform.LookAt(player.transform);
            RB.AddForce(transform.forward * chargeSpeed);
        }
        else if(enemyBehaviour == EnemyBehaviour.Ranged)
        {
            if (Vector3.Distance(transform.position, player.position) > range)
            {
                transform.LookAt(player.transform);
                RB.AddForce(transform.forward * chargeSpeed);
            }
            else
            {
                transform.LookAt(player.transform);
            }
            if(cooldown <= 0)
            {
                cooldown = AddToCooldown;
                ShootProjectile();
            }
            if (cooldown > 0) cooldown -= Time.deltaTime;
        }
    }
    public void ShootProjectile()
    {
        Instantiate(Rangedball, transform.position, transform.rotation);
    }
    public void TakeDamage(int damage)
    {
        if(enemyType == EnemyType.Small)
        {
            Die();
        }
        else if(enemyType == EnemyType.Medium)
        {
            Health -= damage;
            if (EnemyMat)
            {
                StartCoroutine(FlashDamage());
            }
            if (BloodspurtFX) Instantiate(BloodspurtFX, transform.position, Quaternion.identity);
            if (Health <= 0) Die();
        }
    }

    public IEnumerator FlashDamage()
    {
        if (EnemyMat)
        {
            EnemyMat.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            EnemyMat.color = OGMatColor;
        }
    }

    private void OnDestroy()
    {
        if(EnemyMat) EnemyMat.color = OGMatColor;
    }

    public void Die()
    {
        ratingController.OnKillEnemy();

        Instantiate(BloodspurtFX,transform.position,Quaternion.identity);
        Instantiate(DeathParticles, transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
