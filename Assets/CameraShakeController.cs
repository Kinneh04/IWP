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
    }
    private void Start()
    {
        originalShakeFrequency = CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
        currentShakeFrequency = originalShakeFrequency;
    }

    private void Update()
    {
        if(currentShakeFrequency != originalShakeFrequency)
        {
            currentShakeFrequency = Mathf.Lerp(currentShakeFrequency, originalShakeFrequency, 2.0f * Time.deltaTime);
            CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = currentShakeFrequency;
        }
    }

    public void AddCameraShake(float camShake)
    {
        currentShakeFrequency = camShake;
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = currentShakeFrequency;
    }
}
