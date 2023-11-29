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

    private int SavedScore;
    float SavedAcc;
    string SavedRank;
    

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
        FinalScoreText.text = "SCORE: " + FinalScore.ToString() ;
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
        SavedScore = Score;
        SavedAcc = Accuracy;
        SavedRank = Grade;
        //Highscore
        NewHighScore.SetActive(true);
        TryAddNewPersonalRecord();
        if(PlayFabClientAPI.IsClientLoggedIn())
        {
            FetchPlayerNameForLeaderboardEntry(MusicController.Instance.LoggedInPlayerID);
        }
    }
    public void TryAddNewPersonalRecord()
    {
        if(string.IsNullOrEmpty(SongManager.CurrentlySelectedSong.LocalScore.LBName) || int.Parse(SongManager.CurrentlySelectedSong.LocalScore.LBScore) < SavedScore)
        {
            // Add new local leaderboard
            SongManager.AddNewLocalLeaderboard(SavedRank, SavedScore, SavedAcc);
        }
    }

}
