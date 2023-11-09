using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
public class OfficialSongManager : MonoBehaviour
{
    [Header("Fill in")]
    public MusicController musicController;
    public AudioSource MainMenuAudioSource;
    public EnemySpawner enemyManager;
    public Button PlayButton;
    public TMP_Text SongSelectText;
    public List<OfficialSongScript> OfficialSongsList = new List<OfficialSongScript>();

    [Header("Popupmenu")]
    public GameObject SelectedPopupMenu;
    public TMP_Text SongNameText, SongTimeText, SongBPMText, SongDifficultyText, SongBossText;
    public GameObject LeaderboardScoreprefab;
    public Transform LeaderboardPrefabParent;

    [Header("Dont fill in")]
    public OfficialSongScript CurrentlySelectedSong;
    void GetOfficialSongScores()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
        {
            Keys = new List<string> { "OfficialSongScores" }
        }, OnGetTitleDataSuccess, OnGetTitleDataFailure);
    }

    void OnGetTitleDataSuccess(GetTitleDataResult result)
    {
        if (result.Data.TryGetValue("OfficialSongScores", out string officialSongScores))
        {
            // You have successfully retrieved the data. Use 'officialSongScores' for further processing.
        }
        else
        {
            Debug.Log("OfficialSongScores not found in title data.");
        }
    }
    void SerializeAndStoreData()
    {
        string serializedData = JsonConvert.SerializeObject(OfficialSongsList);

        Dictionary<string, string> titleData = new Dictionary<string, string>
        {
            { "OfficialSongData", serializedData }
        };

        PlayFabClientAPI.SetTitleData(new SetTitleDataRequest
        {
            Data = titleData
        }, OnSetTitleDataSuccess, OnSetTitleDataFailure);
    }

    void OnSetTitleDataSuccess(SetTitleDataResult result)
    {
        Debug.Log("Data successfully stored in title data.");
    }

    void OnSetTitleDataFailure(PlayFabError error)
    {
        Debug.LogError("Error storing data in title data: " + error.GenerateErrorReport());
    }
    void OnGetTitleDataFailure(PlayFabError error)
    {
        Debug.LogError("Error retrieving title data: " + error.GenerateErrorReport());
    }

    private void Start()
    {
        PlayButton.interactable = false;
        GetOfficialSongScores();
    }
    public void LoadSong()
    {
        musicController.BPM_Divider = CurrentlySelectedSong.BPM;
        musicController.MusicAudioSource.clip = CurrentlySelectedSong.SongAudioClip;
        musicController.LightColorPalette.Clear();
        foreach (Color c in CurrentlySelectedSong.colors)
        {
            musicController.LightColorPalette.Add(c);
        }
        musicController.LoadNewEventsFromOfficialSong(CurrentlySelectedSong);
        musicController.MusicAudioSource.time = CurrentlySelectedSong.OfficialStartTime;
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

            SelectedPopupMenu.SetActive(true);
            SongNameText.text = SS.TitleOfSong;
            int length = (int)SS.SongAudioClip.length;
            int minutes = length / 60;
            int seconds = length % 60;

            string formattedTime = string.Format("{0}:{1:00}", minutes, seconds);

            SongTimeText.text = "TIME: " + formattedTime;
            SongBPMText.text = "BPM: " + SS.BPM.ToString();
            SongDifficultyText.text = "DIFFICULTY: " + SS.DifficultyOverride.ToString() + "/10";
            if (SS.ContainsBoss)
            {
                SongBossText.text = "CONTAINS BOSS";
            }
            else SongBossText.text = "";

        }
    }

    public IEnumerator TransitionToNewSong()
    {
        while (MainMenuAudioSource.volume > 0)
        {
            MainMenuAudioSource.volume -= Time.deltaTime;
            yield return null;
        }
        MainMenuAudioSource.clip = CurrentlySelectedSong.SongAudioClip;
        MainMenuAudioSource.time = CurrentlySelectedSong.PreviewStartTime;
        MainMenuAudioSource.Play();
        while (MainMenuAudioSource.volume < 1)
        {
            MainMenuAudioSource.volume += Time.deltaTime;
            yield return null;
        }
    }
}

