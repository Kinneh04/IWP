using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_Ez : ModPrefab
{
    public override void ModBaseEffects()
    {
        FirstPersonController.Instance.maxHealth = 50;
        base.ModBaseEffects();
    }

    public override void ModDeselectEffects()
    {
        FirstPersonController.Instance.maxHealth = 100;
        base.ModDeselectEffects();
    }
}
