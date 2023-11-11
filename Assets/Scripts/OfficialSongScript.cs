using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficialSongScript : MonoBehaviour
{
    [Header("Necessities")]
    public int SongID;
    public string TitleOfSong;
    [Space(1)]
    public AudioClip SongAudioClip;
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<SongEvent> Events = new List<SongEvent>();
    public int DifficultyOverride;
    public bool ContainsBoss;
    
    [Header("optional")]
    public float PreviewStartTime;

    [Header("Leaderboard")]
    public OfficialSongLeaderboard songLeaderboard;

    [Header("For Debugging only!")]
    public float OfficialStartTime;
}


