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
    private Transform player;

    void Start()
    {
        OGMatColor = EnemyMat.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ChargeBehavior();
    }

    void ChargeBehavior()
    {
        transform.LookAt(player.transform);
        RB.AddForce(transform.forward * chargeSpeed);
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
        Instantiate(BloodspurtFX,transform.position,Quaternion.identity);
        Instantiate(DeathParticles, transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
