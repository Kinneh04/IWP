using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_Autofire : ModPrefab
{
    public override void ModBaseEffects()
    {
        MusicController.Instance.autoFire = true;
        base.ModBaseEffects();
    }

    public override void ModDeselectEffects()
    {
        MusicController.Instance.autoFire = false;
        base.ModDeselectEffects();
    }
}
