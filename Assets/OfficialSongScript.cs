using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficialSongScript : MonoBehaviour
{
    public AudioClip SongAudioClip;
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<SongEvent> Events = new List<SongEvent>();
    public int DifficultyOverride;

    [Header("optional")]
    public float PreviewStartTime;
}
