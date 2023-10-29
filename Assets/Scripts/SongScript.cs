using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;

public class SongScript : MonoBehaviour
{
    public bool LOCKEDIN = false;
    public string SongName;
    public byte[] audioData;
    public AudioClip currentSongAudioClip;
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<SongEvent> Events = new List<SongEvent>();
    public int DifficultyOverride;
}
