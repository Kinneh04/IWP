using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public MusicController musicController;
    public WeaponMovement weaponMovement;
    public PlayerRatingController playerRatingController;
    public bool Holdfire;
    public int SetDamage = 25;
    [Header("Tracer")]
    public float tracerMoveSpeed;
    public Transform GunShootFrom;
    public GameObject Tracer;
    public GameObject MuzzleFlash;
    private void Update()
    {
        //If user taps to the beat
        if(Input.GetMouseButtonDown(0) && musicController.canFire)
        {
            Holdfire = false;
            weaponMovement.TryShootVisual();
            musicController.canFire = false;
            FireRaycast();

            if (musicController.isLate())
            {
                Debug.Log("Late!");
                playerRatingController.AddRating(5, "Late Beat!");
            }
            else
            {
                playerRatingController.AddRating(10, "On Beat!");
            }
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
                playerRatingController.AddRating(10, "Enemy Hit!");
            }
            SpawnTracer(hit.point);
        }
        GameObject GO =Instantiate(MuzzleFlash, GunShootFrom.transform.position, Quaternion.identity);
        GO.transform.SetParent(GunShootFrom.transform);
    }
    bool RaycastFromCameraCenter(out RaycastHit hit)
    {
        Camera mainCamera = Camera.main;

        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        return Physics.Raycast(rayOrigin, rayDirection, out hit);
    }

    public void SpawnTracer(Vector3 Destination)
    {
        GameObject NewTracer = Instantiate(Tracer, GunShootFrom.transform.position, Quaternion.identity);
        StartCoroutine(MoveTracer(NewTracer, Destination));
    }

    public IEnumerator MoveTracer(GameObject GO, Vector3 Destination)
    {
        while(Vector3.Distance(GO.transform.position, Destination) > 0.1f)
        {
            GO.transform.position = Vector3.Lerp(GO.transform.position, Destination, tracerMoveSpeed * Time.deltaTime);
            yield return null;

        }
    }
}
