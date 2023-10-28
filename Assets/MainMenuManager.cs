using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    public Animator MainMenuAnimator;
    public AnimationClip ShowSubbuttonsAnimationClip;
    public List<GameObject> GameobjectsToEnableForDemo = new List<GameObject>();
    public List<GameObject> GameobjectsToDisableForDemo = new List<GameObject>();
    public GameObject LoadingScreen;

    [Header("MainMenu")]
    public GameObject MainMenuUI;
    public AudioSource MainMenuAudioSource;
    [Header("EditorScreen")]
    public SongEditorManager SEM;
    public GameObject EditorScreen;
    public AudioSource CustomEditorAudioSource;
    public GameObject SongPickerScreen;

    public IEnumerator FadeOutAudioSource(AudioSource AS)
    {
        while(AS.volume > 0)
        {
            yield return null;
            AS.volume = Mathf.Lerp(AS.volume, 0, 3 * Time.deltaTime);
        }
        AS.volume = 0;
        AS.Pause();
    }

    public IEnumerator FadeInAudioSource(AudioSource AS)
    {
        AS.Play();
        while (AS.volume < 1)
        {
            yield return null;
            AS.volume = Mathf.Lerp(AS.volume, 1, 3 * Time.deltaTime);
        }
        AS.volume = 1;
        
    }

    public void MakeNewCustomSongButton()
    {
        StartCoroutine(MakeNewCustomSongCoroutine());
    }
    public IEnumerator MakeNewCustomSongCoroutine()
    {
        StartCoroutine(FadeOutAudioSource(MainMenuAudioSource));
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(false);
        SongPickerScreen.SetActive(true);
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        SEM.Cleanup();
        StartCoroutine(FadeInAudioSource(CustomEditorAudioSource));
    }

    public void ExitSongpicker()
    {
        StartCoroutine(ExitSongPickerCoroutine());
    }

    public IEnumerator ExitSongPickerCoroutine()
    {
        if(CustomEditorAudioSource.clip != null)
        {
            MainMenuAudioSource.clip = CustomEditorAudioSource.clip;
            MainMenuAudioSource.time = CustomEditorAudioSource.time;
        }
        StartCoroutine(FadeOutAudioSource(CustomEditorAudioSource));
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(true);
        SongPickerScreen.SetActive(false);
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        StartCoroutine(FadeInAudioSource(MainMenuAudioSource));
    }

    public void OnClickEditorButton()
    {
        StartCoroutine(EditorSequence());
    }

    public IEnumerator EditorSequence()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(true);
        MainMenuUI.SetActive(false);
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        
    }

    public IEnumerator ReturnToMainMenuSequence()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(false);
        MainMenuUI.SetActive(true);
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuSequence());
    }
    public void ExitGame()
    {
        SaveData();
        Application.Quit();
    }

    public void PressPlayButton()
    {
        StartCoroutine(PlaydemoSequence());
    }

    public IEnumerator PlaydemoSequence()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        foreach (GameObject GO in GameobjectsToDisableForDemo)
        {
            GO.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            GO.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        LoadingScreen.SetActive(false);
        yield return new WaitForSeconds(1);
        BlackscreenController.Instance.FadeIn();
    }

    public void SaveData()
    {
        Debug.Log("Havent implemented yet lmao");
    }

    public void ClickOnMainButton()
    {
        MainMenuAnimator.Play(ShowSubbuttonsAnimationClip.name);
    }
}