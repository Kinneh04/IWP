using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using PlayFab.ServerModels;

public class OfficialSongManager : MonoBehaviour
{
    [Header("Fill in")]
    public MusicController musicController;
    public AudioSource MainMenuAudioSource;
    public EnemySpawner enemyManager;
    public Button PlayButton;
    public TMP_Text SongSelectText;
   
    public List<OfficialSongLeaderboard> SongLeaderboardList;

    [Header("Popupmenu")]
    public GameObject SelectedPopupMenu;
    public TMP_Text SongNameText, SongTimeText, SongBPMText, SongDifficultyText, SongBossText;


    [Header("LeaderboardThings")]
    public GameObject LeaderboardScoreprefab;
    public Transform LeaderboardPrefabParent;
    public List<GameObject> InstantiatedLeaderboardPrefabs = new List<GameObject>();
    public TMP_Text RefreshingText;

    [Header("Dont fill in")]

    public OfficialSongScript CurrentlySelectedSong;

    public void RefreshLeaderboardBasedOnNumber(int i)
    {
        if(InstantiatedLeaderboardPrefabs.Count > 0)
        {
            foreach(GameObject GO in InstantiatedLeaderboardPrefabs)
            {
                Destroy(GO);
            }
            InstantiatedLeaderboardPrefabs.Clear();
        }
        RefreshingText.gameObject.SetActive(true);
        RefreshingText.text = "Refreshing...";

        foreach(OfficialSongLeaderboard OSL in SongLeaderboardList)
        {
            if (OSL.SongID == i)
            {
                if (OSL.leaderboardEntries.Count > 0)
                {
                    foreach (LeaderboardEntry LBE in OSL.leaderboardEntries)
                    {
                        GameObject GO = Instantiate(LeaderboardScoreprefab);
                        LeaderboardPrefab LBP = GO.GetComponent<LeaderboardPrefab>();
                        LBP.NameText.text = LBE.LBName;
                        LBP.ScoreText.text = "SCORE: "+LBE.LBScore;
                        LBP.RankText.text = LBE.LBRanking;
                        LBP.AccText.text = LBE.LBAccuracy;
                        GO.transform.SetParent(LeaderboardPrefabParent, false);
                        InstantiatedLeaderboardPrefabs.Add(GO);

                    }
                    RefreshingText.gameObject.SetActive(false);
                }
                else
                {
                    RefreshingText.text = "LEADERBOARD EMPTY!";
                }
                return;
            }
        }
    }

    void OnDestroy()
    {
      //  SerializeAndStoreData();
    }
    void GetOfficialSongScores()
    {
        PlayFabClientAPI.GetTitleData(new PlayFab.ClientModels.GetTitleDataRequest
        {
            Keys = new List<string> { "OfficialSongScores" }
        }, OnGetTitleDataSuccess, OnGetTitleDataFailure);
    }

    void OnGetTitleDataSuccess(PlayFab.ClientModels.GetTitleDataResult result)
    {
        if (result.Data.TryGetValue("OfficialSongScores", out string officialSongScores))
        {
            SongLeaderboardList = JsonConvert.DeserializeObject<List<OfficialSongLeaderboard>>(officialSongScores);
        }
        else
        {
            Debug.Log("OfficialSongScores not found in title data.");
        }
    }
    void SerializeAndStoreData()
    {
        string serializedData = JsonConvert.SerializeObject(SongLeaderboardList, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        if (serializedData == "{}" || serializedData == "[]")
        {
            Debug.LogError("Failed To serialize Data! ");
            return;
        }
        Dictionary<string, string> titleData = new Dictionary<string, string>
        {
            { "OfficialSongData", serializedData }
        };

        PlayFabServerAPI.SetTitleData(new SetTitleDataRequest
        {
            Key = "OfficialSongScores",
            Value = serializedData
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
       // GetOfficialSongScores();
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
            RefreshLeaderboardBasedOnNumber(SS.SongID);
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

