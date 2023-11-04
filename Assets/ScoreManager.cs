using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int FinalScore;
    public TMP_Text FinalScoreText;
    public GameObject FinalScoreGameObject;
    public TMP_Text FinalGrade, Kills, HighestCombo, Multikills, Accuracy;


    public void ChangeLevelCompleteVars(int Score, int Kills, int HighestCombo, int Multikills, float Accuracy)
    {
        
    }
}
