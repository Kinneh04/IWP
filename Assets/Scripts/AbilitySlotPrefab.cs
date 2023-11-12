using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AbilitySlotPrefab : MonoBehaviour
{
    public int Cost;
    public bool isEquipped, isBought;
    public GameObject Lockscreen;

    public GameObject HeldAbilityObject;
    public Image IconImage;
    public Button EquipButton, BuyButton;
    public TMP_Text EquippedText, CostText, titletext, DescriptionText;
    public Image EquippedBackgroundImage;
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
