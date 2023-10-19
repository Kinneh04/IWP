using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public MusicController musicController;
    public WeaponMovement weaponMovement;
    public bool Holdfire;

    public int SetDamage = 25;
    private void Update()
    {
        //If user taps to the beat
        if(Input.GetMouseButtonDown(0) && musicController.canFire)
        {
            Holdfire = false;
            weaponMovement.TryShootVisual();
            musicController.canFire = false;
            FireRaycast();

           
        }
        //If user presses and holds, grant less score for kill.
        else if(Input.GetMouseButton(0) && musicController.canFire)
        {
            Holdfire = true;
            weaponMovement.TryShootVisual();
            musicController.canFire = false;
            FireRaycast();
        }
    }

    public void FireRaycast()
    {
        RaycastHit hit;
        if (RaycastFromCameraCenter(out hit))
        {
            if(hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyScript>().TakeDamage(SetDamage);
                Debug.Log("HitEnemy!");
            }
        }
    }
    bool RaycastFromCameraCenter(out RaycastHit hit)
    {
        Camera mainCamera = Camera.main;

        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        return Physics.Raycast(rayOrigin, rayDirection, out hit);
    }
}
