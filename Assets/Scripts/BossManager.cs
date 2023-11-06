using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public List<Boss> AvailableBosses = new List<Boss>();
    public BossScript InstantiatedBoss;
    public MusicController MC;
    public GameObject LightbeamEffect;
    public Transform SpawnPosition;
    public int zoomFactor;
    int currentZoomFactor = 0;
    float OriginalFOV;
    public float addToFoV;
    public float TargetFOV;
    public Intervals AddedInterval;
    public Boss CurrentlyChosenBoss;
    public Transform PlayerRoot;
    public CinemachineVirtualCamera CVC;
    private void Update()
    {
        if(CVC.m_Lens.FieldOfView != TargetFOV)
        {
            CVC.m_Lens.FieldOfView = Mathf.Lerp(CVC.m_Lens.FieldOfView, TargetFOV, 7.0f * Time.deltaTime);
        }
    }
    public void ZoomInOnBoss1Factor()
    {
        currentZoomFactor++;
        if (currentZoomFactor >= zoomFactor)
        {
            TargetFOV = OriginalFOV;
            currentZoomFactor = 0;
            AddedInterval.ToBeDeleted = true;

            EnemySpawner.Instance.AllowedToSpawn = true;
            InstantiatedBoss.canStartAttacking = true;
            FirstPersonController.Instance.isTransitioning = false;
        }
        else
        {
            TargetFOV -= addToFoV;
        }
    }

    public void spawnBoss(Boss B)
    {
        StartCoroutine(SpawnBossCoroutine(B));
    }

    public IEnumerator SpawnBossCoroutine(Boss B)
    {

        LightbeamEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        FirstPersonController.Instance.isTransitioning = true;
        EnemySpawner.Instance.RemoveAllEnemies();
        EnemySpawner.Instance.AllowedToSpawn = false;
        OriginalFOV = TargetFOV;
        GameObject GO = Instantiate(B.BossObject, SpawnPosition.position, Quaternion.identity);
        CurrentlyChosenBoss = B;
        InstantiatedBoss = GO.GetComponent<BossScript>();
        PlayerRoot.LookAt(GO.transform.position);
        AddedInterval = new Intervals();
        AddedInterval._steps = 1;
        AddedInterval._trigger = new UnityEngine.Events.UnityEvent();
        AddedInterval._trigger.AddListener(delegate { ZoomInOnBoss1Factor(); });
        AddedInterval._lastInterval = 0;
        MC._intervals.Add(AddedInterval);
    }

    public void SpawnBossByName(string name)
    {
        foreach(Boss B in AvailableBosses)
        {
            if(B.BossName == name)
            {
                spawnBoss(B);
                return;
            }
        }
    }
}

[System.Serializable]

public class Boss
{
    public string BossName;
    public GameObject BossObject;
    public float Health;
    public GameObject Extrashit;
}
