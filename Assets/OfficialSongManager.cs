using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OfficialSongManager : MonoBehaviour
{
    [Header("Fill in")]
    public MusicController musicController;
    public AudioSource MainMenuAudioSource;
    public EnemySpawner enemyManager;
    public Button PlayButton;

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
        
    }

    public void PreviewSong(OfficialSongScript SS)
    {
        if (CurrentlySelectedSong != SS)
        {
            CurrentlySelectedSong = SS;
            StartCoroutine(TransitionToNewSong());
            PlayButton.interactable = true;
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
