using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public int OGDifficulty;
    public EnemyScript AttachedBossEnemyScript;
    public GameObject BossHealth;
    public TMP_Text BossNameText;
    public Slider BossHealthSlider;

    public GameObject BossKilledFX;
    public GameObject BossKilledUIElements;
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

            Intervals BossStartI = new Intervals();
            BossStartI._steps = 1;
            BossStartI._trigger = new UnityEngine.Events.UnityEvent();
            BossStartI._trigger.AddListener(delegate { InstantiatedBoss.ChooseRandomAttack(); });
            BossStartI._lastInterval = 1;
            MC._intervals.Add(BossStartI);
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
    public void UpdateHealthSlider()
    {
        BossHealthSlider.value = AttachedBossEnemyScript.Health;
    }
    public void KillBoss()
    {
        BossHealth.SetActive(false);
        Destroy(InstantiatedBoss.gameObject);
        CurrentlyChosenBoss = null;
        AttachedBossEnemyScript = null;
        StartCoroutine(KillbossEffects());
        PlayerRatingController.Instance.AddRating(100, "Boss Slain", Color.cyan);

     
    }

    public IEnumerator KillbossEffects()
    {
        if (BossKilledFX)
        {
            BossKilledFX.SetActive(true);
        }
        if (BossKilledUIElements)
        {
            BossKilledUIElements.SetActive(true);
        }

        yield return new WaitForSeconds(2);
        BossKilledFX.SetActive(false);
        BossKilledUIElements.SetActive(false);
    }
    public IEnumerator SpawnBossCoroutine(Boss B)
    {
        OGDifficulty = EnemySpawner.Instance.difficulty;
        EnemySpawner.Instance.difficulty = 2;
        LightbeamEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        FirstPersonController.Instance.isTransitioning = true;
        EnemySpawner.Instance.RemoveAllEnemies();
        EnemySpawner.Instance.AllowedToSpawn = false;
        OriginalFOV = TargetFOV;
        GameObject GO = Instantiate(B.BossObject, SpawnPosition.position, Quaternion.identity);
        CurrentlyChosenBoss = B;
        BossNameText.text = B.BossName;
        InstantiatedBoss = GO.GetComponent<BossScript>();
        AttachedBossEnemyScript = GO.GetComponent<EnemyScript>();
        AttachedBossEnemyScript.AttachedBossManager = this;
        PlayerRoot.LookAt(GO.transform.position);
        AddedInterval = new Intervals();
        AddedInterval._steps = 1;
        AddedInterval._trigger = new UnityEngine.Events.UnityEvent();
        AddedInterval._trigger.AddListener(delegate { ZoomInOnBoss1Factor(); });
        AddedInterval._lastInterval = 0;
        BossHealthSlider.maxValue = AttachedBossEnemyScript.Health;
        BossHealthSlider.value = AttachedBossEnemyScript.Health;
        MC._intervals.Add(AddedInterval);
        BossHealth.SetActive(true);
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
