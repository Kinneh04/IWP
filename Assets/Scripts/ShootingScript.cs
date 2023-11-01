using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ShootingScript : MonoBehaviour
{
    public MusicController musicController;
    public WeaponMovement weaponMovement, UziWeaponMovement;
    
    public PlayerRatingController playerRatingController;
    public bool Holdfire;
    public int SetDamage = 25;

    [Header("Animations")]
    public Animator PistolAnimator;
    public AnimationClip PistolFireAnimClip;


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
    public Animator GatlingGunAnimator;
    public AnimationClip UziShootAnimClip;

    public GameObject Minigun, Revolver;

    [Header("Ammo")]
    public int maxAmmo;
    public int CurrentAmmo;
    public bool isReloading;
    public List<WeaponReloadPart> PistolReloadAnimClips = new List<WeaponReloadPart>();
    public int ReloadIndex = 0;
    public float reloadTime;
    public TMP_Text AmmoCountText;

    [Header("antiSpam")]
    public bool isSpamming = false;
    private float clickCount = 0;
    private float lastClickTime = 0;
    public GameObject AntiSpamGO;
    [Header("LateEarlyRating")]
    public TMP_Text LateEarlyRatingText;
    public float spreadAngle;

    [Header("Sounds")]
    public AudioSource PlayerAS;
    public AudioClip ShootingAudioClip, ClickAudioClip;
   
    //public void ShowTimingRating()
    //{
    //    StopCoroutine(ShowTimingRatingCoroutine());
    //    StartCoroutine(ShowTimingRatingCoroutine());
    //}

    //IEnumerator ShowTimingRatingCoroutine()
    //{
    //   // LateEarlyRatingText.text = musicController.timing.ToString();
    //    LateEarlyRatingText.gameObject.SetActive(false);
    //    LateEarlyRatingText.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(0.3f);
    //    LateEarlyRatingText.gameObject.SetActive(false);
    //}

    private void Start()
    {
        CurrentAmmo = maxAmmo;
        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    void DispenseAmmo()
    {
        CurrentAmmo -= 1;
        PlayerAS.PlayOneShot(ShootingAudioClip);
        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    //public IEnumerator StartReload()
    //{
    //    if (!isReloading)
    //    {
    //        isReloading = true;
    //        PistolAnimator.Play(ReloadAnimClip.name);
    //        yield return new WaitForSeconds(reloadTime);
    //        if (CurrentAmmo > 0) CurrentAmmo = maxAmmo + 1;
    //        else CurrentAmmo = maxAmmo;
    //        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    //        isReloading = false;
    //    }
    //}

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

    public void IncrementReload()
    {
        isReloading = true;
        musicController.hasFired = true;
        PistolAnimator.Play(PistolReloadAnimClips[ReloadIndex].animClip.name);
        PlayerAS.PlayOneShot(PistolReloadAnimClips[ReloadIndex].audioClip);
        ReloadIndex++;
        if(ReloadIndex >= PistolReloadAnimClips.Count)
        {
            ReloadIndex = 0;
            if (CurrentAmmo != 0) CurrentAmmo = maxAmmo + 1;
            else CurrentAmmo = maxAmmo;
            isReloading = false;
            AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
        }
    }

    private void Update()
    {
        if (!FrenzyMode)
        {
            if(Input.GetKeyDown(KeyCode.R) && musicController.canReload && CurrentAmmo < maxAmmo)
            {
                IncrementReload();
            }
            if (CurrentAmmo > 0 && !isReloading && musicController.canFire)
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
                        playerRatingController.AddRating(1);
                    }
                    Holdfire = false;
                    weaponMovement.TryShootVisual();
                    FireRaycast();
                    DispenseAmmo();
                  //  ShowTimingRating();
                    PistolAnimator.Play(PistolFireAnimClip.name);
                    musicController.beatAlreadyHit = true;
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
                    musicController.hasFired = true;
                    musicController.canReload = false;
                  //  musicController.canFireButEarly = false;
                }
                //If user presses and holds, grant less score for kill.
                //else if (Input.GetMouseButton(0) && musicController.canFire && !isReloading)
                //{
                //    clickCount--;
                //    if (clickCount < 5)
                //    {
                //        isSpamming = false;
                //        AntiSpamGO.SetActive(false);
                //    }
                //    Holdfire = true;
                //    PistolAnimator.Play(PistolFireAnimClip.name);
                //    weaponMovement.TryShootVisual();
                //    DispenseAmmo();
                //    musicController.canFire = false;
                //    FireRaycast();
                //    GameObject GO = Instantiate(MuzzleFlash, GunShootFrom.transform.position, Quaternion.identity);
                //    GO.transform.SetParent(GunShootFrom.transform);
                //}
                else if (Input.GetMouseButtonUp(0)) Holdfire = false;
            }
            else
            {
                if(Input.GetMouseButtonDown(0) && !musicController.canFire && CurrentAmmo > 0)
                {
                    LateEarlyRatingText.text = "Missed!";
                    LateEarlyRatingText.color = Color.red;
                    PlayerAS.PlayOneShot(ClickAudioClip);
                }
                else if(Input.GetMouseButtonDown(0) && CurrentAmmo <= 0)
                {
                    LateEarlyRatingText.text = "No Ammo!";
                    PlayerAS.PlayOneShot(ClickAudioClip);
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && gatlingGunCooldown <= 0)
            {
                UziWeaponMovement.TryShootVisual();
                FireRaycast(false);
                gatlingGunCooldown = AddToGatlingGunCooldown;
                foreach (GameObject GO in GatlingGunMuzzleFlashPoints)
                {
                    GameObject MFGO = Instantiate(MuzzleFlash, GO.transform.position, Quaternion.identity);
                    MFGO.transform.SetParent(GO.transform);
                    GatlingGunAnimator.Play(UziShootAnimClip.name);
                }
            }
            else if (gatlingGunCooldown > 0) gatlingGunCooldown -= Time.deltaTime;
            
        }
        Color T = Color.red;
        T.a = 0;
        LateEarlyRatingText.color = Color.Lerp(LateEarlyRatingText.color, T, Time.deltaTime * 3);
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
                if(AddToAcc)
                playerRatingController.AddHitShot();
            }
            else if (hit.collider.CompareTag("Coin"))
            {
                hit.collider.GetComponent<CoinScript>().Ricochet();
                if (AddToAcc)
                    playerRatingController.AddHitShot();
            }
            else
            {
                if (AddToAcc)
                    playerRatingController.AddMissedShot();
            }
        }
        SpawnTracer(hit.point);
        
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
        Quaternion randomRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);

        GameObject NewTracer = Instantiate(Tracer, GunShootFrom.transform.position, Camera.main.transform.rotation * randomRotation);
        StartCoroutine(MoveTracer(NewTracer, Destination));
    }

    public IEnumerator MoveTracer(GameObject GO, Vector3 Destination)
    {
        while(Vector3.Distance(GO.transform.position, Destination) > 1f)
        {
            GO.transform.position += GO.transform.forward * Time.deltaTime * tracerMoveSpeed;
            yield return null;

        }
        Destroy(GO);
    }
}

[System.Serializable]
public class WeaponReloadPart
{
    public AnimationClip animClip;
    public AudioClip audioClip;
}
