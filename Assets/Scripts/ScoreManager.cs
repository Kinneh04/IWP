using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int FinalScore;
    public string Grade;
    public TMP_Text FinalScoreText;
    public GameObject FinalScoreGameObject;
    public TMP_Text FinalGradeText, KillsText, HighestComboText, MultikillsText, AccuracyText;
    public GameObject NewHighScore;

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

        //Highscore
        NewHighScore.SetActive(true);
    }
}
