using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongEvent : MonoBehaviour
{
    public float castTimer;
    public bool Played = false;
    public virtual void CastEvent()
    {
        Debug.Log("Event going off!");
    }


}
