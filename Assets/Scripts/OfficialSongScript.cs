using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficialSongScript : MonoBehaviour
{
    public string TitleOfSong;
    public AudioClip SongAudioClip;
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<SongEvent> Events = new List<SongEvent>();
    public int DifficultyOverride;
    public bool ContainsBoss;
    
    [Header("optional")]
    public float PreviewStartTime;

    [Header("Leaderboard")]
    public List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

    [Header("For Debugging only!")]
    public float OfficialStartTime;
}


[System.Serializable]
public class LeaderboardEntry
{
    public string LBName;
    public string LBAccuracy;
    public string LBRanking;
    public string LBScore;
}

