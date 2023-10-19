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

    private void Start()
    {
        RatingImage.sprite = ratingClasses[currentRatingIndex].RatingSprite;
        RatingSlider.maxValue = ratingClasses[currentRatingIndex].MaximumRating;
        RatingSlider.value = Rating;
        Minscale = RatingImage.transform.localScale;
        MaxScale = Minscale * MaxScaleMultiplier;
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
    }



}

[Serializable]
public class RatingClass
{
    public Sprite RatingSprite;
    public int MinimumRating;
    public int MaximumRating;
}