using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [Header("BPM Controller")]
    public float BPM;
    public float BPM_Divider;
    public float DecreaseTime;
    public bool canFire;
    public bool canFireButEarly;
    [Header("GameObject components")]
    public Transform Crosshair;
    public Vector3 OriginalScaleTransform;
    public GameObject CrosshairFadeGO, CrosshairFadeParent;
    public float ShootLeeway;
    float currentShootLeeway;
    public PlayerRatingController playerRating;
    public float shotTime;
    public BPMPulse[] Pulses;
    //public WeaponMovement weaponMovement;
    float StartTime;
    public AudioSource MusicAudioSource, ExtraBeatAudioSource;
    public AudioClip BeatClip, SnareClip;
    public Slider MusicProgressionSlider;
    int BeatCount;
    bool firedOnce = false;
    public enum Timing
    {
        Early, Perfect, Late
    }

    public Timing timing;
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
    }

    public void StartMusic()
    {
        MusicAudioSource.PlayScheduled(0);
    }
    private void Start()
    {
        Pulses = GameObject.FindObjectsOfType<BPMPulse>();
        OriginalScaleTransform = Crosshair.localScale;
        StartMusic();
    }

    public void SpawnFadeCrosshair()
    {
        GameObject GO = Instantiate(CrosshairFadeGO);
        GO.transform.SetParent(CrosshairFadeParent.transform, false);
        GO.GetComponent<CrosshairFadeBPM>().Speed = 1 / (BPM / BPM_Divider);
    }

    private void PlayBeatAudio()
    {
        ExtraBeatAudioSource.PlayOneShot(BeatClip);
    }
    private void PlaySnareAudio()
    {
        ExtraBeatAudioSource.PlayOneShot(SnareClip);
    }
    private void Update()
    {
        if (MusicAudioSource.isPlaying)
        {
            float currentTime = MusicAudioSource.time;
            float totalTime = MusicAudioSource.clip.length;

            MusicProgressionSlider.value = currentTime / totalTime;
        }
        StartTime += Time.fixedUnscaledDeltaTime;
      
        if (StartTime > BPM / BPM_Divider)
        {
            timing = Timing.Perfect;
            StartTime = 0;
            Pulse();
            BeatCount++;
            PlayBeatAudio();
          
            if (BeatCount == 2)
            {
                BeatCount = 0;
                PlaySnareAudio();
                SpawnLargeCrosshair(CrosshairSmallL, CrosshairSmallR);
            }
            else
            {
                SpawnLargeCrosshair(CrosshairL, CrosshairR);
            }
            foreach (BPMPulse BPMP in Pulses) BPMP.Pulse();
           //SpawnFadeCrosshair();
            playerRating.PumpScale(1.1f);

            currentShootLeeway = ShootLeeway;
            firedOnce = false;

        }
        else if (StartTime > BPM / BPM_Divider - ShootLeeway && !canFire && !firedOnce)
        {
            canFire = true;
            firedOnce = true;
            timing = Timing.Early;
        }
        else 
        if (Crosshair.localScale != OriginalScaleTransform)
        {
            Crosshair.localScale = Vector3.Lerp(Crosshair.localScale, OriginalScaleTransform, DecreaseTime * Time.deltaTime);
        }

        if(canFire)
        {
            shotTime += Time.deltaTime;
            currentShootLeeway -= Time.deltaTime;
            if (currentShootLeeway <= 0)
            {
                canFire = false;
                shotTime = 0;
            }
            else if (currentShootLeeway <= ShootLeeway / 2)
            {
                timing = Timing.Late;
            }
        }
    }
    public void Pulse()
    {
        Crosshair.localScale = OriginalScaleTransform * 1.5f;
    }

    public bool isLate()
    {
        if(shotTime > BPM / BPM_Divider + ShootLeeway / 4)
        {
            return true;
        }
        return false;
    }
}
