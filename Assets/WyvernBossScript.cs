using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernBossScript:BossScript
{
    [Header("ForWyvern")]
    public int turnsBeforeHealingGlide;
    public AnimationClip HealingGlideAnimClip, OriginalIdleAnimClip;
    public GameObject FafnirGO;
    public Animator FafnirAnimator;
    public float moveDistance = 3f;
    public float moveSpeed = 5f;
    public int numberOfMoves = 3;
    private Vector3 originalPosition;
    public EnemyScript RelatedES;
    public AudioClip RoarSFX;
    public AnimationClip Attack1AnimClip;
    public GameObject AttackBeamGO;
    public int idleTimesLeft = 3;
    public GameObject FirePrefabGO, FireGlobulePrefab;
    public float shootForce = 10f;
    public float spawnInterval = 1f;
    public Transform GlobuleSpawnPoint;
    public float maxSpreadAngle = 15f; // Maximum angle of spread
    private void Start()
    {
        // Store the original position of the object
        originalPosition = transform.position;

        // Call the function to start the movement
       // PerformHealingGlide();
       MusicController.Instance.SFXAudioSource.PlayOneShot(RoarSFX);
    }
    public void PerformHealingGlide()
    {
       StartCoroutine(MoveRoutine());
       
    }

   
    IEnumerator MoveRoutine()
    {
        FafnirAnimator.Play(HealingGlideAnimClip.name);
        for (int i = 0; i < numberOfMoves; i++)
        {
            // Move forward
            float elapsedTime = 0f;
            Vector3 startPos = transform.position;
            Vector3 targetPos = transform.position + transform.forward * moveDistance;

            while (elapsedTime < 1f)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }

            // Teleport back to original position
           
            Vector3 OffsetOGPosition = originalPosition;
            OffsetOGPosition.z -= 75;
            transform.position = OffsetOGPosition;
            // Wait for a moment before the next move
            yield return new WaitForSeconds(0.1f);
            RelatedES.TakeDamage(-15);
        }
        
        transform.position = originalPosition;
        // Movement completed
        Debug.Log("Movement completed.");
        FafnirAnimator.CrossFade(OriginalIdleAnimClip.name, 0.1f);
    }

    public void CastBeamAttack()
    {
        StartCoroutine(BeamAttackWyvern());
    }

    IEnumerator BeamAttackWyvern()
    {
        isAttacking = true;
        FafnirAnimator.CrossFade(Attack1AnimClip.name, 0.1f);
        yield return new WaitForSeconds(0.15f);
        AttackBeamGO.SetActive(true);
        float t = 0;
        int i = 0;
        MusicController.Instance.SFXAudioSource.PlayOneShot(RoarSFX);
        while (t < 4)
        {
            i++;
            if(i > 20)
            {
                GameObject newPrefab = Instantiate(FireGlobulePrefab, GlobuleSpawnPoint.position, Quaternion.identity);
                Rigidbody rb = newPrefab.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    // Calculate the base shoot direction
                    Vector3 baseShootDirection = (transform.forward + transform.up * 0.1f).normalized;

                    // Apply random spread to the base shoot direction
                    Vector3 randomSpread = UnityEngine.Random.insideUnitSphere * maxSpreadAngle;
                    Vector3 finalShootDirection = Quaternion.Euler(randomSpread) * baseShootDirection;

                    // Apply the shoot force to the prefab
                    rb.AddForce(finalShootDirection * shootForce, ForceMode.Impulse);

                    // Add gravity to the spawned prefab
                    rb.useGravity = true;
                }
                i = 0;
            }
            CameraShakeController.Instance.AddCameraShake(0.1f);
            t += Time.deltaTime;
            yield return null;

         
        }
        AttackBeamGO.SetActive(false);
        isAttacking = false;
    }


    // Update is called once per frame
    public override void Update()
    {
        if (!canStartAttacking) return;
    }

    public override void ChooseRandomAttack()
    {
        if (isAttacking) return;
        Debug.Log("Testing");

        idleTimesLeft--;
        if(idleTimesLeft <= 0)
        {
            idleTimesLeft = UnityEngine.Random.Range(3, 6);

            int i = UnityEngine.Random.Range(0, 1);
            if(i == 0)
            {
                CastBeamAttack();
            }
            else
            {
                PerformHealingGlide();
            }

        }

       // base.ChooseRandomAttack();
    }
}
