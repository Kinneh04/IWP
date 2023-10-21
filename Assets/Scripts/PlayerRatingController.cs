using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("KillFeed")]
    public Transform KillfeedParent;
    public GameObject KillFeedPrefab;
    private Vector3 Minscale, MaxScale;
    public float MaxScaleMultiplier;
    public GameObject Tracer;

    [Header("Frenzy")]
    public Slider FrenzySlider;
    public float CurrentFrenzyAmount;
    public float AmountNeededForFrenzy;
    public float decreaseRate;
    public float FrenzyDecreaseAmount;
    bool frenzyAvailable = false;
    public GameObject SpaceToActivateGO;

    [Header("Components")]
    public ShootingScript shootingScript;


    public void AddToFrenzy(float rating)
    {
        if (shootingScript.FrenzyMode) return;
        CurrentFrenzyAmount += rating;
        FrenzySlider.value = CurrentFrenzyAmount;
        if(CurrentFrenzyAmount > AmountNeededForFrenzy + 100)
        {
            CurrentFrenzyAmount = AmountNeededForFrenzy + 100;
        }
        if (CurrentFrenzyAmount >= AmountNeededForFrenzy)
        {
            frenzyAvailable = true;
            SpaceToActivateGO.SetActive(true);
            //shootingScript.StartFrenzyMode();
        }
    }

    public void DecreaseFrenzy(float rating)
    {
        CurrentFrenzyAmount -= rating;
        FrenzySlider.value = CurrentFrenzyAmount;
        if(CurrentFrenzyAmount < AmountNeededForFrenzy)
        {
            frenzyAvailable = false;
            SpaceToActivateGO.SetActive(false);
        }
        if (CurrentFrenzyAmount <= 0)
        {
            CurrentFrenzyAmount = 0;
            if (shootingScript.FrenzyMode) shootingScript.EndFrenzyMode();
        }
    }


    private void Start()
    {
        RatingImage.sprite = ratingClasses[currentRatingIndex].RatingSprite;
        RatingSlider.maxValue = ratingClasses[currentRatingIndex].MaximumRating;
        RatingSlider.value = Rating;
        Minscale = RatingImage.transform.localScale;
        MaxScale = Minscale * MaxScaleMultiplier;
        FrenzySlider.maxValue = AmountNeededForFrenzy;
        IncreaseRatingBy1();
    }

    public void AddRating(float rating, string ShowString = null)
    {
        Rating += rating;
        RatingSlider.value = Rating;

        if(ShowString != "" && ShowString != null)
        {
            GameObject GO = Instantiate(KillFeedPrefab);
            GO.transform.SetParent(KillfeedParent, true);
            TMP_Text text = GO.GetComponent<TMP_Text>();
            text.text = ShowString + " +" + rating.ToString();
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
       
        if (currentRatingIndex - 1 < 0) RatingPrefab.SetActive(false);
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

        if(Input.GetKeyDown(KeyCode.Space) && frenzyAvailable)
        {
            shootingScript.StartFrenzyMode();
            frenzyAvailable = false;
            SpaceToActivateGO.SetActive(false);
        }
    }



}

[Serializable]
public class RatingClass
{
    public Sprite RatingSprite;
    public int MinimumRating;
    public int MaximumRating;
}