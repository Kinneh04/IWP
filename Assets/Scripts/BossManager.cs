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
    public TMP_Text TimeLeftToKillText;
    public GameObject BossKilledUIElements;
    public GameObject BossEscapedUIElements;
    Intervals BossStartI = new Intervals();
    public float TimeLeftToKill = 0;
    private static BossManager _instance;

    public void StartFinisher()
    {
        EnemySpawner.Instance.RemoveAllEnemies();
        EnemySpawner.Instance.AllowedToSpawn = false;
        MusicController.Instance.MusicAudioSource.Pause();
        InstantiatedBoss.canStartAttacking = false;
        PlayerRatingController.Instance.shootingScript.freefire = true;
        //     BossKilledFX.SetActive(false);
        //BossKilledUIElements.SetActive(false);
        BossHealth.SetActive(false);
        InstantiatedBoss.BossAnimator.Play(InstantiatedBoss.HurtAnimation.name);
        InstantiatedBoss.finisher = true;

    }
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
    public static BossManager Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<BossManager>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MainMenuManager");
                    _instance = singletonObject.AddComponent<BossManager>();
                }
            }

            return _instance;
        }
    }

    private void Update()
    {
        if(CVC.m_Lens.FieldOfView != TargetFOV)
        {
            CVC.m_Lens.FieldOfView = Mathf.Lerp(CVC.m_Lens.FieldOfView, TargetFOV, 7.0f * Time.deltaTime);
        }
        if (InstantiatedBoss && TimeLeftToKill > 0 && !InstantiatedBoss.finisher)
        {
            TimeLeftToKill -= Time.deltaTime;
            TimeLeftToKillText.text = "TIME TO KILL: " + TimeLeftToKill.ToString("F1");
            if(TimeLeftToKill <= 0)
            {
                BossEscape();
                
            }
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

            BossStartI = new Intervals();
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
        if(InstantiatedBoss) Destroy(InstantiatedBoss.gameObject);
        CurrentlyChosenBoss = null;
        AttachedBossEnemyScript = null;
        StartCoroutine(KillbossEffects());
        PlayerRatingController.Instance.AddRating(100, "Boss Slain", Color.cyan);
        BossStartI.ToBeDeleted = true;
     //   EnemySpawner.Instance.difficulty = OGDifficulty;
    }

    public void Cleanup()
    {
        BossHealth.SetActive(false);
        if (InstantiatedBoss) Destroy(InstantiatedBoss.gameObject);
        CurrentlyChosenBoss = null;
        AttachedBossEnemyScript = null;
        if(BossStartI == null) BossStartI.ToBeDeleted = true;
        EnemySpawner.Instance.difficulty = OGDifficulty;
        LightbeamEffect.SetActive(false);
    }

    public void BossEscape()
    {
        BossHealth.SetActive(false);
        Destroy(InstantiatedBoss.gameObject);
        InstantiatedBoss = null;
        CurrentlyChosenBoss = null;
        AttachedBossEnemyScript = null;
        StartCoroutine(BossEscapeEffects());
        PlayerRatingController.Instance.AddRating(-100, "Boss Escaped", Color.red);
        BossStartI.ToBeDeleted = true;
        EnemySpawner.Instance.difficulty = OGDifficulty;
    }
    public IEnumerator BossEscapeEffects()
    {
        if (BossEscapedUIElements)
        {
            BossEscapedUIElements.SetActive(true);
        }

        yield return new WaitForSeconds(2);
        BossEscapedUIElements.SetActive(false);
    }
    public IEnumerator KillbossEffects()
    {

        BossKilledUIElements.SetActive(true);
        
        MusicController.Instance.CastFireworks();
        yield return new WaitForSeconds(3);

        StartCoroutine(MusicController.Instance.StartFinishGameSequence());
        yield return new WaitForSeconds(3);
        BossKilledUIElements.SetActive(false);
    }
    public IEnumerator SpawnBossCoroutine(Boss B)
    {
        OGDifficulty = EnemySpawner.Instance.difficulty;
        EnemySpawner.Instance.difficulty = 0;
        LightbeamEffect.SetActive(true);
        yield return new WaitForSeconds(0.9f);
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
        //PlayerRoot.LookAt(GO.transform.position);
        float t = 0;
        Quaternion startRotation = PlayerRoot.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(GO.transform.position - PlayerRoot.position);

        while (t < 1)
        {
            t += Time.deltaTime * 10.0f; // You need to define the duration for the lerp
            PlayerRoot.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null; // Wait for the next frame
        }
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

    public void SpawnBossByName(string name, float time = 0)
    {
        foreach(Boss B in AvailableBosses)
        {
            if(B.BossName == name)
            {
                spawnBoss(B);
                TimeLeftToKill = time;
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
