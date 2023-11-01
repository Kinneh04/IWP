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

    [Header("MainGamescreen")]
    public GameObject MainGameSelectionScreen;
    [Header("EditorScreen")]
    public SongEditorManager SEM;
    public GameObject EditorScreen;
    public AudioSource CustomEditorAudioSource;
    public GameObject SongPickerScreen;
    private static MainMenuManager _instance;

    [Header("CustomSongs")]
    public Transform CustomSongParent;
    public GameObject CustomSongBoxPrefab;
    public GameObject CustomSongBoxPrefabParent;
    public GameObject StarPrefab;
    public List<GameObject> LoadedCustomSongsGO = new List<GameObject>();

    public void LoadCurrentlySavedCustomSongs()
    {
        foreach(GameObject gameObject in LoadedCustomSongsGO)
        {
            Destroy(gameObject);
        }
        LoadedCustomSongsGO.Clear();
        if(CustomSongParent.childCount > 0)
        {
            for (int i = 0; i < CustomSongParent.childCount; i++)
            {
                Transform child = CustomSongParent.GetChild(i);

                SongScript SS = child.GetComponent<SongScript>();
                GameObject GO = Instantiate(CustomSongBoxPrefab);
                GO.transform.SetParent(CustomSongBoxPrefabParent.transform, false);
                CustomSongBoxprefab customSong = GO.GetComponent<CustomSongBoxprefab>();
                customSong.boxImage.sprite = SS.ImageSprite;
                customSong.MusicTitleText.text = SS.SongName;
                customSong.RelatedSong = SS;
                for(int u = 0; u < SS.DifficultyOverride; u++)
                {
                    GameObject Star = Instantiate(StarPrefab);
                    Star.transform.SetParent(customSong.StarRatingParent.transform);
                }
                LoadedCustomSongsGO.Add(GO);
            }
        }
    }

    // This property provides global access to the instance
    public static MainMenuManager Instance
    {
        get
        {
            // If the instance doesn't exist, find it in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainMenuManager>();

                // If it still doesn't exist, create a new instance
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("MainMenuManager");
                    _instance = singletonObject.AddComponent<MainMenuManager>();
                }
            }

            return _instance;
        }
    }

    // Optional: Add any other methods or properties you need for your MainMenuManager

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
        LoadCurrentlySavedCustomSongs();
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
        LoadCurrentlySavedCustomSongs();
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
        StartCoroutine(GoToSongSelectionSequence());
    }

    public IEnumerator GoToSongSelectionSequence()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MainMenuUI.SetActive(false);
        MainGameSelectionScreen.SetActive(true);
        yield return new WaitForSeconds(BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();

    }

    public void StartDemoGame()
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
        StartCoroutine(MusicController.Instance.StartMatch());
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
