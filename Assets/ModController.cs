using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModController : MonoBehaviour
{
    public TMP_Text HelpText;
    public GameObject HelpPopupBox;

    public TMP_Text MultiplierText;
    public float MultiplierScale = 1.0f;
    public float RealMultiplierScale = 1.0f;
    public void OnClickHelpButton(string s)
    {
        HelpText.text = s;
        HelpPopupBox.SetActive(true);
    }

    public void OnSelectMultiplierEffect(float MChange)
    {
        MultiplierScale += MChange;
        RealMultiplierScale = MultiplierScale;
        if (RealMultiplierScale < 0) RealMultiplierScale = 0;
        else if (RealMultiplierScale > 2) RealMultiplierScale = 2;
        MultiplierText.text = "x" + RealMultiplierScale.ToString("F2");
    }
    public void OnDeSelectMEffect(float MChange)
    { 
        MultiplierScale -= MChange;
        RealMultiplierScale = MultiplierScale;
        if (RealMultiplierScale < 0) RealMultiplierScale = 0;
        else if (RealMultiplierScale > 2) RealMultiplierScale = 2;
        MultiplierText.text = "x" + RealMultiplierScale.ToString("F2");
    }
}
