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
    private void Start()
    {
        // Store the original position of the object
        originalPosition = transform.position;

        // Call the function to start the movement
        PerformHealingGlide();
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


    // Update is called once per frame
    public override void Update()
    {
        if (!canStartAttacking) return;
    }

    public override void ChooseRandomAttack()
    {
        Debug.Log("Testing");
       // base.ChooseRandomAttack();
    }

    public void CastBeamAttack()
    {

    }
}
