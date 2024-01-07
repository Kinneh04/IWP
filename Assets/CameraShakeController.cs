using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    public CinemachineVirtualCamera CVC;
    float originalShakeFrequency;
    float currentShakeFrequency;
    private static CameraShakeController _instance;
    public CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    public static CameraShakeController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraShakeController>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("CameraShakeController");
                    _instance = singletonObject.AddComponent<CameraShakeController>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //originalShakeFrequency = CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
        //currentShakeFrequency = originalShakeFrequency;
        if (!cinemachineBasicMultiChannelPerlin) cinemachineBasicMultiChannelPerlin =CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if(currentShakeFrequency != originalShakeFrequency)
        {
            currentShakeFrequency = Mathf.Lerp(currentShakeFrequency, originalShakeFrequency, 2.0f * Time.deltaTime);
           cinemachineBasicMultiChannelPerlin.m_FrequencyGain = currentShakeFrequency;
        }
        if(originalShakeFrequency <= 0)
        {
            originalShakeFrequency = cinemachineBasicMultiChannelPerlin.m_FrequencyGain;
            currentShakeFrequency = originalShakeFrequency;
        }
        if (!cinemachineBasicMultiChannelPerlin) cinemachineBasicMultiChannelPerlin = CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void AddCameraShake(float camShake)
    {
        currentShakeFrequency = camShake;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain += currentShakeFrequency;
    }
}
