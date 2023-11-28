using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAbility : Ability
{
    public GameObject Shockwave;
    public float tossbackforce = 25f;

    public override void UseAbility()
    {
        Transform PlayerTransform = FirstPersonController.Instance.transform;
        GameObject GO = Instantiate(Shockwave, PlayerTransform.position + PlayerTransform.forward, PlayerTransform.rotation);



        base.UseAbility();
    }
}
