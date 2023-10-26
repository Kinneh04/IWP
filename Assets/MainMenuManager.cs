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
