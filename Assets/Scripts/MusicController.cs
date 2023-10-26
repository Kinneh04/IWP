using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
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
            StartTime = 0;
            Pulse();
            BeatCount++;
            PlayBeatAudio();
            if (BeatCount == 2)
            {
                BeatCount = 0;
                PlaySnareAudio();
            }
            foreach (BPMPulse BPMP in Pulses) BPMP.Pulse();
            SpawnFadeCrosshair();
            playerRating.PumpScale(1.1f);

            currentShootLeeway = ShootLeeway;
            firedOnce = false;

        }
        else if (StartTime > BPM / BPM_Divider - ShootLeeway && !canFire && !firedOnce)
        {
            canFire = true;
            firedOnce = true;
        }
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
