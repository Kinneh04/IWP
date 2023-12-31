using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public ParticleSystem BloodspurtFX;
    public bool isStatic = false;

    [Header("ForMelee")]
    public int TouchDamage;
    [Header("ForMediumAndBoss")]
    public int Health = 100;
    public Rigidbody RB;
    public GameObject DeathParticles;
    public Material EnemyMat;
    private Color OGMatColor;
    public bool enemyIsDead = false;
    float onTouchCooldown = 0;
    public Animator HitAnimator;
    public AnimationClip HitAnimClip;
    public float BounceForce;

    public enum EnemyType
    {
        Small, Medium, Boss
    }
    public Vector3 TargetPosition;

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
    public float raycastCooldown = 0.2f;
    public FirstPersonController FPC;
    public BossManager AttachedBossManager;
    public Animator EnemyAnimator;
    public AnimationClip AttackAnimationClip;
    public int NumberOfProjectiles;
    public float SpreadAngle;
    public float cooldownBeforeShooting = 2.0f;
    [Header("ForBossOnly")]
    public Animator BossAnimator;
    public AnimationClip[] bossHitAnimClips;
    float audioHitCooldown = 0.0f;

    public GameObject DamageText;

   
    //public void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Bullet"))
    //    {
    //        TakeDamage();
    //    }
    //} // Dont need trigger enter as we using raycast only;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            if (enemyType == EnemyType.Small)
            {
                other.GetComponentInParent<FirstPersonController>().TakeDamage(TouchDamage);
                Die(true);
            }
            else if (enemyType == EnemyType.Medium && onTouchCooldown <= 0)
            {
                FirstPersonController playerController = other.GetComponentInParent<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(TouchDamage);

                    onTouchCooldown = 2.0f;

                    Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
                    if (enemyRigidbody != null)
                    {
                        // Apply a force to push the enemy backward
                        Vector3 direction = transform.position - playerController.transform.position;
                        direction.Normalize();
                        enemyRigidbody.AddForce(direction * BounceForce, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    [Header("Effects")]
    public bool onFire = false;
    public float fireTime = 0.0f;
    public GameObject OnFireEffects;
    public int FireDamage = 2;


    public float chargeSpeed = 5f;
    public float flankSpeed = 3f;
    private Transform player;

    [Header("SoundEffects")]
    AudioSource AS;
    public AudioClip IntroAudio, HitAudio;
    void Start()
    {
       if(EnemyMat)  OGMatColor = EnemyMat.color;
        player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
        RB = GetComponent<Rigidbody>();
        FPC = FirstPersonController.Instance;
        AS = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();

        if (AS && IntroAudio) AS.PlayOneShot(IntroAudio);
        if (!isOnValidSurface(transform.position) && enemyType != EnemyType.Boss)
        {
            Destroy(gameObject);
        }

    }

    void Update()
    {
        if(enemyType != EnemyType.Boss && !isStatic)
            ChargeBehavior();


        if (onTouchCooldown > 0) onTouchCooldown -= Time.deltaTime;

        if (audioHitCooldown > 0) audioHitCooldown -= Time.deltaTime;

        if (enemyType != EnemyType.Boss && OnFireEffects)
        {
            if (fireTime > 0)
            {
                onFire = true;
                OnFireEffects.SetActive(true);
                fireTime -= Time.deltaTime;

            }
            else
            {
                onFire = false;
                OnFireEffects.SetActive(false);
            }
        }
    }

    public void SetOnFire(float time)
    {
        fireTime += time;
    }


    void ChargeBehavior()
    {
        if (enemyBehaviour == EnemyBehaviour.Melee)
        {
            transform.LookAt(player.transform);
            transform.Translate(chargeSpeed * Time.deltaTime * transform.forward);
            //if (raycastCooldown > 0)
            //{
            //    raycastCooldown -= Time.deltaTime;
            //}
            //else
            //{
            //    raycastCooldown = 0.2f;
            //    RaycastHit hit;
            //    if (Physics.Raycast(transform.position, player.position - transform.position, out hit))
            //    {
            //        if (hit.collider.gameObject == player.gameObject)
            //        {
            //            TargetPosition = player.position;
            //            transform.LookAt(TargetPosition);

            //        }
            //    }
            //    else
            //    {
            //        transform.LookAt(TargetPosition);
            //    }
            //}
            //RB.AddForce(transform.forward * chargeSpeed);
        }
        else if(enemyBehaviour == EnemyBehaviour.Ranged)
        {
            if (Vector3.Distance(transform.position, player.position) > range)
            {
                transform.LookAt(player.transform);
                transform.Translate(chargeSpeed * Time.deltaTime * transform.forward);
            }
            else
            {
                transform.LookAt(player.transform);
           
            }
            if (cooldownBeforeShooting > 0) cooldownBeforeShooting -= Time.deltaTime;
            else
            {

                if (cooldown <= 0)
                {
                    cooldown = AddToCooldown;
                    ShootProjectile();
                }
                if (cooldown > 0) cooldown -= Time.deltaTime;
            }
        }
    }
    public void ShootProjectile()
    {
        if (NumberOfProjectiles < 1) return; // Ensure at least one projectile is fired

        if (AttackAnimationClip && EnemyAnimator)
        {
            EnemyAnimator.Play(AttackAnimationClip.name);
        }

        float angleIncrement = SpreadAngle / NumberOfProjectiles;
        float startingAngle = -SpreadAngle / 2f;

        for (int i = 0; i < NumberOfProjectiles; i++)
        {
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, startingAngle + i * angleIncrement);
            Quaternion finalRotation = transform.rotation * spreadRotation;
            Instantiate(Rangedball, transform.position, finalRotation);
        }
    }
    public void TakeDamage(int damage, bool duplicate = false,bool fireDamage = false)
    {

        if(DamageText)
        {
            GameObject GO = Instantiate(DamageText, transform.position, Quaternion.identity);
            GO.GetComponentInChildren<TMP_Text>().text = damage.ToString();
            if(fireDamage)
            {
                GO.GetComponentInChildren<TMP_Text>().color = Color.red;
            }
        }

        if(enemyType == EnemyType.Small)
        {
            Die(duplicate);
        }
        else if(enemyType == EnemyType.Medium)
        {
            Health -= damage;
            if (EnemyMat)
            {
                StartCoroutine(FlashDamage());
            }
            if (BloodspurtFX) Instantiate(BloodspurtFX, transform.position, Quaternion.identity);
            if (Health <= 0) Die(duplicate);
            HitAnimator.Play(HitAnimClip.name);
        }
        else if (enemyType == EnemyType.Boss)
        {
            Health -= damage;
            if(bossHitAnimClips.Length > 0 && BossAnimator) BossAnimator.Play(bossHitAnimClips[Random.Range(0, bossHitAnimClips.Length)].name);
            AttachedBossManager.UpdateHealthSlider();
            if (Health <= 0)
            {
                if(AttachedBossManager.InstantiatedBoss.finisher)
                {
                    AttachedBossManager.InstantiatedBoss.FinishHim();
                }
                else
                    Die(duplicate);
            }
            if (AS && HitAudio && audioHitCooldown <= 0)
            {
                AS.PlayOneShot(HitAudio);
                audioHitCooldown = 0.35f;
            }
       //     HitAnimator.Play(HitAnimClip.name);
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
    public bool isOnValidSurface(Vector3 targetPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, Vector3.down, out hit, 100))
        {
            // Check if the hit surface is not tagged "ClearFloor"
            if (hit.collider.CompareTag("ClearFloor"))
            {
             //   Debug.Log("Hit the invalid surface!");
                return false;
            }
            else
            {
               // Debug.Log("Hit:  " + hit.collider.name);
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        if(EnemyMat) EnemyMat.color = OGMatColor;
    }

    public void Die(bool duplicate = false)
    {
        if (enemyType == EnemyType.Boss)
        {
            AttachedBossManager.StartFinisher();
        }
        else
        {
            if (!enemyIsDead)
            {

                PlayerRatingController.Instance.OnKillEnemy();
                enemyIsDead = true;
            }

            Instantiate(BloodspurtFX, transform.position, Quaternion.identity);
            Instantiate(DeathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
