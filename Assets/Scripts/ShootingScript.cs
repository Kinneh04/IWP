using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


public class ShootingScript : MonoBehaviour
{
    public MusicController musicController;
    public WeaponMovement weaponMovement, UziWeaponMovement;
    
    public PlayerRatingController playerRatingController;
    public bool Holdfire;
    public int SetDamage = 25;
    public int BurstShotCount = 1;
    public float burstShotInterval = 0.05f;

    [Header("Animations")]
    public Animator MainWeaponAnimator;
    public AnimationClip FireAnimClip;


    [Header("Tracer")]
    public float tracerMoveSpeed;
    public Transform GunShootFrom;
    public GameObject Tracer;
    public GameObject MuzzleFlash;
    public float MaxShotDistance;

    [Header("Frenzy mode and gatling gun")]
    public bool FrenzyMode = false;
    public GameObject[] GatlingGunMuzzleFlashPoints;
    public Animator GatlingGunAnimator;
    public AnimationClip UziShootAnimClip;

    public GameObject Minigun, Revolver;
    public bool freefire = false;
    [Header("Ammo")]
    public int maxAmmo;
    public int CurrentAmmo;
    public bool isReloading;
    public List<WeaponReloadPart> ReloadAnimClips = new List<WeaponReloadPart>();

    [Header("ForRevolverBasedReloads")]
    public WeaponReloadPart StartingReloadAnim, EndingReloadAnim, repeatingReloadAnim;
    public bool isRevolverReloadType = false;
    public int revolverReloadTypeIndex = 0;

    public int ReloadIndex = 0;
    public float reloadTime;
    public TMP_Text AmmoCountText;
    public int NumberOfBulletsPerShot;
    public float spreadAngle;
    [Header("antiSpam")]
    public bool isSpamming = false;
    private float clickCount = 0;
    private float lastClickTime = 0;
    public GameObject AntiSpamGO;
    [Header("LateEarlyRating")]
    public TMP_Text LateEarlyRatingText;

    [Header("Sounds")]
    public AudioSource PlayerAS;
    public AudioClip ShootingAudioClip, ClickAudioClip, FrenzyAudioClip, FrenzyShootAudioClip;

    [Header("Frenzy")]
    public GameObject FrenzyUI;
    public GameObject[] ShitToDisableForFrenzy;
    public int FrenzyShotsFired;
    public bool holdingFrenzy;
    public Intervals FrenzyInterval;

    private static ShootingScript _instance;
    public static ShootingScript Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<ShootingScript>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MusicController");
                    _instance = singletonObject.AddComponent<ShootingScript>();
                }
            }

            return _instance;
        }
    }
    private void Awake()
    {
        // Ensure there's only one instance of this object
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        FrenzyInterval._steps = 2;
        FrenzyInterval._trigger.AddListener(delegate { ShootGatlingGun(); });

        MusicController.Instance._intervals.Add(FrenzyInterval);
    }
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

    public void cleanup()
    {
        if(FrenzyMode)
        {
            EndFrenzyMode();
            CurrentAmmo = maxAmmo;
        }
    }

    private void Start()
    {
        CurrentAmmo = maxAmmo;
        AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    void DispenseAmmo()
    {
        if (!FirstPersonController.Instance.canMove) return;
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
        FrenzyUI.SetActive(true);
        foreach(GameObject GO in ShitToDisableForFrenzy)
        {
            GO.SetActive(false);
        }
    }

    public void EndFrenzyMode()
    {
        Revolver.SetActive(true);
        Minigun.SetActive(false);
        FrenzyMode = false;
        FrenzyUI.SetActive(false);
        foreach (GameObject GO in ShitToDisableForFrenzy)
        {
            GO.SetActive(true);
        }
    }

    public void IncrementReload()
    {
        musicController.hasFired = true;
        if (isRevolverReloadType)
        {

            if(revolverReloadTypeIndex == 0)
            {
                MainWeaponAnimator.Play(StartingReloadAnim.animClip.name);
                revolverReloadTypeIndex++;
                PlayerAS.PlayOneShot(StartingReloadAnim.audioClip);
            }
            else if(revolverReloadTypeIndex == 1)
            {
                if(CurrentAmmo >= maxAmmo)
                {
                    revolverReloadTypeIndex++;
                }
                else
                {
                    MainWeaponAnimator.Play(repeatingReloadAnim.animClip.name);
                    PlayerAS.PlayOneShot(repeatingReloadAnim.audioClip);
                    CurrentAmmo++;
                    AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
                }
            }
            if(revolverReloadTypeIndex >= 2)
            {
                revolverReloadTypeIndex = 0;
                MainWeaponAnimator.Play(EndingReloadAnim.animClip.name);
                PlayerAS.PlayOneShot(EndingReloadAnim.audioClip);
                AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
            }
        }
        else
        {
            isReloading = true;
            MainWeaponAnimator.Play(ReloadAnimClips[ReloadIndex].animClip.name);
            PlayerAS.PlayOneShot(ReloadAnimClips[ReloadIndex].audioClip);
            ReloadIndex++;
            if (ReloadIndex >= ReloadAnimClips.Count)
            {
                ReloadIndex = 0;
                CurrentAmmo = maxAmmo;
                isReloading = false;
                musicController.canFire = false;
                AmmoCountText.text = CurrentAmmo.ToString() + "/" + maxAmmo.ToString();
            }
        }
    }

    IEnumerator burstShot()
    {
        
        for(int i = 0; i < BurstShotCount; i++)
        {
            FireRaycast();
            DispenseAmmo();
            yield return new WaitForSeconds(burstShotInterval);
        }
    }

    private void Update()
    {
        if (freefire)
        {
            if (Input.GetKeyDown(KeyCode.R)
                     || Input.GetMouseButtonDown(1))
            {
                if(CurrentAmmo < maxAmmo || isRevolverReloadType)
                    IncrementReload();
            }
            if (Input.GetMouseButtonDown(0) && !isReloading && FirstPersonController.Instance.canMove && CurrentAmmo > 0)
            {
                revolverReloadTypeIndex = 0;
                weaponMovement.TryShootVisual();
                FireRaycast(forcedOneBullet: true);
                DispenseAmmo();
                //  ShowTimingRating();
                if (FireAnimClip)
                {
                    MainWeaponAnimator.Play(FireAnimClip.name);
                }
            }
        }
        else
        {
            if (!FrenzyMode)
            {

                if (Input.GetKeyDown(KeyCode.R) && musicController.canReload
                    || Input.GetMouseButtonDown(1) && musicController.canReload)
                {
                    if (CurrentAmmo < maxAmmo || isRevolverReloadType)
                        IncrementReload();
                }
                else if (Input.GetKeyDown(KeyCode.R) && CurrentAmmo >= maxAmmo && musicController.canReload)
                {
                    LateEarlyRatingText.text = "Ammo Full!";
                    LateEarlyRatingText.color = Color.yellow;
                }
                else if (Input.GetKeyDown(KeyCode.R) && !musicController.canReload)
                {
                    LateEarlyRatingText.text = "Missed!";
                    LateEarlyRatingText.color = Color.yellow;
                }
                if (CurrentAmmo > 0 && !isReloading && musicController.canFire)
                {
                    //If user taps to the beat
                    if (Input.GetMouseButtonDown(0) && !isReloading && FirstPersonController.Instance.canMove)
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
                        if (!isSpamming)
                        {
                            playerRatingController.AddRating(1);
                        }
                        Holdfire = false;
                        weaponMovement.TryShootVisual();
                        revolverReloadTypeIndex = 0;
                        if (BurstShotCount > 1)
                        {
                            StartCoroutine(burstShot());
                        }
                        else
                        {
                            FireRaycast();
                            DispenseAmmo();
                        }//  ShowTimingRating();
                        if (FireAnimClip)
                        {
                            MainWeaponAnimator.Play(FireAnimClip.name);
                        }
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
                    if (Input.GetMouseButtonDown(0) && !musicController.canFire && CurrentAmmo > 0 && !FirstPersonController.Instance.isTransitioning)
                    {
                        LateEarlyRatingText.text = "Missed!";
                        LateEarlyRatingText.color = Color.yellow;
                        PlayerAS.PlayOneShot(ClickAudioClip);
                        playerRatingController.AddMissedShot();
                    }
                    else if (Input.GetMouseButtonDown(0) && CurrentAmmo <= 0)
                    {
                        LateEarlyRatingText.text = "No Ammo!";
                        LateEarlyRatingText.color = Color.yellow;
                        PlayerAS.PlayOneShot(ClickAudioClip);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    holdingFrenzy = true;
                }
                else
                {
                    holdingFrenzy = false;
                    FrenzyShotsFired = 0;
                }

            }
            Color T = Color.red;
            T.a = 0;
            LateEarlyRatingText.color = Color.Lerp(LateEarlyRatingText.color, T, Time.deltaTime * 3);
        }
    }

    public void ShootGatlingGun()
    {
        if (holdingFrenzy && FrenzyMode)
        {
           
            FrenzyShotsFired++;
            //if (FrenzyShotsFired >= 17)
            //{
            //    FrenzyInterval._steps = 8;
            //  //  FrenzyInterval._lastInterval++;
            //}
            if (FrenzyShotsFired == 8)
            {
                FrenzyInterval._steps = 4;
             //   FrenzyInterval._lastInterval++;
            }
            else if (FrenzyShotsFired == 3)
            {
                FrenzyInterval._steps = 2;
               // FrenzyInterval._lastInterval++;
            }
            else
            {
                UziWeaponMovement.TryShootVisual();
                PlayerAS.PlayOneShot(FrenzyAudioClip);
                FireRaycast(false);
                MusicController.Instance.SFXAudioSource.PlayOneShot(FrenzyAudioClip);
                foreach (GameObject GO in GatlingGunMuzzleFlashPoints)
                {
                    GameObject MFGO = Instantiate(MuzzleFlash, GO.transform.position, Quaternion.identity);
                    MFGO.transform.SetParent(GO.transform);
                    GatlingGunAnimator.Play(UziShootAnimClip.name);
                }
            }
        }
        else
        {
            FrenzyShotsFired = 0;
            FrenzyInterval._steps = 1 ;
        }
    }


    public void FireRaycast(bool AddToAcc = true, bool forcedOneBullet = false)
    {
        if (!FirstPersonController.Instance.canMove) return;
        bool first = false;
        for (int i = 0; i < NumberOfBulletsPerShot; i++)
        {
            
            RaycastHit hit;

            // Adjust the ray direction for shotgun spread
            Vector3 rayDirection = Camera.main.transform.forward; // Assuming cameraTransform is your camera's transform
            if(i > 0) rayDirection = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0) * rayDirection;

            if (RaycastFromCameraCenter(out hit, rayDirection))
            {
                if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss"))
                {
                    hit.collider.GetComponent<EnemyScript>().TakeDamage(SetDamage, first);
                    if (AddToAcc)
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

                SpawnTracer(hit.point);
            }
            else
            {
                if (AddToAcc)
                    playerRatingController.AddMissedShot();
            }
            first = true;
            if (forcedOneBullet) return;
        }
    }
    bool RaycastFromCameraCenter(out RaycastHit hit, Vector3 direction = default)
    {
        Transform cameraTransform = Camera.main.transform;
        if (direction == default)
            direction = cameraTransform.forward;

        Ray ray = new Ray(cameraTransform.position, direction);
        return Physics.Raycast(ray, out hit, MaxShotDistance);
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
        
        while(GO && Vector3.Distance(GO.transform.position, Destination) > 1f)
        {
            GO.transform.position += GO.transform.forward * Time.deltaTime * tracerMoveSpeed;
            yield return null;

        }
        if(GO) Destroy(GO);
    }
}

[System.Serializable]
public class WeaponReloadPart
{
    public AnimationClip animClip;
    public AudioClip audioClip;
}
