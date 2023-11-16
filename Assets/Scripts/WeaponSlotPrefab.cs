using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponSlotPrefab : MonoBehaviour
{
    public TMP_Text WeaponTitleText, WeaponDescText, WeaponDetailsText, EquippedText, CostText;
    public Image EquippedBackgroundImage;
    public Image IconImage;
    public Button EquipButton, BuyButton;
    public GameObject LockScreen;

    [Header("DontFill")]
    public int Cost;
    public bool isPurchased, isEquipped;
    public Weapon RelatedWeapon;

    public void EquipGun()
    {
        EquippedText.text = "EQUIPPED";
        EquippedBackgroundImage.color = Color.green;
        isEquipped = true;
    }

    public void UnequipGun()
    {
        EquippedText.text = "EQUIP";
        EquippedBackgroundImage.color = Color.white;
        isEquipped = false;
    }
}
