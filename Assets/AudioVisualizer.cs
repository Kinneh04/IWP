using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    private Vector3 originalScale;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    public float BPM, BPM_Divider;
    int BCount;
    [Header("Crosshair")]
    public GameObject LeftTarget;
    public GameObject RightTarget;
    public GameObject CrosshairL, CrosshairR, CrosshairSmallL, CrosshairSmallR;
    public GameObject CrosshairParent;
    public Transform LTarget, RTarget;
    float StartTime;
    public float CrosshairScaleMultiplier = 3.0f;
    public void SpawnLargeCrosshair(GameObject CL, GameObject CR)
    {
        GameObject LGO = Instantiate(CL, LeftTarget.transform.position, Quaternion.identity);
        GameObject RGO = Instantiate(CR, RightTarget.transform.position, Quaternion.identity);
        LGO.transform.SetParent(CrosshairParent.transform, true);
        LGO.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
        LGO.transform.localScale *= CrosshairScaleMultiplier;
        RGO.transform.localScale *= CrosshairScaleMultiplier;
        RGO.transform.SetParent(CrosshairParent.transform, true);
        CrosshairLerp CHL = LGO.GetComponent<CrosshairLerp>();
        CrosshairLerp CHR = RGO.GetComponent<CrosshairLerp>();
        CHL.target = LTarget.transform; CHR.target = RTarget.transform;
        CHL.speed = 1 / (BPM / BPM_Divider); CHR.speed = 1 / (BPM / BPM_Divider);
    }

    void Start()
    {
        audioSource.Play();
        originalScale = circleRectTransform.localScale;
    }
    public RectTransform circleRectTransform;
    public float scaleMultiplier = 10f;

    void Update()
    {
        float[] spectrumData = new float[256];
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

        float average = 0;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            average += spectrumData[i];
        }
        average /= spectrumData.Length;
        Debug.Log(average);
        float newScale = average * scaleMultiplier;
        newScale = Mathf.Clamp(newScale, originalScale.x * minScale, originalScale.x * maxScale);
        newScale = Mathf.Lerp(circleRectTransform.localScale.x, newScale, Time.deltaTime * 5f);
        circleRectTransform.localScale = new Vector3(newScale, newScale, 1f);
        StartTime += Time.fixedUnscaledDeltaTime;
        if (StartTime > BPM / BPM_Divider && average > 0.011f)
        {
            StartTime = 0;
            BCount++;
            if (BCount > 1)
            {
                SpawnLargeCrosshair(CrosshairSmallL, CrosshairSmallR);
                BCount = 0;
            }
            else SpawnLargeCrosshair(CrosshairL, CrosshairR);
        }
    }
}
