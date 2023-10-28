using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
public class SongEditorManager : MonoBehaviour
{
    public AudioSource CustomAudioSource;
    public AudioClip NewAudioClip = null;
    public TMP_Text ResultsText;
  
    public GameObject EditSoundtrackMenu;
    public GameObject ChooseSongMenu;

    [Header("Scrubber")]
    public Slider audioSlider;
    public TMP_Text currentTimeText;
    private float currentPlayheadTime = 0f;
    public TMP_Text EndTime;
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
            audioSlider.value = CustomAudioSource.time;
            currentPlayheadTime = CustomAudioSource.time;
        }
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