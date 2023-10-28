using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongScript : MonoBehaviour
{
    public List<Color> colors = new List<Color>();
    public int BPM;
    public List<Event> Events = new List<Event>();
}

[Serializable]
public class SongEvent
{

}
