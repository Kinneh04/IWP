using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public float addToCooldown;
    private float currentCooldown;
    public FirstPersonController firstPersonController;
    public virtual void Update()
    {
        if(currentCooldown >= -0.5f)
        {
            currentCooldown -= Time.deltaTime;
            firstPersonController.UpdateAbilityCooldownText(currentCooldown);
        }
    }

    // Use base
    public virtual void UseAbility()
    {

        currentCooldown = addToCooldown;
    }

    private void Start()
    {
        firstPersonController = GameObject.FindObjectOfType<FirstPersonController>();
       // currentCooldown += addToCooldown;
    }
}
