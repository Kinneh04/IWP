using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class OfficialSongManager : MonoBehaviour
{
    [Header("Fill in")]
    public MusicController musicController;
    public AudioSource MainMenuAudioSource;
    public EnemySpawner enemyManager;
    public Button PlayButton;
    public TMP_Text SongSelectText;

    [Header("Dont fill in")]
    public OfficialSongScript CurrentlySelectedSong;

    private void Start()
    {
        PlayButton.interactable = false;
    }
    public void LoadSong()
    {
        musicController.BPM_Divider = CurrentlySelectedSong.BPM;
        musicController.MusicAudioSource.clip = CurrentlySelectedSong.SongAudioClip;
        musicController.LightColorPalette.Clear();
        foreach(Color c in CurrentlySelectedSong.colors)
        {
            musicController.LightColorPalette.Add(c);
        }
        musicController.LoadNewEventsFromOfficialSong(CurrentlySelectedSong);
    }

    public void PreviewSong(OfficialSongScript SS)
    {
     
        if (CurrentlySelectedSong != SS)
        {
            StopAllCoroutines();
            CurrentlySelectedSong = SS;
            StartCoroutine(TransitionToNewSong());
            PlayButton.interactable = true;
            SongSelectText.text = "Selected: " + SS.TitleOfSong;
        }
    }

    public IEnumerator TransitionToNewSong()
    {
        while(MainMenuAudioSource.volume > 0)
        {
            MainMenuAudioSource.volume -= Time.deltaTime;
            yield return null;
        }
        MainMenuAudioSource.clip = CurrentlySelectedSong.SongAudioClip;
        MainMenuAudioSource.time = CurrentlySelectedSong.PreviewStartTime;
        MainMenuAudioSource.Play();
        while(MainMenuAudioSource.volume < 1)
        {
            MainMenuAudioSource.volume += Time.deltaTime;
            yield return null;
        }
    }
}