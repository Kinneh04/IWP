using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
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
    public LeaderboardEntry LocalScore;
    [Header("optional")]
    public float PreviewStartTime;
    public float EndBufferTime;

    //[Header("Leaderboard")]
    //public OfficialSongLeaderboard songLeaderboard;

    [Header("For Debugging only!")]
    public float OfficialStartTime;

    private void OnDestroy()
    {
        string s = JsonConvert.SerializeObject(LocalScore);
        PlayerPrefs.SetString(SongID.ToString(), s);
    }

    private void Awake()
    {   
        string s = PlayerPrefs.GetString(SongID.ToString());
        LocalScore = JsonConvert.DeserializeObject<LeaderboardEntry>(s);
    }
}


