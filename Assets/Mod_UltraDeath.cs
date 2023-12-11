using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_UltraDeath : ModPrefab
{
    public override void ModBaseEffects()
    {
        FirstPersonController.Instance.maxHealth = 50;
        FirstPersonController.Instance.Health = 50;
        FirstPersonController.Instance.HealthSlider.maxValue = 50;
        base.ModBaseEffects();
    }

    public override void ModDeselectEffects()
    {
        FirstPersonController.Instance.maxHealth = 100;
        FirstPersonController.Instance.Health = 100;
        FirstPersonController.Instance.HealthSlider.maxValue = 100;
        base.ModDeselectEffects();
    }
}
