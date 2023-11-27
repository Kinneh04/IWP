using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BossScript : MonoBehaviour
{
    public bool canStartAttacking = false;

    public Transform player;

    public float dashSpeed = 5.0f;
    public float minDistance = 3.0f;
    public float maxDistance = 10.0f;

    private Vector3 targetPosition;
    private bool isDashing = false;

    public int AvailableDashes = 3;
    public bool isAttacking = false;
    public int ChanceForDash;
    public bool ThreeDash = false;

    [Header("Attack1_Projectiles")]
    public List<GameObject> Projectiles = new List<GameObject>();
    private GameObject CurrentlyChosenProjectile;
    public bool SpawnOnBeat;
    public float SpawnInterval;
    public int minSpawnAmount, maxSpawnAmount;
    public bool isSpawning;
    public int chosenAmountToSpawn;
    bool isSpawningOnBeat;

    [Header("Attack2_Beam")]
    public bool isAttackingBeam;
    public GameObject Beam;
    public bool isBeaming;
    public float beamTime;
    public int WarningFlashes = 2;
    int CurrentWarningFlashes = 0;
    public float OriginalEmission = 0.0f;
    public float WarningEmissionIntensity = 1.0f;
    public float lerpSpeed = 3.5f;
    public bool isWarning = false;
    public bool hasComplimented = false;
    public GameObject HolyParticles;
    public Transform ParticleSpawner;

    [Header("Attack3_Ring")]
    public GameObject RingObject;
    public Vector3 Offset;

    [Header("Lighting")]
    public Light Bosslight;

    [Header("Audio")]
     AudioSource AS;
    public AudioClip BeamAudioClip, WarningBeamAudioClip, ProjectileSpawnAudioClip, IntroAudioClip;

    [Header("Trail")]
    public GameObject TrailGameObject;
    public float TrailCooldown;
    float currentTrailCooldown;

    [Header("Finisher")]
    public List<AudioClip> FinishHimAudioClips = new List<AudioClip>();
    int finishHimIndex = 0;
    public bool finisher = false;
    public Animator BossAnimator;
    public AnimationClip HurtAnimation;
    public AnimationClip FinisherAnimation;
    public GameObject FinishedParticleEffects;
    public GameObject AboutToDieParticles;

    [Header("RecordedProjectiles")]
    public List<GameObject> SpawnedGameobjects = new List<GameObject>();


    public void CastRingAttack()
    {
        RaycastHit hit;

        // Raycast downwards from the current position of this GameObject
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Check if the ray hits an object tagged as "floor"
            if (hit.collider.CompareTag("Floor"))
            {
                Debug.Log("SpawnedRing!");
                // Instantiate RingObject at the hit point with default rotation
                GameObject GO = Instantiate(RingObject, hit.point + Offset, Quaternion.identity);
                ExpandingRingAttack ERA = GO.GetComponent<ExpandingRingAttack>();
                Intervals I = new Intervals();
                I._steps = 2;
                I._trigger.AddListener(delegate { ERA.ExpandObject(); });
                MusicController.Instance._intervals.Add(I);
                ERA.recordedInterval = I;
            }
        }
    }
    public void ChooseRandomAttack()
    {
        if (!canStartAttacking) return;
        if (ThreeDash)
        {
            DashToRandomLocation();
            if(AvailableDashes <= 0)
            {
                ThreeDash = false;
            }
        }
        else
        {

            int o = Random.Range(0, 100);
            if (o < ChanceForDash && AvailableDashes > 0 && !isSpawning && !isAttackingBeam)
            {
                if (o < ChanceForDash / 2)
                {
                    ThreeDash = true;
                }
                else
                {
                    DashToRandomLocation();
                }
            }
            else
            {
                if(isAttackingBeam)
                {
                    if (isWarning || CurrentWarningFlashes > 0) WarnPlayerOnBeam();
                    else
                    {
                        StartCoroutine(BeamAttack());
                    }
                }
                else if(isSpawningOnBeat && isSpawning)
                {
                    CurrentlyChosenProjectile = Projectiles[Random.Range(0, Projectiles.Count)];
                    ShootProjectile(transform.position);
                }
                else
                {
                    int i = Random.Range(0, 100);
                    if (i < 33)
                    {
                        CurrentlyChosenProjectile = Projectiles[Random.Range(0, Projectiles.Count)];
                        TryShootAttack1();
                    }
                    else
                    {
                        int p = Random.Range(1, 3);
                        if (p == 2)
                        {
                            Debug.Log("Attack 3!!!");
                            CastRingAttack();
                        }
                        else
                        {
                            isAttackingBeam = true;
                            isWarning = true;
                            CurrentWarningFlashes = WarningFlashes;
                        }
                    }
                }
                AvailableDashes = 3;
                return;
            }
        }

    }
    public IEnumerator BeamAttack()
    {
        isBeaming = true;
        AS.PlayOneShot(BeamAudioClip);
        Beam.SetActive(true);
        yield return new WaitForSeconds(beamTime);
        Beam.SetActive(false);
        isWarning = false;
        isBeaming = false;
        hasComplimented = false;
        isAttackingBeam = false;
    }

    public void TryShootAttack1()
    {
        chosenAmountToSpawn = Random.Range(minSpawnAmount, maxSpawnAmount);

        if(SpawnOnBeat)
        {
            isSpawningOnBeat = true;
        }
        else
        {
            StartCoroutine(ShootProjectilesWithDelay());
        }
        isSpawning = true;
    }

    IEnumerator ShootProjectilesWithDelay()
    {
        float angleStep = 360f / chosenAmountToSpawn; // Calculate angle step for equal spread
        float currentAngle = 0f; // Start from 0 degrees

        for (int i = 0; i < chosenAmountToSpawn; i++)
        {
            float spawnX = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * currentAngle); // Calculate x position
            float spawnY = transform.position.y + Mathf.Cos(Mathf.Deg2Rad * currentAngle); // Calculate y position

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, transform.position.z); // Set spawn position

            // Instantiate and spawn projectile at the calculated position
            ShootProjectile(spawnPosition);

            currentAngle += angleStep; // Move to the next angle for the next spawn
            yield return new WaitForSeconds(SpawnInterval);
        }
    }
    public void ShootProjectile(Vector3 spawnPosition)
    {
        AS.PlayOneShot(ProjectileSpawnAudioClip);
       
        chosenAmountToSpawn--;
        SpawnedGameobjects.Add(Instantiate(CurrentlyChosenProjectile, spawnPosition, transform.rotation));
        if (chosenAmountToSpawn <= 0)
        {
            chosenAmountToSpawn = 0;
            isSpawningOnBeat = false;
            isSpawning = false;
        }
    }

    public void ClearAllProjectiles()
    {
        Beam.SetActive(false);
        foreach(GameObject GO in SpawnedGameobjects)
        {
            Destroy(GO);
        }
        AboutToDieParticles.SetActive(true);
    }

    public void FinishHim()
    {

        AS.Stop();
        AS.PlayOneShot(FinishHimAudioClips[finishHimIndex]);
        finishHimIndex++;
        BossAnimator.Play(FinisherAnimation.name);
        if(finishHimIndex >= FinishHimAudioClips.Count)
        {
            Instantiate(FinishedParticleEffects, transform.position, Quaternion.identity);
            BossManager.Instance.KillBoss();
           // GetComponent<EnemyScript>().Die();
        }
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
        AS = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
        AS.PlayOneShot(IntroAudioClip);
    }

    public void WarnPlayerOnBeam()
    {
        AS.PlayOneShot(WarningBeamAudioClip);
        Bosslight.intensity = WarningEmissionIntensity;
        CurrentWarningFlashes--;
        Instantiate(HolyParticles, ParticleSpawner.position, transform.rotation);
        if (CurrentWarningFlashes <= 0) isWarning = false;
    }

    void Update()
    {
        if (player != null && !isBeaming)
        {
            Vector3 targetDirection = player.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
        }
        if (isDashing)
        {

            if (currentTrailCooldown <= 0)
            {
                Instantiate(TrailGameObject, transform.position, transform.rotation);
                currentTrailCooldown = TrailCooldown;
            }
            else currentTrailCooldown -= Time.deltaTime;
            float step = dashSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);

            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isDashing = false;
            }
        }
        else currentTrailCooldown = TrailCooldown;
        if(isBeaming && !hasComplimented && FirstPersonController.Instance.isDashing)
        {
            hasComplimented = true;
            PlayerRatingController.Instance.GoodDodge();
        }
       if(Bosslight.intensity != OriginalEmission)
        {
            Bosslight.intensity = Mathf.Lerp(Bosslight.intensity, OriginalEmission, lerpSpeed * Time.deltaTime);
        }
        
    }

    public void DashToLocation(Vector3 destination)
    {
        targetPosition = destination;
        isDashing = true;
        AvailableDashes--;
    }

    public void DashToRandomLocation()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep it on the same plane as the boss
        randomDirection.Normalize();

        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 destination = transform.position + randomDirection * randomDistance;

        // Check if there's a clear path before dashing
        if (IsClearPath(transform.position, destination))
        {
            DashToLocation(destination);
        }
    }

    bool IsClearPath(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, to - from, out hit, Vector3.Distance(from, to)))
        {

            if (hit.transform.gameObject.CompareTag("Wall"))
            {
                return false;
            }
        }
        else return true;
        return false;
    }
}
