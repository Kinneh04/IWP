using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Rendering;

public class SongEditorManager : MonoBehaviour
{
    public AudioSource CustomAudioSource;
    public AudioClip NewAudioClip = null;
    public TMP_Text ResultsText;
  
    public GameObject EditSoundtrackMenu;
    public GameObject ChooseSongMenu;

    [Header("CustomSongEventTimeline")]
    public SongScript CustomSong;

    [Header("Scrubber")]
    public Slider audioSlider;
    public TMP_Text currentTimeText;
    private float currentPlayheadTime = 0f;
    public TMP_Text EndTime;

    [Header("SongDetails")]
   
    public TMP_InputField BPMInput;
    public GameObject BPMWarning;
    public GameObject SongDetailWarning;
    public Image BGImage;

    [Header("ColourPalette")]
    public List<Image> ColorPaletteImages = new List<Image>();
    public GameObject ColorPalette;
    public Image CurrentlySelectedImage;
    public Image PreviewImage;

    [Header("MapDetails")]
    public TMP_InputField MapNameInputField;

    public Slider DifficultyOverrideSlider;
    public TMP_Text DifficultyInt;

    [Header("EnableDrums")]
    public bool drumsEnabled;
    public Toggle enableDrumsToggle;

    [Header("Events")]
    public Transform SliderHandleTransform;
    public List<EventMarker> eventMarkers = new List<EventMarker>();
    public GameObject EventMarkerPrefab;
    public Transform eventMarkerPrefabParent;
    public GameObject EventMarkerWindow;
    public EventMarker CurrentlySelectedMarker;
    public Button AddEventButton;
    public TMP_Dropdown EventTypeDropdown;
    public int maxEvents;
    public TMP_Text EventsLimitText;
    bool maxEventsReached = false;

    [Header("EventMenus")]
    public GameObject BPMChangerMenu;
    public TMP_InputField BPMChangeInput;
    public GameObject Warning;

    public GameObject DifficultyChangerMenu;
    public TMP_Text CurrentDifficultyText;
    public Slider ChangeDifficultySlider;

    public TMP_Dropdown BossDropdown;
    public GameObject BossSpawnMenu;

    [Header("SaveAndPlay")]
    public Button SaveButton, PlayButton;
    public GameObject SavingBlocker;
    public TMP_Text SavingText;
    public GameObject OkButton;
    public GameObject CustomSongPrefab;
    public GameObject CustomSongPrefabParent;

    [Header("Exit")]
    public GameObject ExitConfirmationGO;

    public void RenameMapName()
    {
        CustomSong.SongName = MapNameInputField.text;
    }

    public void BackOutButton()
    {
        if(!CustomSong || CustomSong.LOCKEDIN)
        {
            MainMenuManager.Instance.ExitSongpicker();
        }
        else
        {
            ExitConfirmationGO.SetActive(true);
        }
    }

    public void DiscardSong()
    {
        Destroy(CustomSong.gameObject);
        MainMenuManager.Instance.ExitSongpicker();
    }
    public IEnumerator SaveSongCoroutine()
    {
        OkButton.SetActive(false);
        SavingBlocker.SetActive(true);
        SavingText.text = "Saving level. This may take some time...";
        MainMenuManager.Instance.LoadingScreen.SetActive(true);
        yield return new WaitForSeconds(1);
        try
        {
            //Saving custom level code here
            CompileEvents();
            SavingText.text = "Level Saved successfully!";
            //End
        }
        catch(Exception e)
        {
            SavingText.text = "Error with saving level. \n Error log: \n" + e.ToString();
        }

        MainMenuManager.Instance.LoadingScreen.SetActive(false);
        OkButton.SetActive(true);
    }

    public void CompileEvents()
    {
        drumsEnabled = enableDrumsToggle.isOn;
        CustomSong.LOCKEDIN = true;
        CustomSong.Events.Clear();
        foreach(EventMarker EM in eventMarkers)
        {
            if(EM.currentEvent)
            {
                SongEvent newSE = new SongEvent();
                newSE = EM.currentEvent;
                CustomSong.Events.Add(newSE);
            }
        }
    }

    public void SaveSong()
    {
        StartCoroutine(SaveSongCoroutine());
    }

    public void ChangeBossTypeForEvent()
    {
        SpawnBossSongEvent SBSE = ((SpawnBossSongEvent)CurrentlySelectedMarker.currentEvent);
        SBSE.bossName = BossDropdown.options[BossDropdown.value].text;
        }

    public void RidAllEventSubmenus()
    {
        DifficultyChangerMenu.SetActive(false);
        BPMChangerMenu.SetActive(false);
        BossSpawnMenu.SetActive(false);
    }

    public enum EventTypes
    {
        Nothing, BPMChange, DifficultyChange, BossSpawn
    }

    public void SelectMarker(EventMarker EM)
    {
        CurrentlySelectedMarker = EM;
        EventMarkerWindow.SetActive(true);
        EventTypeDropdown.value = EM.value;
        DisplayAppropriateEventMenu(EM.value);
    }

    public void DisplayAppropriateEventMenu(int v)
    {
        RidAllEventSubmenus();
        switch (v)
        {
            case 0:
                break;
            case 1:
                BPMChangerMenu.SetActive(true);
                BPMChangeInput.text = ((BPMChangeSongEvent)CurrentlySelectedMarker.currentEvent).newBPM.ToString();
                break;
            case 2:
                DifficultyChangerMenu.SetActive(true);
                DifficultyChangeSongEvent DCSM = ((DifficultyChangeSongEvent)CurrentlySelectedMarker.currentEvent);
                CurrentDifficultyText.text = DCSM.newDifficulty.ToString();
                ChangeDifficultySlider.value = DCSM.newDifficulty;
                break;
            case 3:
                break;
            default:
                return;
        }
        EventTypeDropdown.value = v;
    }

    public void RemoveMarkerEvent()
    {
       if(CurrentlySelectedMarker.currentEvent)Destroy(CurrentlySelectedMarker.currentEvent);
    }
    public void ResetDefaultEventMenu()
    {
        EventTypeDropdown.value = 0;
        RidAllEventSubmenus();
    }

    public void EnterNewBPMForBPMChangeEvent()
    {
        int newBPM = int.Parse(BPMChangeInput.text);
        if (newBPM < 1 || newBPM > 240) Warning.SetActive(true);
        else
        {
            Warning.SetActive(false);
            BPMChangeSongEvent BPMChange = ((BPMChangeSongEvent)CurrentlySelectedMarker.currentEvent);
            BPMChange.newBPM = newBPM;
        }
    }

    public void EnterNewDifficultyForEvent()
    {
        DifficultyChangeSongEvent DCSM = ((DifficultyChangeSongEvent)CurrentlySelectedMarker.currentEvent);
        DCSM.newDifficulty = (int)ChangeDifficultySlider.value;
        CurrentDifficultyText.text = DCSM.newDifficulty.ToString();
    }

    public void ChangeEventType()
    {
        int v = EventTypeDropdown.value;
        RemoveMarkerEvent();
        RidAllEventSubmenus();
        switch (v)
        {
            case 0:
                CurrentlySelectedMarker.eventType = EventTypes.Nothing;
                CurrentlySelectedMarker.EventIndicator.text = "N";
                CurrentlySelectedMarker.currentEvent = null;

                break;
            case 1:
                CurrentlySelectedMarker.eventType = EventTypes.BPMChange;
                CurrentlySelectedMarker.EventIndicator.text = "C";
                CurrentlySelectedMarker.currentEvent = CustomSong.gameObject.AddComponent<BPMChangeSongEvent>();
                
                BPMChangerMenu.SetActive(true);
                break;
            case 2:
                CurrentlySelectedMarker.eventType = EventTypes.DifficultyChange;
                CurrentlySelectedMarker.EventIndicator.text = "D";
                CurrentlySelectedMarker.currentEvent = CustomSong.gameObject.AddComponent<DifficultyChangeSongEvent>();
                DifficultyChangerMenu.SetActive(true);
                break;
            case 3:
                CurrentlySelectedMarker.eventType = EventTypes.BossSpawn;
                CurrentlySelectedMarker.EventIndicator.text = "B";
                CurrentlySelectedMarker.currentEvent = CustomSong.gameObject.AddComponent<SpawnBossSongEvent>();
                BossSpawnMenu.SetActive(true);
                break;
            default:
                return;
        }
        if(CurrentlySelectedMarker.currentEvent) CurrentlySelectedMarker.currentEvent.castTimer = CurrentlySelectedMarker.activateAtTime;
        CurrentlySelectedMarker.value = v;
    }

    public void DeleteEvent()
    {
        RemoveMarkerEvent();
        eventMarkers.Remove(CurrentlySelectedMarker);
        Destroy(CurrentlySelectedMarker.gameObject);
        CurrentlySelectedMarker = null;
        EventMarkerWindow.SetActive(false);
        RecalculateEvents();
    }
    public void AddEventInTime()
    {
        if (maxEventsReached) return;
        GameObject GO = Instantiate(EventMarkerPrefab, SliderHandleTransform.position, Quaternion.identity);
        GO.transform.SetParent(eventMarkerPrefabParent);
        EventMarkerWindow.SetActive(true);
        EventMarker EM = GO.GetComponent<EventMarker>();
        EM.button.onClick.AddListener(delegate { SelectMarker(EM); });
        EM.SEM = this;
        EM.TimeText.text = FormatTime(CustomAudioSource.time);
        EM.activateAtTime = CustomAudioSource.time;
        GO.transform.localScale = new Vector3(1, 1, 1);
        CurrentlySelectedMarker = EM;
        foreach(EventMarker RecordedEM in eventMarkers)
        {
            if(RecordedEM.activateAtTime == EM.activateAtTime)
            {
                Vector3 newPos = RecordedEM.transform.position;
                newPos.y += 100f;
                EM.transform.position = newPos;
            }
        }
        EM.value = 0;
        eventMarkers.Add(EM);
        RecalculateEvents();
        DisplayAppropriateEventMenu(EM.value);
    }

    public void RecalculateEvents()
    {
        int currentevents = eventMarkers.Count;
        EventsLimitText.text = currentevents.ToString() + "/" + maxEvents.ToString();
        if (currentevents >= maxEvents)
        {
            maxEventsReached = true;
            EventsLimitText.color = Color.red;
        }
        else
        {
            maxEventsReached = false;
            EventsLimitText.color = Color.white;
        }
    }

    public void OnChangeDifficultySlider()
    {
        DifficultyInt.text = DifficultyOverrideSlider.value.ToString();
        CustomSong.DifficultyOverride = (int)DifficultyOverrideSlider.value;
    }
    public void OpenColorPalette(Image i)
    {
        ColorPalette.SetActive(true);
        CurrentlySelectedImage = i;
    }

    public void ChangeSongColors()
    {
        for(int i = 0; i < ColorPaletteImages.Count; i++)
        {
            CustomSong.colors[i] = ColorPaletteImages[i].color;
        }
    }

    public void AcceptColorPalette()
    {
        ColorPalette.SetActive(false);
        CurrentlySelectedImage.color = PreviewImage.color;
        CurrentlySelectedImage = null;
        ChangeSongColors();
    }


    public void SetBPM()
    {
        int bpm = int.Parse(BPMInput.text);
        if (bpm > 240 || bpm < 1)
        {
            BPMWarning.SetActive(true);
            SongDetailWarning.SetActive(true);
        }
        else
        {
            BPMWarning.SetActive(false);
            CustomSong.BPM = int.Parse(BPMInput.text);
            SongDetailWarning.SetActive(false);
        }
    }
    void Update()
    {
        if (!CustomAudioSource.clip) return;
        if (CustomAudioSource.isPlaying || !CustomAudioSource.gameObject.activeInHierarchy)
        {
            audioSlider.value = CustomAudioSource.time;
            audioSlider.maxValue = CustomAudioSource.clip.length;
            // Update the current time text (optional)
        
        }
        currentTimeText.text = FormatTime(CustomAudioSource.time);
        EndTime.text = FormatTime(CustomAudioSource.clip.length);
        // Update the hidden float
        currentPlayheadTime = CustomAudioSource.time;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle between playing and pausing the audio
            if (CustomAudioSource.isPlaying)
            {
                CustomAudioSource.Pause();
                PlayButton.interactable = true;
                SaveButton.interactable = true;
            }
            else
            {
                CustomAudioSource.Play();
                PlayButton.interactable = false;
                SaveButton.interactable = false;
            }
        }
        UpdateSliderValue();
    }
    private void UpdateSliderValue()
    {
        // Update the slider value based on the audio playback time
        if (CustomAudioSource.isPlaying)
        {
            AddEventButton.interactable = false;
            audioSlider.value = CustomAudioSource.time;
            currentPlayheadTime = CustomAudioSource.time;
        }
        else AddEventButton.interactable = true;
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Cleanup()
    {
        NewAudioClip = null;
        EditSoundtrackMenu.SetActive(false);
        ChooseSongMenu.SetActive(true);
    }

    public void OnSliderValueChanged()
    {
        // When the slider value changes, set the audio playback time
        CustomAudioSource.time = audioSlider.value;

        // Update the hidden float
        currentPlayheadTime = audioSlider.value;
    }
    public void ChooseSong()
    {
        var bp = new BrowserProperties();
      //  bp.filter = "Audio files(*.mp3)";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            StartCoroutine(LoadAudioPath(path));
        });
    }

    public void ChooseImage()
    {
        var bp = new BrowserProperties();
        //  bp.filter = "Audio files(*.mp3)";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            StartCoroutine(LoadImagePath(path));
        });
    }
      public IEnumerator LoadImagePath(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error retrieving file! " + uwr.error);
                ResultsText.text = uwr.error;
            }
            else
            {
                var texture2D = DownloadHandlerTexture.GetContent(uwr);
                Debug.Log("Loaded " + texture2D.name);
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);
                BGImage.sprite = sprite;
                CustomSong.ImageSprite = sprite;
                CustomSong.ImageData = ConvertTexture2DToData(texture2D);
            }
        }
    }

    public string ConvertTexture2DToData(Texture2D texture)
    {
        // Encode Texture2D to byte array
        byte[] bytes = texture.EncodeToPNG();

        // Convert byte array to Base64 string
        string encodedString = System.Convert.ToBase64String(bytes);
        return encodedString;
    }

    public Sprite ConvertDataToSprite(string s)
    {
        // Retrieve the string later
        string retrievedString = s;

        // Convert Base64 string back to byte array
        byte[] retrievedBytes = System.Convert.FromBase64String(retrievedString);

        // Create a new Texture2D and load the byte array
        Texture2D retrievedTexture = new Texture2D(2, 2);
        retrievedTexture.LoadImage(retrievedBytes);

        // Create a new Sprite using the retrieved Texture2D
        Sprite retrievedSprite = Sprite.Create(retrievedTexture, new Rect(0, 0, retrievedTexture.width, retrievedTexture.height), Vector2.one * 0.5f);
        return retrievedSprite;
    }

    public IEnumerator LoadAudioPath(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();
            if(uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error retrieving file! " + uwr.error );
                ResultsText.text = uwr.error;
            }
            else
            {
                var SelectedAudioSource = DownloadHandlerAudioClip.GetContent(uwr);
                Debug.Log("Loaded " + SelectedAudioSource.name);
                NewAudioClip = SelectedAudioSource;
                NewAudioClip.name = SelectedAudioSource.name;
                CustomAudioSource.clip = NewAudioClip;
                ChooseSongMenu.SetActive(false);
                GameObject GO = Instantiate(CustomSongPrefab);
                GO.transform.SetParent(CustomSongPrefabParent.transform);
                CustomSong = GO.GetComponent<SongScript>();
                EditSoundtrackMenu.SetActive(true);
                //CustomSong.audioData = WavUtility.FromAudioClip(SelectedAudioSource);

                CustomSong.currentSongAudioClip = SelectedAudioSource;
            }
        }
    }
}
