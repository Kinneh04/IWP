using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public int FinalScore;
    public string Grade;
    public TMP_Text FinalScoreText;
    public GameObject FinalScoreGameObject;
    public TMP_Text FinalGradeText, KillsText, HighestComboText, MultikillsText, AccuracyText;
    public GameObject NewHighScore;
    public GameObject LoginToSaveScoreGO;
    [Header("SongManager")]
    public OfficialSongManager SongManager;
    public SongEditorManager customSongManager;
    public bool customSong = false;
    private int SavedScore;
    float SavedAcc;
    string SavedRank;
    public ModController MC;
    public GameObject NuhUhGameobject;
    public AudioClip ThumpAC;
    

    public void FetchPlayerNameForLeaderboardEntry(string playFabId)
    {
        var request = new GetPlayerProfileRequest
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileFailure);
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        string playerName = result.PlayerProfile.DisplayName;
      //  Debug.Log("Player Name: " + playerName);

        SongManager.TryAddNewLeaderboard(playerName, SavedRank, SavedScore, SavedAcc);
    }

    private void OnGetPlayerProfileFailure(PlayFabError error)
    {
       // Debug.LogError("Error getting player profile: " + error.ErrorMessage);
    }
    public void ChangeLevelCompleteVars(int Score, int Kills, int HighestCombo, int Multikills, float Accuracy, bool bosskilled = false)
    {


        FinalScore = Score;
        KillsText.text = Kills.ToString();
        HighestComboText.text = HighestCombo.ToString();
        MultikillsText.text = Multikills.ToString();
        AccuracyText.text = Accuracy.ToString("F2") + "%";
        FinalScoreText.text = "SCORE: " + (FinalScore * MC.RealMultiplierScale) .ToString() ;
        if (bosskilled)
        {
            FinalGradeText.text = "B";
            Grade = "B";
            FinalGradeText.color = Color.cyan;
        }
        else
        {
            FinalGradeText.text = "P";
            Grade = "P";
            FinalGradeText.color = Color.yellow;
        }
        SavedScore = (int)(Score * MC.RealMultiplierScale);
        SavedAcc = Accuracy;
        SavedRank = Grade;
        //Highscore
        NewHighScore.SetActive(true);

        if (MC.MultiplierScale > 0)
        {
            if (!customSong) TryAddNewPersonalRecord();
            else TryAddNewPersonalRecordToCustomSong();
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                FetchPlayerNameForLeaderboardEntry(MusicController.Instance.LoggedInPlayerID);
            }
        }
        else
        {
            NuhUhGameobject.SetActive(true);
            MusicController.Instance.SFXAudioSource.PlayOneShot(ThumpAC);
        }
    }
    public void TryAddNewPersonalRecord()
    {
        if (MC.MultiplierScale <= 0)
        {
            NuhUhGameobject.SetActive(true);
        }
        else
        {
            if (SongManager.CurrentlySelectedSong.LocalScore == null || int.Parse(SongManager.CurrentlySelectedSong.LocalScore.LBScore) < SavedScore)
            {
                // Add new local leaderboard
                SongManager.AddNewLocalLeaderboard(SavedRank, SavedScore, SavedAcc);
            }
        }
    }
    public void TryAddNewPersonalRecordToCustomSong()
    {
        if (MC.MultiplierScale <= 0)
        {
            NuhUhGameobject.SetActive(true);
        }
        else
        {
            if (customSongManager.CurrentlySelectedSong.LocalScore == null || int.Parse(customSongManager.CurrentlySelectedSong.LocalScore.LBScore) < SavedScore)
            {
                // Add new local leaderboard
                customSongManager.AddNewLocalLeaderboard(SavedRank, SavedScore, SavedAcc);
            }
        }
    }
}
