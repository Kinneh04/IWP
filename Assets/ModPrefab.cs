using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModPrefab : MonoBehaviour
{
    public TMP_Text ModTitle;
    public string ModName;
    public float MultiplierChange;
    public string HelpText;
    public ModController MC;

    public Animator ModButtonAnimator;
    public AnimationClip modSelectAnimClip, modDeselectAnimClip;

    bool isSelected = false;

    public virtual void SelectMod()
    {
        if(isSelected)
        {
            onDeselectMod();
        }
        else
        {
            onSelectMod();
        }
    }

    private void Start()
    {
        if(!MC)MC = GameObject.FindObjectOfType<ModController>();
        ModTitle.text = ModName;
    }

    public void OnClickHelpButton()
    {
        MC.OnClickHelpButton(HelpText);
    }

    public void onSelectMod()
    {
        isSelected = true;
        ModButtonAnimator.Play(modSelectAnimClip.name);
        MC.OnSelectMultiplierEffect(MultiplierChange);
        ModBaseEffects();
    }

    public void onDeselectMod()
    {
        isSelected = false;
        ModButtonAnimator.CrossFade(modDeselectAnimClip.name, 0.05f);
        MC.OnDeSelectMEffect(MultiplierChange);
        ModDeselectEffects();
    }

    public virtual void ModBaseEffects()
    {

    }

    public virtual void ModDeselectEffects()
    {
        
    }
}
