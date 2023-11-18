using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public int AvailableDashes = 3;
    public bool isAttacking = false;
    public int ChanceForDash;
    public bool ThreeDash = false;

    [Header("Attack1_Projectiles")]
    public GameObject Projectile;
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

    [Header("Lighting")]
    public Light Bosslight;

    [Header("Audio")]
     AudioSource AS;
    public AudioClip BeamAudioClip, WarningBeamAudioClip;

    
    
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
                    ShootProjectile();
                }
                else
                {
                    int i = Random.Range(0, 100);
                    if(i < 50)
                    {
                        TryShootAttack1();
                    }
                    else
                    {
                        isAttackingBeam = true;
                        isWarning = true;
                        CurrentWarningFlashes = WarningFlashes;
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
        for(int i = 0; i < chosenAmountToSpawn; i++)
        {
            ShootProjectile();
            yield return new WaitForSeconds(SpawnInterval);
        }
    }
    public void ShootProjectile()
    {
        
        Instantiate(Projectile, transform.position, transform.rotation);
        chosenAmountToSpawn--;
        if(chosenAmountToSpawn <= 0)
        {
            chosenAmountToSpawn = 0;
            isSpawningOnBeat = false;
            isSpawning = false;
        }
    }
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHitbox").transform;
        AS = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
    }

    public void WarnPlayerOnBeam()
    {
        AS.PlayOneShot(WarningBeamAudioClip);
        Bosslight.intensity = WarningEmissionIntensity;
        CurrentWarningFlashes--;
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
            float step = dashSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);

            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isDashing = false;
            }
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
