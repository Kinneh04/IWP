using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("BPM Controller")]
    public float BPM;
    public float BPM_Divider;
    public float DecreaseTime;
    public bool canFire;
    [Header("GameObject components")]
    public Transform Crosshair;
    public Vector3 OriginalScaleTransform;
    public GameObject CrosshairFadeGO, CrosshairFadeParent;
    public float ShootLeeway;
    float currentShootLeeway;
    public PlayerRatingController playerRating;
    float shotTime;
    //public WeaponMovement weaponMovement;
    float StartTime;
    private void Start()
    {
        OriginalScaleTransform = Crosshair.localScale;
    }

    public void SpawnFadeCrosshair()
    {
        GameObject GO = Instantiate(CrosshairFadeGO);
        GO.transform.SetParent(CrosshairFadeParent.transform, false);
        GO.GetComponent<CrosshairFadeBPM>().Speed = 1 / (BPM / BPM_Divider);
    }

    private void Update()
    {
        StartTime += Time.deltaTime;
        if (StartTime > BPM/ BPM_Divider)
        {
            StartTime = 0;
            Pulse();
            SpawnFadeCrosshair();
            playerRating.PumpScale(1.1f);
            canFire = true;
            currentShootLeeway = ShootLeeway;
        }
        if(Crosshair.localScale != OriginalScaleTransform)
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
