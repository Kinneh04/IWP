using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    private Vector3 originalScale;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public bool spawnCrosshairs = true;
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
    public float barMultiplier = 4.0f;
    public float glowMultiplier;
    public RawImage mainGlow, backGroundGlow;
    public List<Transform> bars = new List<Transform>();
    public GameObject MountainGO;
    public float MountainScaler;
    public Vector3 OGMountainScale;
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
        if(!audioSource.isPlaying)
            audioSource.Play();
        originalScale = circleRectTransform.localScale;
        if (MountainGO)
        {
            OGMountainScale = MountainGO.transform.localScale;
        }
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
        for (int i = 0; i < bars.Count; i++)
        {
            float targetScaleY = spectrumData[i] * (1 - (i / (float)bars.Count - 1)) * barMultiplier;
            Vector3 targetScale = new Vector3(2, targetScaleY, 1);

            bars[i].transform.localScale = Vector3.Lerp(bars[i].transform.localScale, targetScale, Time.deltaTime * 3.0f);
        }
        average /= spectrumData.Length;
       // Debug.Log(average);

        float newScale = average * scaleMultiplier;
        newScale = Mathf.Clamp(newScale, originalScale.x * minScale, originalScale.x * maxScale);
        newScale = Mathf.Lerp(circleRectTransform.localScale.x, newScale, Time.deltaTime * 2f);
        circleRectTransform.localScale = new Vector3(newScale, newScale, 1f);

        if (MountainGO)
        {
            float newMountainScale = average * MountainScaler;
            newMountainScale = Mathf.Clamp(newMountainScale, OGMountainScale.x * minScale, OGMountainScale.x * maxScale);
            newMountainScale = Mathf.Lerp(MountainGO.transform.localScale.x, newMountainScale, Time.deltaTime * 2f);
            MountainGO.transform.localScale = new Vector3(newMountainScale, newMountainScale, 1f);
        }

        float opacity = average * glowMultiplier;
        Color color = mainGlow.color;
        color.a = Mathf.Clamp01(opacity); // Ensure opacity is between 0 and 1
        float targetOpacity = average * glowMultiplier;
        float currentOpacity = mainGlow.color.a;

        float newOpacity = Mathf.Lerp(currentOpacity, targetOpacity, Time.deltaTime * 3.0f);
        color.a = newOpacity;
        mainGlow.color = color;

        if (spawnCrosshairs)
        {
            StartTime += Time.deltaTime;
            if (StartTime > BPM / BPM_Divider && average > 0.009f)
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
}
