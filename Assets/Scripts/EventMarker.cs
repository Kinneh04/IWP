using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
public class EventMarker : MonoBehaviour
{
    public SongEditorManager SEM;
    public Button button;
    public TMP_Text TimeText;
    public SongEditorManager.EventTypes eventType;
    public TMP_Text EventIndicator;
    public float activateAtTime;
}
