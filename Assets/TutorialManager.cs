using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    public bool DoTutorialChecking = false;
    public List<TutorialChunk> TutorialParts = new List<TutorialChunk>();
    MusicController mc;

    [Header("TutorialChunk")]
    TutorialChunk CurrentlyDisplayingTutorialChunk;
    public int index;
    public bool isDisplaying;
    public bool isTyping;
    

    [Header("TutorialGameObject")]
    public GameObject TutorialGameObject;
    public TMP_Text TutorialTextbox;
    public Animator TutorialMascotSpriteAnimator;
    public AnimationClip TutorialMascotSpriteBounceanimationClip;
    public TMP_Text NameText;
    public string Name = "Sigrun";
    public GameObject Nextbutton;
    


    IEnumerator TypeText(string text, float speed)
    {
        Nextbutton.SetActive(false);
        isTyping = true;
        TutorialTextbox.text = ""; // Clear the text before typing

        foreach (char c in text)
        {
            TutorialTextbox.text += c;
            yield return new WaitForSecondsRealtime(speed);
        }
        isTyping = false;
        Nextbutton.SetActive(true);
    }

    public void LoadTutorialChunk(TutorialChunk TC)
    {
        Nextbutton.SetActive(false);
        if (mc.LoggedInPlayerName != "")
        {
            Name = mc.LoggedInPlayerName + ":";
        }
        else
        {
            Name = "Sigrun:";
        }
        NameText.text = Name;
        StartCoroutine(FadeOut(mc.MusicAudioSource, 1.0f));
        TC.hasBeenDisplayed = true;
        CurrentlyDisplayingTutorialChunk = TC;
        isDisplaying = true;
        index = 0;
        TutorialGameObject.SetActive(true);
        StartCoroutine(TypeText(TC.textStrings[index], 0.05f));
        //TutorialTextbox.text = TC.textStrings[index];
    }

    public void NextTutorialString()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            TutorialTextbox.text = CurrentlyDisplayingTutorialChunk.textStrings[index];
            mc.MusicAudioSource.volume = 0;
        }
        else
        {
            index++;
            if (index >= CurrentlyDisplayingTutorialChunk.textStrings.Count)
            {
                TutorialGameObject.SetActive(false);
                isDisplaying = false;
                isTyping = false;
                Nextbutton.SetActive(true);
                StartCoroutine(FadeIn(mc.MusicAudioSource, 2.0f));
            }
            else
            {
                StartCoroutine(TypeText(CurrentlyDisplayingTutorialChunk.textStrings[index], 0.05f));
            }
        }
       
    }

    private void Start()
    {
        mc = MusicController.Instance;
    }
    private void Update()
    {
        if (!DoTutorialChecking) return;

        foreach (TutorialChunk TC in TutorialParts)
        {
            if (mc.MusicAudioSource.time >= TC.TimeShown && !TC.hasBeenDisplayed)
            {
                LoadTutorialChunk(TC);
            }
        }

    }
    public IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float i = 0;
        while (audioSource.volume > 0)
        {
            i += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, i);
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Pause();
        audioSource.volume = startVolume;
    }

    public IEnumerator FadeIn(AudioSource audioSource, float fadeDuration)
    {
        audioSource.volume = 0;
        audioSource.Play();
        float i = 0;
        while (audioSource.volume < 1)
        {
            i += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(0, 1, i);
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1;
    }
}

[System.Serializable]
public class TutorialChunk
{
    public List<string> textStrings = new List<string>();
    public GameObject[] GameobjectsToActivate;
    public float TimeShown;
    public bool hasBeenDisplayed;
}
