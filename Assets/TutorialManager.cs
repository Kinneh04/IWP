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

    [Header("Audio")]
    public AudioSource SFXAudioSource;
    public AudioClip ClickAudioClip;


    [Header("Input")]
    public StarterAssetsInputs SAI;


    IEnumerator TypeText(string text, float speed)
    {
        Nextbutton.SetActive(false);
        isTyping = true;
        TutorialTextbox.text = ""; // Clear the text before typing

        foreach (char c in text)
        {
            TutorialTextbox.text += c;
            SFXAudioSource.PlayOneShot(ClickAudioClip);
            yield return new WaitForSecondsRealtime(speed);
        }
        isTyping = false;
        Nextbutton.SetActive(true);
    }

    public void LoadTutorialChunk(TutorialChunk TC)
    {
        SAI.cursorLocked = false;
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
        StartCoroutine(FadeOut(mc.MusicAudioSource, 5.0f));
        TC.hasBeenDisplayed = true;
        CurrentlyDisplayingTutorialChunk = TC;
        isDisplaying = true;
        index = 0;
        TutorialGameObject.SetActive(true);
        StartCoroutine(TypeText(TC.textStrings[index], 0.02f));
        //TutorialTextbox.text = TC.textStrings[index];
    }

    public void NextTutorialString()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            TutorialTextbox.text = CurrentlyDisplayingTutorialChunk.textStrings[index];
            mc.MusicAudioSource.volume = 0;
            Nextbutton.SetActive(true);
            isTyping = false;
        }
        else
        {
            index++;
            if (index >= CurrentlyDisplayingTutorialChunk.textStrings.Count)
            {
                SAI.cursorLocked = true;
                TutorialGameObject.SetActive(false);
                isDisplaying = false;
                isTyping = false;
                Nextbutton.SetActive(false);
                StartCoroutine(FadeIn(mc.MusicAudioSource, 8.0f));
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
                Debug.Log("CurrentSourceTime: " + mc.MusicAudioSource.time);
                LoadTutorialChunk(TC);
            }
        }

    }
    public IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float i = 0;
        FirstPersonController.Instance.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        while (audioSource.volume > 0)
        {
            i += Time.deltaTime * fadeDuration;
            Time.timeScale = Mathf.Lerp(1, 0, i);
            audioSource.volume -= Time.deltaTime * fadeDuration;
            yield return new WaitForSecondsRealtime(0.016f);
        }

        audioSource.Pause();
        audioSource.volume = startVolume;
    }

    public IEnumerator FadeIn(AudioSource audioSource, float fadeDuration)
    {
        audioSource.volume = 0;
        audioSource.UnPause();
        float i = 0;
        FirstPersonController.Instance.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        while (audioSource.volume < 1)
        {
            i += Time.deltaTime * fadeDuration;
            Time.timeScale = Mathf.Lerp(0.3f, 1, i);
            audioSource.volume += Time.deltaTime * fadeDuration;
            yield return new WaitForSecondsRealtime(0.016f);
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
