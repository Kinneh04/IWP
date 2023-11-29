using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmpedUpAbility : Ability
{
    public GameObject Shockwave;
    public float lastsForTime = 6.0f;
    public AudioClip FinishAudioClip;

    [Header("OGStats")]
    float OGspeed;
    int OGDamage;
    public override void UseAbility()
    {
        Transform PlayerTransform = FirstPersonController.Instance.transform;
        GameObject GO = Instantiate(Shockwave, PlayerTransform.position + PlayerTransform.forward, PlayerTransform.rotation);
        GO.transform.SetParent(PlayerTransform);
        Destroy(GO, 7.0f);
        base.UseAbility();
    }

    IEnumerator UseAbilityCoroutine()
    {
        OGspeed = firstPersonController.MoveSpeed;
        firstPersonController.MoveSpeed *= 1.5f;
        OGDamage = ShootingScript.Instance.SetDamage;
        ShootingScript.Instance.SetDamage += 10;
        yield return new WaitForSeconds(lastsForTime);
        AS.PlayOneShot(FinishAudioClip);
        firstPersonController.MoveSpeed = OGspeed;
        ShootingScript.Instance.SetDamage = OGDamage;
    }
}
