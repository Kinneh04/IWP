using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Xml;

public class SongEditorManager : MonoBehaviour
{
    public AudioSource CustomAudioSource;
    public AudioClip NewAudioClip = null;
    public TMP_Text ResultsText;
  
    public GameObject EditSoundtrackMenu;
    public GameObject ChooseSongMenu;

    [Header("CustomSongEventTimeline")]
    public SongScript CustomSong = new SongScript();

    [Header("Scrubber")]
    public Slider audioSlider;
    public TMP_Text currentTimeText;
    private float currentPlayheadTime = 0f;
    public TMP_Text EndTime;

    [Header("SongDetails")]
    public TMP_InputField BPMInput;
    public GameObject BPMWarning;
    public GameObject SongDetailWarning;

    [Header("ColourPalette")]
    public List<Image> ColorPaletteImages = new List<Image>();
    public GameObject ColorPalette;
    public Image CurrentlySelectedImage;
    public Image PreviewImage;

    [Header("MapDetails")]
    public Slider DifficultyOverrideSlider;
    public TMP_Text DifficultyInt;

    [Header("EnableDrums")]
    public AudioClip Drums;
      public AudioClip Snare;

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
    public enum EventTypes
    {
        Nothing, BPMChange, DifficultyChange, EnemySpawn, BossSpawn
    }

    public void SelectMarker(EventMarker EM)
    {
        CurrentlySelectedMarker = EM;
        EventMarkerWindow.SetActive(true);
    }

    public void ChangeEventType()
    {
        int v = EventTypeDropdown.value;
        switch(v)
        {
            case 0:
                CurrentlySelectedMarker.eventType = EventTypes.Nothing;
                CurrentlySelectedMarker.EventIndicator.text = "N";
                break;
            case 1:
                CurrentlySelectedMarker.eventType = EventTypes.BPMChange;
                CurrentlySelectedMarker.EventIndicator.text = "C";
                break;
            case 2:
                CurrentlySelectedMarker.eventType = EventTypes.DifficultyChange;
                CurrentlySelectedMarker.EventIndicator.text = "D";
                break;
            case 3:
                CurrentlySelectedMarker.eventType = EventTypes.EnemySpawn;
                CurrentlySelectedMarker.EventIndicator.text = "E";
                break;
            case 4:
                CurrentlySelectedMarker.eventType = EventTypes.BossSpawn;
                CurrentlySelectedMarker.EventIndicator.text = "B";
                break;
            default:
                return;
        }
    }

    public void DeleteEvent()
    {
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
       
        CurrentlySelectedMarker = EM;
        foreach(EventMarker RecordedEM in eventMarkers)
        {
            if(RecordedEM.activateAtTime == EM.activateAtTime)
            {
                Vector3 newPos = RecordedEM.transform.position;
                newPos.y += 80f;
                EM.transform.position = newPos;
            }
        }
        eventMarkers.Add(EM);
        RecalculateEvents();
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
                CustomAudioSource.Pause();
            else
                CustomAudioSource.Play();
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
                EditSoundtrackMenu.SetActive(true);
            }
        }
    }
}
