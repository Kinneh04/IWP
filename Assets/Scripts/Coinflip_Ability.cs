using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coinflip_Ability : Ability
{
    public GameObject Coinobject;
    public float tossForce = 10f; // The force applied to the coin
    public float airTime = 2.0f; // The time (in seconds) the coin stays in the air before gravity kicks in
    public float coinSlowdownTime;
    public override void UseAbility()
    {
        GameObject InstantiatedCoin = Instantiate(Coinobject, firstPersonController.SpawnAbilitySpawnablesFrom.position, Quaternion.identity);

        Rigidbody rb = InstantiatedCoin.GetComponent<Rigidbody>();

        Vector3 upwardForce = transform.up * tossForce;

        // Add force to the rigidbody
        rb.AddForce(upwardForce, ForceMode.Impulse);
        rb.AddForce(firstPersonController.transform.forward * tossForce, ForceMode.Impulse);
        StartCoroutine(DelayGravity(rb));
        base.UseAbility();
    }
    private IEnumerator DelayGravity(Rigidbody rb)
    {
        // Disable gravity initially
        if(rb)
        rb.useGravity = false;
        yield return new WaitForSeconds(0.5f);
        while(rb && rb.velocity.magnitude > 0.5f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, coinSlowdownTime * Time.deltaTime);

            yield return null;
        }


        // Wait for the specified airTime
        yield return new WaitForSeconds(airTime);
        if(rb)
        // Enable gravity after the airTime has passed
        rb.useGravity = true;
    }
}
