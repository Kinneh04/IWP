using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Ability : Ability
{
    public GameObject GrenadeObject;
    public float tossForce = 10f; // The force applied to the coin  
    public MusicController musicController;
    public override void UseAbility()
    {
        GameObject InstantiatedCoin = Instantiate(GrenadeObject, firstPersonController.SpawnAbilitySpawnablesFrom.position, Quaternion.identity);

        Rigidbody rb = InstantiatedCoin.GetComponent<Rigidbody>();

        Vector3 upwardForce = transform.up * tossForce;

        // Add force to the rigidbody
        rb.AddForce(upwardForce, ForceMode.Impulse);
        rb.AddForce(firstPersonController.transform.forward * tossForce, ForceMode.Impulse);
        base.UseAbility();
    }
}
