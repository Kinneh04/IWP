using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Animator MainMenuAnimator;
    public AnimationClip ShowSubbuttonsAnimationClip;
    public List<GameObject> GameobjectsToEnableForDemo = new List<GameObject>();
    public List<GameObject> GameobjectsToDisableForDemo = new List<GameObject>();
    public List<GameObject> DisabledGOsBeforeEntering = new List<GameObject>();
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

    [Header("OfficialSongs")]
    public OfficialSongManager officialSongManager;
    public GameObject PreviewSongMenu;

    [Header("Shop")]
    public GameObject ShopUI;
    public AudioSource ShopAudioSource;
    public StoreManager storeManager;

    [Header("ForMunninsTrial")]
    public GameObject OriginalDungeon;
    public List<GameObject> UIToEnableForMunninsTrial;
    public List<GameObject> UIToDisableForMunninsTrial;

    [Header("Settings")]
    public Slider musicSlider, sfxSlider;
    public TMP_Text musicAmountText, sfxAmountText;
    public AudioSource SFXAudioSource;

    [Header("Refs")]
    public ScoreManager SM;

    public void ChangeMusicVolume()
    {
        musicAmountText.text = musicSlider.value.ToString("f1");
        MainMenuAudioSource.volume = musicSlider.value;
        MusicController.Instance.MusicAudioSource.volume = musicSlider.value;
    }

    public void ChangeSFXVolume()
    {
        sfxAmountText.text = sfxSlider.value.ToString("f1");
        MusicController.Instance.SFXAudioSource.volume = sfxSlider.value;
    }

    public void StartMunninsTrial()
    {
        StartCoroutine(TransitionToMunninsTrial());
    }

    public IEnumerator TransitionToMunninsTrial()
    {
        BlackscreenController.Instance.FadeOut();
        officialSongManager.CurrentlySelectedSong = MunninsTrialManager.Instance.MunninsTrialSong;
        PreviewSongMenu.SetActive(false);
        StartCoroutine(FadeOutAudioSource(MainMenuAudioSource));
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        foreach (GameObject GO in GameobjectsToDisableForDemo)
        {
            if (!GO) continue;
            if (GO.activeSelf)
            {
                GO.SetActive(false);
                DisabledGOsBeforeEntering.Add(GO);
            }
        }
        storeManager.LoadWeaponDetails();
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            if (!GO) continue;

            GO.SetActive(true);
        }
        foreach(GameObject GO in UIToEnableForMunninsTrial)
        {
            GO.SetActive(true);
        }
        foreach (GameObject GO in UIToDisableForMunninsTrial)
        {
            GO.SetActive(false);
        }
        OriginalDungeon.SetActive(false);
        MusicController.Instance.isPlayingMunninsTrial = true;
        storeManager.LoadAbilityDetails();
        MunninsTrialManager.Instance.StartDungeon();
        officialSongManager.LoadSong();
        yield return new WaitForSeconds(0.75f);
        LoadingScreen.SetActive(false);
        yield return new WaitForSeconds(1);
        Cursor.lockState = CursorLockMode.Locked;
        BlackscreenController.Instance.FadeIn();
        StartCoroutine(MusicController.Instance.StartMatch());
    }
    public void ToShop()
    {
        StartCoroutine(TransitionToShop());
    }
    public void FromShopToMenu()
    {
        StartCoroutine(BackToMenuFromShop());
    }

    public IEnumerator TransitionToShop()
    {
        BlackscreenController.Instance.FadeOut();
        StartCoroutine(FadeOutAudioSource(MainMenuAudioSource));
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MainGameSelectionScreen.SetActive(false);
        ShopUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        //ShopAudioSource.Play();
        StartCoroutine(FadeInAudioSource(ShopAudioSource));
    }

    public IEnumerator BackToMenuFromShop()
    {
        BlackscreenController.Instance.FadeOut();
        StartCoroutine(FadeOutAudioSource(ShopAudioSource));

        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MainGameSelectionScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f); 
        ShopUI.SetActive(false);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        StartCoroutine(FadeInAudioSource(MainMenuAudioSource));
        ShopAudioSource.Stop();
    }
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
                customSong.startButton.onClick.AddListener(delegate { SEM.PreviewCustomSong(SS); });
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
        Application.targetFrameRate = 144;
    }
    public IEnumerator FadeOutAudioSource(AudioSource AS)
    {
        StopCoroutine(FadeOutAudioSource(AS));
        while(AS.volume > 0.05f)
        {
            yield return null;
            AS.volume = Mathf.Lerp(AS.volume, 0, 6 * Time.deltaTime);
        }
        AS.volume = 0;
        AS.Pause();
    }

    public IEnumerator FadeInAudioSource(AudioSource AS)
    {
        StopCoroutine(FadeOutAudioSource(AS));
        AS.volume = 0.0f;
        AS.Play();

        while (AS.volume < musicSlider.value)
        {
            yield return null;
            AS.volume += 0.035f;
        }
        AS.volume = musicSlider.value;


    }

    public void MakeNewCustomSongButton()
    {
        StartCoroutine(MakeNewCustomSongCoroutine());
    }
    public IEnumerator MakeNewCustomSongCoroutine()
    {
        StartCoroutine(FadeOutAudioSource(MainMenuAudioSource));
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(false);
        SongPickerScreen.SetActive(true);
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
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
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        EditorScreen.SetActive(true);
        SongPickerScreen.SetActive(false);
        LoadCurrentlySavedCustomSongs();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
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
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        LoadCurrentlySavedCustomSongs();
        EditorScreen.SetActive(true);
        MainMenuUI.SetActive(false);
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        
    }

    public IEnumerator ReturnToMainMenuSequence()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MainGameSelectionScreen.SetActive(false);
        EditorScreen.SetActive(false);
        MainMenuUI.SetActive(true);
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
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
      
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MainMenuUI.SetActive(false);
        MainGameSelectionScreen.SetActive(true);
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();

    }

    public void StartDemoGame(bool custom = false)
    {
        StartCoroutine(PlaydemoSequence(custom));
    }

    public void ReturnToMainMenuFromGame()
    {
        StartCoroutine(ReturnToMenuFromGameCoroutine());
    }

    public void RetryLevel()
    {
        StartCoroutine(RetryLevelCoroutine());
    }

    public IEnumerator ReturnToMenuFromGameCoroutine()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);

        foreach (GameObject GO in DisabledGOsBeforeEntering)
        {
            if (!GO) continue;
            GO.SetActive(true);
        }
        DisabledGOsBeforeEntering.Clear();
        MusicController.Instance.Cleanup();
        PlayerRatingController.Instance.Cleanup();
        FirstPersonController.Instance.Cleanup();
        //FirstPersonController.Instance.DeathScreen.SetActive(false);
        EnemySpawner.Instance.Cleanup();
        BossManager.Instance.Cleanup();
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            if (!GO) continue;
                GO.SetActive(false);

        }
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        //  StartCoroutine(MusicController.Instance.StartMatch());
    }

    public IEnumerator RetryLevelCoroutine()
    {
        BlackscreenController.Instance.FadeOut();
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        MusicController.Instance.Cleanup();
        EnemySpawner.Instance.Cleanup();
        FirstPersonController.Instance.Cleanup();
        BossManager.Instance.Cleanup();
        foreach (GameObject GO in DisabledGOsBeforeEntering)
        {
            if (!GO) continue;
            GO.SetActive(true);
        }
        DisabledGOsBeforeEntering.Clear();
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            if (!GO) continue;
            GO.SetActive(false);

        }
        foreach (GameObject GO in GameobjectsToDisableForDemo)
        {
            if (!GO) continue;
            if (GO.activeSelf)
            {
                GO.SetActive(false);
                DisabledGOsBeforeEntering.Add(GO);
            }
        }
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            if (!GO) continue;

            GO.SetActive(true);
        }
        FirstPersonController.Instance.DeathScreen.SetActive(false);
        PlayerRatingController.Instance.Cleanup();
        yield return new WaitForSeconds(0.3f);
        LoadingScreen.SetActive(false);
        BlackscreenController.Instance.FadeIn();
        yield return new WaitForSeconds(1);
        StartCoroutine(MusicController.Instance.StartMatch());
    }

    public IEnumerator PlaydemoSequence(bool customsong = false)
    {
        BlackscreenController.Instance.FadeOut();
        PreviewSongMenu.SetActive(false);
        StartCoroutine(FadeOutAudioSource(MainMenuAudioSource));
        yield return new WaitForSeconds(1 / BlackscreenController.Instance.fadeSpeed);
        LoadingScreen.SetActive(true);
        foreach (GameObject GO in GameobjectsToDisableForDemo)
        {
            if (!GO) continue;
            if (GO.activeSelf)
            {
                GO.SetActive(false);
                DisabledGOsBeforeEntering.Add(GO);
            }
        }
        storeManager.LoadWeaponDetails();
        SM.customSong = customsong;
        yield return new WaitForSeconds(0.5f);
   
        foreach (GameObject GO in GameobjectsToEnableForDemo)
        {
            if (!GO) continue; 

            GO.SetActive(true);
        }
        MusicController.Instance.PulsingLights.Clear();
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            MusicController.Instance.PulsingLights.Add(light);
        }
        storeManager.LoadAbilityDetails();
        if(!customsong) officialSongManager.LoadSong();
        yield return new WaitForSeconds(0.75f);
        LoadingScreen.SetActive(false);
        yield return new WaitForSeconds(1);
        Cursor.lockState = CursorLockMode.Locked;
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
