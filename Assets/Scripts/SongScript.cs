using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;

public class SongScript : MonoBehaviour
{
    [Header("Saving")]
    public bool LOCKEDIN = false;

    [Header("ImageData")]
    public string ImageData;
    public Sprite ImageSprite;

    [Header("SongData")]
    public string SongName;
    public byte[] audioData;
    public AudioClip currentSongAudioClip;

    [Header("Others")]
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<SongEvent> Events = new List<SongEvent>();
    public int DifficultyOverride;
}
