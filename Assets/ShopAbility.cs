using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopAbility
{
    public string AbilityName, abilityDesc;
    public Sprite AbilitySprite;

    public bool bought, equipped;
    public int Cost;
    public GameObject AttachedAbility;
}
