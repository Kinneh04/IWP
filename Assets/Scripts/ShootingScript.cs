using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShootingScript : MonoBehaviour
{
    public MusicController musicController;
    public WeaponMovement weaponMovement;
    public PlayerRatingController playerRatingController;
    public bool Holdfire;
    public int SetDamage = 25;

    [Header("Animations")]
    public Animator PistolAnimator;
    public AnimationClip PistolFireAnimClip, PistolReloadAnimClip;
    [Header("Tracer")]
    public float tracerMoveSpeed;
    public Transform GunShootFrom;
    public GameObject Tracer;
    public GameObject MuzzleFlash;
    [Header("Frenzy mode and gatling gun")]
    public bool FrenzyMode = false;
    public float gatlingGunCooldown;
    public float AddToGatlingGunCooldown = 0.1f;
    public GameObject[] GatlingGunMuzzleFlashPoints;

    public GameObject Minigun, Revolver;

    [Header("Ammo")]
    public int maxAmmo;
    public int CurrentAmmo;
    public bool isReloading;
    public AnimationClip ReloadAnimClip;
    public float reloadTime;
    public TMP_Text AmmoCountText;

    [Header("antiSpam")]
    public bool isSpamming = false;
    private float clickCount = 0;
    private float lastClickTime = 0;
    public GameObject AntiSpamGO;

    private void Start()
    {
        CurrentAmmo = maxAmmo;
        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    void DispenseAmmo()
    {
        CurrentAmmo -= 1;
        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public IEnumerator StartReload()
    {
        if (!isReloading)
        {
            isReloading = true;
            PistolAnimator.Play(ReloadAnimClip.name);
            yield return new WaitForSeconds(reloadTime);
            if (CurrentAmmo > 0) CurrentAmmo = maxAmmo + 1;
            else CurrentAmmo = maxAmmo;
            AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
            isReloading = false;
        }
    }

    public void StartFrenzyMode()
    {
        Revolver.SetActive(false);
        Minigun.SetActive(true);
        FrenzyMode = true;
    }

    public void EndFrenzyMode()
    {
        Revolver.SetActive(true);
        Minigun.SetActive(false);
        FrenzyMode = false;
    }
    private void Update()
    {
        if (!FrenzyMode)
        {
            if(!isReloading && Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(StartReload());
            }
            if (CurrentAmmo > 0 && !isReloading)
            {
                //If user taps to the beat
                if (Input.GetMouseButtonDown(0)&& !isReloading)
                {

                    float currentTime = Time.time;

                    // If it's been more than 1 second since the last click, reset the click count
                    if (currentTime - lastClickTime > 0.20f)
                    {
                        clickCount = 1;
                    }
                    else
                    {
                        clickCount++;
                    }

                    lastClickTime = currentTime;

                    // Check if the click count is over 4
                    if (clickCount > 6)
                    {
                        clickCount = 6;
                        isSpamming = true;
                        playerRatingController.AddRating(-5, "Spam Detected!");
                        AntiSpamGO.SetActive(true);
                    }
                    else
                    {
                        isSpamming = false;
                        AntiSpamGO.SetActive(false);
                    }
                    if(!isSpamming)
                    {
                        playerRatingController.AddRating(5, "On Beat!");
                    }
                    Holdfire = false;
                    weaponMovement.TryShootVisual();
                    FireRaycast();
                    DispenseAmmo();
                    PistolAnimator.Play(PistolFireAnimClip.name);
                    //if (musicController.isLate())
                    //{
                    //    Debug.Log("Late!");
                    //    playerRatingController.AddRating(5, "Late Beat!");
                    //}
                    //else
                    //{
                    //    playerRatingController.AddRating(10, "On Beat!");
                    //}
                    GameObject GO = Instantiate(MuzzleFlash, GunShootFrom.transform.position, Quaternion.identity);
                    GO.transform.SetParent(GunShootFrom.transform);
                    musicController.canFire = false;
                    musicController.canFireButEarly = false;
                }
                //If user presses and holds, grant less score for kill.
                else if (Input.GetMouseButton(0) && musicController.canFire && !isReloading)
                {
                    clickCount--;
                    if (clickCount < 5)
                    {
                        isSpamming = false;
                        AntiSpamGO.SetActive(false);
                    }
                    Holdfire = true;
                    PistolAnimator.Play(PistolFireAnimClip.name);
                    weaponMovement.TryShootVisual();
                    DispenseAmmo();
                    musicController.canFire = false;
                    FireRaycast();
                    GameObject GO = Instantiate(MuzzleFlash, GunShootFrom.transform.position, Quaternion.identity);
                    GO.transform.SetParent(GunShootFrom.transform);
                }
                else if (Input.GetMouseButtonUp(0)) Holdfire = false;
            }
            else
            {
                if(Input.GetMouseButton(0) && musicController.canFire)
                {
                    StartCoroutine(StartReload());
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && gatlingGunCooldown <= 0)
            {
                FireRaycast(false);
                gatlingGunCooldown = AddToGatlingGunCooldown;
                foreach (GameObject GO in GatlingGunMuzzleFlashPoints)
                {
                    GameObject MFGO = Instantiate(MuzzleFlash, GO.transform.position, Quaternion.identity);
                    MFGO.transform.SetParent(GO.transform);
                }
            }
            else if (gatlingGunCooldown > 0) gatlingGunCooldown -= Time.deltaTime;
            
        }
    }

    public void FireRaycast(bool AddToAcc = true)
    {
        RaycastHit hit;
        if (RaycastFromCameraCenter(out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyScript>().TakeDamage(SetDamage);
                Debug.Log("HitEnemy!");
                playerRatingController.AddRating(10, "Enemy Hit!");
                playerRatingController.AddHitShot();
            }
            else if (hit.collider.CompareTag("Coin"))
            {
                hit.collider.GetComponent<CoinScript>().Ricochet();
                playerRatingController.AddHitShot();
            }
            else
            {
                playerRatingController.AddMissedShot();
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

    public void SpawnGatlingGunTracer(Vector3 Destination)
    {
        foreach (GameObject GO in GatlingGunMuzzleFlashPoints)
        {
            GameObject NewTracer = Instantiate(Tracer, GO.transform.position, Quaternion.identity);
            StartCoroutine(MoveTracer(NewTracer, Destination));
        }
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
        //Destroy(GO);
    }
}
