using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PlayFab;

public class MusicController : MonoBehaviour
{
    [Header("Login")]
    public string LoggedInPlayerID;
    public string LoggedInPlayerName;

    [Header("Countdown")]
    public TMP_Text countdownText;
    public int BeginningCountdown;

    [Header("BPM Controller")]
    public float BPM;
    public float BPM_Divider;
    public float DecreaseTime;
    public bool canFire;
    public bool canReload;

    [Header("LightsController")]
    public List<Light> PulsingLights;
    public Light[] TransitioningLights;
    public List<Color> LightColorPalette = new List<Color>();
    public Light CelingLight;
    public float OriginalLightIntensity;
    public float PulseSpeed;
    public float PulseLightIntensity;

   // public bool canFireButEarly;
    [Header("GameObject components")]
    public Transform Crosshair;
    public Vector3 OriginalScaleTransform;
    public GameObject CrosshairFadeGO, CrosshairFadeParent;

    [Header("Shooting")]
    public float ShootLeeway;
    float currentShootLeeway;
    public PlayerRatingController playerRating;
    //public float shotTime;
    public BPMPulse[] Pulses;
    //public WeaponMovement weaponMovement;
    float StartTime;

    [Header("Audio")]
    public AudioSource MusicAudioSource, ExtraBeatAudioSource;
    public AudioClip BeatClip, SnareClip;
    public Slider MusicProgressionSlider;
    int BeatCount;
    public bool beatAlreadyHit = false;
    public bool StartedMatch = false;
    public float EndBuffer;

    [Header("BPMREWORK")]
    public List<Intervals> _intervals = new List<Intervals>();
    public bool hasFired = false;
    public Image CrosshairOuterImage;
    int beat;

    [Header("EventsAndStuff")]
    public List<SongEvent> LoadedEvents = new List<SongEvent>();

    [Header("Finished")]
    public bool isFinished = false;
    public bool canExit = false;
    public ScoreManager scoreManager;
    public GameObject LevelClearGO;

    [Header("Glows")]
    public List<RawImage> Glows = new List<RawImage>();
    public Color OriginalGlowColor, PulseGlowColor;

    [Header("MunninsTrial")]
    public bool isPlayingMunninsTrial = false;

    [Header("Drop")]
    public bool isDrop = false;

    [Header("SFX")]
    public AudioSource SFXAudioSource;
    public AudioClip DisplayScoreAudioClip;
    public AudioClip ApplauseAudioClip;

    [Header("Fireworks")]
    public List<ParticleSystem> Fireworks = new List<ParticleSystem>();
    public ShootingScript SS;
    public AudioClip FireworksAC;
    public void PlaySFX(AudioClip AC)
    {
        SFXAudioSource.PlayOneShot(AC);
    }
    public void PulseGlows()
    {

        Color newPulseColor = PulseGlowColor;
        
        if (isDrop) newPulseColor.a = 0.65f;
        else newPulseColor.a += (100 - FirstPersonController.Instance.Health) / 100;
        foreach (RawImage RI in Glows)
        {
            RI.color = newPulseColor;
        }
    }

    public void CastFireworks()
    {
        SFXAudioSource.PlayOneShot(FireworksAC);
        foreach(ParticleSystem PS in Fireworks)
        {
            PS.Play();
        }
    }

    public void Cleanup()
    {
        isFinished = false;
        canExit = false;
        currentShootLeeway = 0;
        canFire = false;
        StartedMatch = false;
        beatAlreadyHit = false;
        MusicAudioSource.time = 0;
        MusicAudioSource.Stop();
        MusicProgressionSlider.value = 0;
        scoreManager.FinalScoreGameObject.SetActive(false);

        foreach(SongEvent SE in LoadedEvents)
        {
            SE.Played = false;
        }

        foreach(Intervals I in _intervals)
        {
            if(I.ToBeDeleted)
            {
                _intervals.Remove(I);
            }
        }

        isDrop = false;
    }

    public void LoadNewEventsFromOfficialSong(OfficialSongScript OSS)
    {
        LoadedEvents.Clear();
        foreach(SongEvent E in OSS.Events)
        {
            LoadedEvents.Add(E);
        }
        foreach(Light L in PulsingLights)
        {
            L.color = OSS.colors[0];
        }
        CelingLight.color = OSS.colors[1];


    }

    //public enum Timing
    //{
    //    Early, Perfect, Late
    //}
    private static MusicController _instance;

   // public Timing timing;
    [Header("Crosshair")]
    public GameObject LeftTarget;
    public GameObject RightTarget;
    public GameObject CrosshairL, CrosshairR, CrosshairSmallL, CrosshairSmallR;
    public float DistanceFromMiddle = 200;
    public GameObject CrosshairParent;
    public Transform LTarget, RTarget;
    public void SpawnLargeCrosshair(GameObject CL, GameObject CR)
    {
        GameObject LGO = Instantiate(CL, LeftTarget.transform.position, Quaternion.identity);
        GameObject RGO = Instantiate(CR, RightTarget.transform.position, Quaternion.identity);
        LGO.transform.SetParent(CrosshairParent.transform, true);
        LGO.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
        RGO.transform.SetParent(CrosshairParent.transform, true);
        CrosshairLerp CHL = LGO.GetComponent<CrosshairLerp>();
        CrosshairLerp CHR = RGO.GetComponent<CrosshairLerp>();
        CHL.target = LTarget.transform; CHR.target = RTarget.transform;
        CHL.speed = 1 / (BPM / BPM_Divider); CHR.speed = 1 / (BPM / BPM_Divider);
        PulseGlows();
    }

    public void StartMusic()
    {
        MusicAudioSource.PlayScheduled(0);
    }
    private void Start()
    {
       
        OriginalScaleTransform = Crosshair.localScale;
       // StartMusic();
    }

    public void SpawnFadeCrosshair()
    {
        GameObject GO = Instantiate(CrosshairFadeGO);
        GO.transform.SetParent(CrosshairFadeParent.transform, false);
        GO.GetComponent<CrosshairFadeBPM>().Speed = 1 / (BPM / BPM_Divider);

  
    }

    public IEnumerator FadeOutMusic()
    {
        while(MusicAudioSource.volume > 0)
        {
            MusicAudioSource.volume -= Time.deltaTime;
            yield return null;
        }
        MusicAudioSource.Pause();
    }
    public static MusicController Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicController>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MusicController");
                    _instance = singletonObject.AddComponent<MusicController>();
                }
            }

            return _instance;
        }
    }

    // Optional: Add any other methods or properties you need for your MainMenuManager

    private void Awake()
    {
        // Ensure there's only one instance of this object
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
    }
    public void PlayBeatAudio()
    {
        ExtraBeatAudioSource.PlayOneShot(BeatClip);
    }
    public void PlaySnareAudio()
    {
        ExtraBeatAudioSource.PlayOneShot(SnareClip);
    }

    public void SpawnCorrectCrosshairPulses()
    {
        if (!FirstPersonController.Instance.canMove) return;
        beat++;
        CrosshairOuterImage.color = Color.white;
        if(beat == 2)
        {
            beat = 0;
            SpawnLargeCrosshair(CrosshairSmallL, CrosshairSmallR);
        }
        else SpawnLargeCrosshair(CrosshairL, CrosshairR);
        foreach (BPMPulse BPMP in Pulses) BPMP.Pulse();
        Pulses = GameObject.FindObjectsOfType<BPMPulse>();
        if (FirstPersonController.Instance.isLow)
        {
            FirstPersonController.Instance.HeartbeatAudioSource.PlayOneShot(FirstPersonController.Instance.Heartbeat);
        }
    }


    public IEnumerator StartMatch()
    {
        yield return new WaitForSeconds(1);
        int cooldown = BeginningCountdown;
        countdownText.gameObject.SetActive(true);
        countdownText.text = cooldown.ToString();
        float start = 0.0f;
        while(cooldown > 0)
        {
            start += Time.deltaTime;
            if(start >= BPM / BPM_Divider)
            {
                PlayBeatAudio();
                cooldown--;
                countdownText.text = cooldown.ToString();
                start = 0f;
                if(cooldown <= 0)
                {
                    EnemySpawner.Instance.StartEnemySpawning();
                    StartMusic();
                    StartedMatch = true;
                    countdownText.gameObject.SetActive(false);
                    PlayBeatAudio();
                    

                    yield break;
                }
              
            }
            yield return null;
        }
    }

    public void StartShootLeewayCoroutine()
    {
        StartCoroutine(ShotLeewayCoroutine());
    }

    public IEnumerator ShotLeewayCoroutine()
    {
        yield return new WaitForSecondsRealtime(ShootLeeway);
        canFire = false;
        hasFired = false;
        canReload = false;
    }

    public void PulseDungeonLights()
    {
        foreach (Light L in PulsingLights)
        {
            L.intensity = PulseLightIntensity;
        }
    }

    public void UpdateLights()
    {
        foreach(Light L in PulsingLights)
        {
            if(L.intensity > OriginalLightIntensity)
            {
                L.intensity = Mathf.Lerp(L.intensity, OriginalLightIntensity, PulseSpeed * Time.deltaTime);
            }
        }
    }

    public IEnumerator StartFinishGameSequence()
    {
        
        EnemySpawner.Instance.Cleanup();
        isFinished = true;
        LevelClearGO.SetActive(true);
        SFXAudioSource.PlayOneShot(ApplauseAudioClip);
        SS.weaponMovement.gameObject.SetActive(false);
        SS.freefire = false;
        canFire = false;

        scoreManager.ChangeLevelCompleteVars(playerRating.Targetscore, playerRating.KillAmount, playerRating.HighestCombo, playerRating.MultikillAmount, playerRating.TargetAcc);
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            scoreManager.LoginToSaveScoreGO.SetActive(true);
        }
        else
        {
            scoreManager.LoginToSaveScoreGO.SetActive(false);
        }
        yield return new WaitForSeconds(3f);
        LevelClearGO.SetActive(false);
        scoreManager.FinalScoreGameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            SFXAudioSource.PlayOneShot(DisplayScoreAudioClip);
            yield return new WaitForSeconds(0.1f);
        }
        canExit = true;
    }

    private void Update()
    {
        if (!StartedMatch) return;


        Color gc = Glows[0].color;
        gc = Color.Lerp(gc, OriginalGlowColor, 3.5f * Time.deltaTime);
        foreach(RawImage RI in Glows)
        {
            RI.color = gc;
        }

        if(MusicAudioSource.time >= MusicAudioSource.clip.length - EndBuffer && !isFinished)
        {
            StartCoroutine(StartFinishGameSequence());
        
        }
        foreach (Intervals interval in _intervals)
        {
            if(interval == null || interval.ToBeDeleted)
            {
                _intervals.Remove(interval);
            }
            if (interval.ToBeDeleted) return;
            float sampledTime = (MusicAudioSource.timeSamples / (MusicAudioSource.clip.frequency * interval.GetIntervalLength(BPM_Divider)));
            // Debug.Log(sampledTime);
            bool f = interval.CheckForNewInterval(sampledTime, ShootLeeway);
            if(f && !hasFired)
            {
                canFire = true;
                canReload = true;
            }
        }
        UpdateLights();
        //OLD METHOD! DEPRECIATED!
        if (MusicAudioSource.isPlaying)
        {
            float currentTime = MusicAudioSource.time;
            float totalTime = MusicAudioSource.clip.length;

            MusicProgressionSlider.value = currentTime / totalTime;
            UpdateEvents();
        }
        //StartTime += Time.deltaTime;

        //if (StartTime >= BPM / BPM_Divider)
        //{
        //    timing = Timing.Perfect;
        //    StartTime = 0;
        //    Pulse();
        //    BeatCount++;
        //    PlayBeatAudio();

        //    if (BeatCount == 2)
        //    {
        //        BeatCount = 0;
        //        PlaySnareAudio();
        //        SpawnLargeCrosshair(CrosshairSmallL, CrosshairSmallR);
        //        foreach (BPMPulse BPMP in Pulses) BPMP.Pulse();
        //    }
        //    else
        //    {
        //        SpawnLargeCrosshair(CrosshairL, CrosshairR);
        //    }

        //    //SpawnFadeCrosshair();
        //    playerRating.PumpScale(1.1f);

        //    currentShootLeeway = ShootLeeway;

        //    // Set canFire to true for 50ms after the beat
        //    if (!beatAlreadyHit)
        //        canFire = true;
        //    StartCoroutine(ResetCanFire());

        //}
        //else if (StartTime >= (BPM / BPM_Divider) - 0.1f)
        //{
        //    if(!beatAlreadyHit)
        //    canFire = true;

        //    timing = Timing.Early;
        //}

        //// Define a coroutine to reset canFire after the leeway period
        //IEnumerator ResetCanFire()
        //{
        //    yield return new WaitForSeconds(ShootLeeway);
        //    canFire = false;
        //    beatAlreadyHit = false;
        //}
        if (Crosshair.localScale != OriginalScaleTransform)
        {
            Crosshair.localScale = Vector3.Lerp(Crosshair.localScale, OriginalScaleTransform, DecreaseTime * Time.deltaTime);
        }

        //if(canFire)
        //{
        //    shotTime += Time.deltaTime;
        //    currentShootLeeway -= Time.deltaTime;
        //    if (currentShootLeeway <= 0)
        //    {
        //        canFire = false;
        //        shotTime = 0;
        //    }
        //    else if (currentShootLeeway <= ShootLeeway / 2)
        //    {
        //        timing = Timing.Late;
        //    }
        //}

        if(CrosshairOuterImage.color.a > 0.2f)
        {
            Color newA = CrosshairOuterImage.color;
            newA.a = Mathf.Lerp(newA.a, 0.2f, Time.deltaTime * 3);
            CrosshairOuterImage.color = newA;
        }
    }
    public void Pulse()
    {
        Crosshair.localScale = OriginalScaleTransform * 1.5f;
    }

    public void UpdateEvents()
    {
        if(LoadedEvents.Count > 0)
        {
            foreach(SongEvent SE in LoadedEvents)
            {
                if(MusicAudioSource.time >= SE.castTimer && !SE.Played)
                {
                    SE.CastEvent();
                    SE.Played = true;
                }
            }
        }
    }

}
[System.Serializable]
public class Intervals
{
    public bool ToBeDeleted = false;
    public float _steps;
    public UnityEvent _trigger;
    public int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public bool CheckForNewInterval(float interval, float leeway)
    {
        
        if((int)Mathf.FloorToInt(interval) != (int)_lastInterval)
        {
          //  Debug.Log("HIT! " + (int)Mathf.FloorToInt(interval) + " " + (int)_lastInterval);
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
            return true;
        }
        else if(_lastInterval != 0 && interval -_lastInterval >1 - leeway)
        {
            return true;
        }
        return false;
    }
}