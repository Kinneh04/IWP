using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSongManager : MonoBehaviour
{
    [Header("Fill in")]
    public MusicController musicController;
    public AudioSource MainMenuAudioSource;
    public EnemySpawner enemyManager;
    public Button PlayButton;
    public TMP_Text SongSelectText;


    [Header("Popupmenu")]
    public GameObject SelectedPopupMenu;
    public TMP_Text SongNameText, SongTimeText, SongBPMText, SongDifficultyText, SongBossText;
    public GameObject AllowForTutorialToggle;


    [Header("PersonalRecord")]
    public TMP_Text PR_RankText;
    public TMP_Text PR_AccText, PR_ScoreText;
    public GameObject hasPRGO, DoesntHavePRGO;

    [Header("Dont fill in")]
    public OfficialSongScript CurrentlySelectedSong;
    public void LoadPR()
    {
        if (CurrentlySelectedSong.LocalScore == null || CurrentlySelectedSong.LocalScore.LBScore == "0" || CurrentlySelectedSong.LocalScore.LBScore == "")
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
}
