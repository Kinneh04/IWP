
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRatingController : MonoBehaviour
{
    [Header("Rating")]
    public float Rating;
    public Image RatingImage;
    public Slider RatingSlider;
    public List<RatingClass> ratingClasses = new List<RatingClass>();
    public GameObject RatingPrefab;
    public int currentRatingIndex = 0;
    public int killCombo = 0;
    public int HighestCombo = 0;
    [Header("Multiplier")]
    public float Multiplier = 1.0f;
    public float AddToMultipliercooldown;
    public float currentMultiplierCooldown;
    public TMP_Text multiplierText;

    [Header("KillFeed")]
    public Transform KillfeedParent;
    public GameObject KillFeedPrefab;
    private Vector3 Minscale, MaxScale;
    public float MaxScaleMultiplier;
    public GameObject Tracer;
    public List<GameObject> InstantiatedKillFeedPrefabs = new List<GameObject>();

    [Header("Frenzy")]
    public Slider FrenzySlider;
    public float CurrentFrenzyAmount;
    public float AmountNeededForFrenzy;
    public float decreaseRate;
    public float FrenzyDecreaseAmount;
    bool frenzyAvailable = false;
    public GameObject SpaceToActivateGO;

    [Header("ScoreAndAccuracy")]
    public int ShotsFired;
    public int ShotsHit;
    public int Targetscore;
    public int currentScore;
    public float currentAcc, TargetAcc;
    public TMP_Text ScoreTMP_Text, AccuracyTMP_Text;

    [Header("Crosshair")]
    public GameObject HitmarkerGameObject;
    public AudioClip HitmarkerAudioSource;
    public float cooldown;
    public float addToCooldown;

    [Header("Scoring")]
    public int KillAmount = 0;
    public int MultikillAmount = 0;
    public GameObject GoodDodgeGO;

    [Header("Components")]
    public ShootingScript shootingScript;
    private static PlayerRatingController _instance;

    public static PlayerRatingController Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerRatingController>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerRatingController");
                    _instance = singletonObject.AddComponent<PlayerRatingController>();
                }
            }

            return _instance;
        }
    }
    public void Cleanup()
    {
        ShotsFired = 0;
        Multiplier = 1.0f;
        killCombo = 0;
        currentMultiplierCooldown = 0;

        KillAmount = 0;
        frenzyAvailable = false;
        CurrentFrenzyAmount = 0;
        Rating = 0;
        currentRatingIndex = 0;
        ShotsHit = 0;
        currentScore = 0;
        Targetscore = 0;
        TargetAcc = 100f;
        currentAcc = TargetAcc;
        ScoreTMP_Text.text = currentScore.ToString("000000");
        AccuracyTMP_Text.text = TargetAcc.ToString() + "%";
        SpaceToActivateGO.SetActive(false);

        foreach(GameObject GO in InstantiatedKillFeedPrefabs)
        {
            if (!GO) continue;
            Destroy(GO);
        }
        InstantiatedKillFeedPrefabs.Clear();
        shootingScript.cleanup();
    }

    public void TargetAndRecalculateAcc()
    {
        TargetAcc = (float)ShotsHit / (float)ShotsFired * 100;
    }
    

    public void AddMissedShot()
    {
        ShotsFired++;
        TargetAndRecalculateAcc();
        Multiplier = 1.0f;
        currentMultiplierCooldown = 0;
        killCombo = 0;
    }
    public void OnKillEnemy()
    {
        killCombo++;
        if (killCombo > HighestCombo) HighestCombo = killCombo;
        KillAmount++;
        if(killCombo <= 1)
        {
            AddRating(10, "Single Kill");
        }
        else if(killCombo == 2)
        {
            AddRating(15, "Double Kill");
        }
        else if (killCombo == 3)
        {
            AddRating(20, "Triple Kill", Color.yellow);
        }
        else if (killCombo == 4)
        {
            AddRating(30, "Quad Kill", Color.yellow);
        }
        else if (killCombo >= 5)
        {
            AddRating(40, "Multi Kill", Color.cyan);
            MultikillAmount++;
        }
    }
    public void AddHitShot()
    {
        ShotsFired++; ShotsHit++;
        TargetAndRecalculateAcc();
        MusicController.Instance.PlaySFX(HitmarkerAudioSource);
        HitmarkerGameObject.SetActive(true);
        cooldown = addToCooldown;
        Multiplier += 0.05f * killCombo;
        if (Multiplier > 5) Multiplier = 5;
        currentMultiplierCooldown = AddToMultipliercooldown;
    }

    public void AddToFrenzy(float rating)
    {
        if (shootingScript.FrenzyMode) return;
        CurrentFrenzyAmount += rating;
        FrenzySlider.value = CurrentFrenzyAmount;
        if(CurrentFrenzyAmount > AmountNeededForFrenzy + 100)
        {
            CurrentFrenzyAmount = AmountNeededForFrenzy + 100;
        }
        if (FrenzySlider.value >= FrenzySlider.maxValue)
        {
            frenzyAvailable = true;
            SpaceToActivateGO.SetActive(true);
            //shootingScript.StartFrenzyMode();
        }
    }

    public void DecreaseFrenzy(float rating)
    {
        if (frenzyAvailable) return;
        CurrentFrenzyAmount -= rating;
        FrenzySlider.value = CurrentFrenzyAmount;
        if(CurrentFrenzyAmount < AmountNeededForFrenzy)
        {
            frenzyAvailable = false;
        }
        if (CurrentFrenzyAmount <= 0)
        {
            CurrentFrenzyAmount = 0;
            if (shootingScript.FrenzyMode) shootingScript.EndFrenzyMode();
        }
    }


    private void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;

        RatingImage.sprite = ratingClasses[currentRatingIndex].RatingSprite;
        RatingSlider.maxValue = ratingClasses[currentRatingIndex].MaximumRating;
        RatingSlider.value = Rating;
        Minscale = RatingImage.transform.localScale;
        MaxScale = Minscale * MaxScaleMultiplier;
        FrenzySlider.maxValue = AmountNeededForFrenzy;
    }

    public IEnumerator GoodDodge()
    {
        GoodDodgeGO.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GoodDodgeGO.SetActive(false);
        AddRating(10, "Good Dodge!", Color.green);
    }

    public void AddRating(float rating, string ShowString = null, Color? c = null)
    {
        Rating += rating * Multiplier;
        if(rating > 0) RatingSlider.value = Rating;
        Targetscore += (int)(rating * Multiplier);

        if(ShowString != "" && ShowString != null)
        {
            GameObject GO = Instantiate(KillFeedPrefab);
            GO.transform.SetParent(KillfeedParent, true);
            InstantiatedKillFeedPrefabs.Add(GO);
            TMP_Text text = GO.GetComponent<TMP_Text>();
            if (rating < 0)
            {
                text.text = "- " + ShowString;
                text.color = Color.red;
            }
            else
            {
                text.text = "+ " + ShowString;
                if(c != null)
                {
                    text.color = (Color)c;
                }
            }
        }
        PumpScale(1.15f);
        AddToFrenzy(rating);

        if (Rating >= ratingClasses[currentRatingIndex].MaximumRating) IncreaseRatingBy1();
        
    }

    public void PumpScale(float multiplier)
    {
        RatingImage.transform.localScale *= multiplier;
        if (RatingImage.transform.localScale.x > MaxScale.x) RatingImage.transform.localScale = MaxScale;
    }

    public void RemoveRating(float rating)
    {
        Rating -= rating;
        if (Rating < 0) Rating = 0;
        RatingSlider.value = Rating;
        if (Rating <= ratingClasses[currentRatingIndex].MinimumRating) DecreaseRatingBy1();
    }

    public void IncreaseRatingBy1()
    {
        if (currentRatingIndex + 1 > ratingClasses.Count - 1)
        {
            return;
        }
        PumpScale(1.15f);
        currentRatingIndex++;
        RatingPrefab.SetActive(true);
        Rating = 50;
        RatingImage.sprite = ratingClasses[currentRatingIndex].RatingSprite;
        RatingSlider.maxValue = ratingClasses[currentRatingIndex].MaximumRating;
    }
    public void DecreaseRatingBy1()
    {

        if (currentRatingIndex - 1 < 0) return;
        else
        {
            PumpScale(1.15f);
            currentRatingIndex--;
            RatingImage.sprite = ratingClasses[currentRatingIndex].RatingSprite;
            RatingSlider.maxValue = ratingClasses[currentRatingIndex].MaximumRating;
            Rating = RatingSlider.maxValue - RatingSlider.maxValue / 5;
        }
    }

    private void Update()
    {

        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0)
            {
                HitmarkerGameObject.SetActive(false);
            }
        }

        if(currentAcc != TargetAcc)
        {
            currentAcc = Mathf.Lerp(currentAcc, TargetAcc, Time.deltaTime * 3);
            AccuracyTMP_Text.text = currentAcc.ToString("F2") + "%";
        }
        if(currentScore != Targetscore)
        {
            currentScore = (int)Mathf.Lerp(currentScore, Targetscore, Time.deltaTime * 2);
            ScoreTMP_Text.text = currentScore.ToString("000000");
        }

        RemoveRating(0.1f * (currentRatingIndex + 1));
        if (RatingImage.transform.localScale.x > Minscale.x) RatingImage.transform.localScale *= 0.995f;

        if(!shootingScript.FrenzyMode)
        {
            DecreaseFrenzy(decreaseRate * Time.deltaTime);
        }
        else
        {
            DecreaseFrenzy(FrenzyDecreaseAmount * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Q) && frenzyAvailable)
        {
            shootingScript.StartFrenzyMode();
            frenzyAvailable = false;
            SpaceToActivateGO.SetActive(false);
        }

        if(FrenzySlider.value >= FrenzySlider.maxValue)
        {
            FrenzySlider.fillRect.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        }
        else
        {
            FrenzySlider.fillRect.GetComponent<Image>().color = Color.red;
        }

        if(currentMultiplierCooldown > 0)
        {
            currentMultiplierCooldown -= Time.deltaTime;
        }
        else
        {
            Multiplier -= Time.deltaTime * 0.1f;
            killCombo = 0;
            if (Multiplier < 1) Multiplier = 1.0f;
        
        }
        multiplierText.text = "x" + Multiplier.ToString("F2") + " Multiplier";
    }
}

[Serializable]
public class RatingClass
{
    public Sprite RatingSprite;
    public int MinimumRating;
    public int MaximumRating;
}