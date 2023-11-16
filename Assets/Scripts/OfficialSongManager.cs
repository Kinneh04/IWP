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
using GetPlayerProfileRequest = PlayFab.ClientModels.GetPlayerProfileRequest;
using PlayerProfileViewConstraints = PlayFab.ClientModels.PlayerProfileViewConstraints;
using GetPlayerProfileResult = PlayFab.ClientModels.GetPlayerProfileResult;

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
    public bool isFetchingLB;
    public Button RefreshButton;
    public TMP_Text TimeoutText;
    public float timeoutTimer;

    [Header("PersonalRecord")]
    public TMP_Text PR_RankText;
    public TMP_Text PR_AccText, PR_ScoreText;
    public GameObject hasPRGO, DoesntHavePRGO;

    [Header("Dont fill in")]

    public OfficialSongScript CurrentlySelectedSong;

    public void LoadPR()
    {
        if(CurrentlySelectedSong.LocalScore == null || CurrentlySelectedSong.LocalScore.LBScore == "0")
        {
            hasPRGO.SetActive(false);
            DoesntHavePRGO.SetActive(true);
        }
        else
        {
            hasPRGO.SetActive(true);
            DoesntHavePRGO.SetActive(false);

            PR_RankText.text = CurrentlySelectedSong.LocalScore.LBRanking;
            PR_AccText.text = "ACC: " + CurrentlySelectedSong.LocalScore.LBAccuracy + "%";
            PR_ScoreText.text = "SCORE: " + CurrentlySelectedSong.LocalScore.LBScore;
        }
    }
    public void RefreshCurrentLeaderboard()
    {
        GetOfficialSongScores();
    }
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
        if (isFetchingLB)
        {
            RefreshingText.gameObject.SetActive(true);
            RefreshingText.text = "Refreshing...";
            return;
        }
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            RefreshingText.text = "Log in to see leaderboards!";
            RefreshingText.gameObject.SetActive(true);
            return;
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
                        if (LBE.LBName == MusicController.Instance.LoggedInPlayerName) LBP.NameText.color = Color.green;
                        LBP.ScoreText.text = "SCORE: "+LBE.LBScore;
                        LBP.RankText.text = LBE.LBRanking;
                        LBP.AccText.text = LBE.LBAccuracy + "%";
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

    private void Update()
    {
        if(timeoutTimer > 0)
        {
            timeoutTimer -= Time.deltaTime;
            RefreshButton.interactable = false;
            TimeoutText.text = ((int)timeoutTimer).ToString();
            TimeoutText.gameObject.SetActive(true);
        }
        else
        {
            TimeoutText.gameObject.SetActive(false);
            RefreshButton.interactable = true;
        }
    }

    void OnDestroy()
    {
      //  SerializeAndStoreData();
    }
    public void GetOfficialSongScores()
    {
        isFetchingLB = true;
        timeoutTimer = 5.0f;
        PlayFabClientAPI.GetTitleData(new PlayFab.ClientModels.GetTitleDataRequest
            {
                Keys = new List<string> { "OfficialSongScores" }
            }, OnGetTitleDataSuccess, OnGetTitleDataFailure);

    }

    void OnGetTitleDataSuccess(PlayFab.ClientModels.GetTitleDataResult result)
    {
        isFetchingLB = false;
        if (result.Data.TryGetValue("OfficialSongScores", out string officialSongScores))
        {
            SongLeaderboardList = JsonConvert.DeserializeObject<List<OfficialSongLeaderboard>>(officialSongScores);
            if(CurrentlySelectedSong)
            {
                RefreshLeaderboardBasedOnNumber(CurrentlySelectedSong.SongID);
            }
        }
        else
        {
            Debug.Log("OfficialSongScores not found in title data.");
        }

    }

    public void AddNewLocalLeaderboard(string Rank, int score, float acc)
    {
        LeaderboardEntry newLBE = new LeaderboardEntry();
        newLBE.LBScore = score.ToString();
        newLBE.LBAccuracy = acc.ToString();
        newLBE.LBRanking = Rank;
        CurrentlySelectedSong.LocalScore = newLBE;
    }
    public void TryAddNewLeaderboard(string playerName, string Rank, int Score, float Acc)
    {
        foreach(OfficialSongLeaderboard leaderboard in SongLeaderboardList)
        {
            if (leaderboard.SongID == CurrentlySelectedSong.SongID)
            {
                foreach(LeaderboardEntry LBE in leaderboard.leaderboardEntries)
                {
                    if (LBE.LBName == playerName)
                    {
                        if (Score > int.Parse(LBE.LBScore))
                        {
                            LBE.LBScore = Score.ToSafeString();
                            LBE.LBRanking = Rank;
                            LBE.LBAccuracy = Acc.ToString();
                            leaderboard.leaderboardEntries.Sort((a, b) => a.LBScore.CompareTo(b.LBScore));
                            SerializeAndStoreData();
                        }

                        return;
                    }
                }
                LeaderboardEntry NewLBE = new LeaderboardEntry();
                NewLBE.LBName = playerName;
                NewLBE.LBScore = Score.ToString();
                NewLBE.LBRanking = Rank;
                NewLBE.LBAccuracy = Acc.ToString();
                leaderboard.leaderboardEntries.Add(NewLBE);
                Debug.Log("ADDED NEW LEADERBOARD ENTRY!!!");

                // Sort the list based on LBScore in ascending order
                leaderboard.leaderboardEntries.Sort((a, b) => a.LBScore.CompareTo(b.LBScore));
                SerializeAndStoreData();
                return;
            }
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
        isFetchingLB = false;

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
        musicController.EndBuffer = CurrentlySelectedSong.EndBufferTime;
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

        StopAllCoroutines();
        CurrentlySelectedSong = SS;
        StartCoroutine(TransitionToNewSong());
        PlayButton.interactable = true;
        SongSelectText.text = "Playing: " + SS.SongAudioClip.name;
        LoadPR();
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

