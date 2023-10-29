using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMChangeSongEvent : SongEvent
{
    public MusicController musicController;
    public float newBPM;
    public override void CastEvent()
    {
        if(musicController == null)
        {
            musicController = GameObject.FindObjectOfType<MusicController>();
        }
        musicController.BPM_Divider = newBPM;
        base.CastEvent();
    }
}
