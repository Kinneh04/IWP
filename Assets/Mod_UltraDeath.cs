using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_UltraDeath : ModPrefab
{
    public FirstPersonController FPC;
    public override void ModBaseEffects()
    {
        
        FPC.maxHealth = 50;
        FPC.Health = 50;
        FPC.HealthSlider.maxValue = 50;
        base.ModBaseEffects();
    }

    public override void ModDeselectEffects()
    {
        FPC.maxHealth = 100;
        FPC.Health = 100;
        FPC.HealthSlider.maxValue = 100;
        base.ModDeselectEffects();
    }
}
