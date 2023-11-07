using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StoreManager : MonoBehaviour
{
    public List<Weapon> Weapons = new List<Weapon>();
    public GameObject WeaponPrefab, WeaponPrefabParent;
    public int CurrentCoinsAmount;
    public TMP_Text CoinsText;

    [Header("CurrentlyEquippedGunStuff")]

    public WeaponSlotPrefab currentWeaponEquipped;
    public Animator GunAnimator;
    public AnimationClip FireAnimClip;
    public int maxAmmo, currentAmmo;
    public List<WeaponReloadPart> ReloadAnims = new List<WeaponReloadPart>();
    public GameObject ActiveWeapon;
    public WeaponMovement weaponMove;   

    [Header("Weapory")]
    public ShootingScript shootingScript;

    [Header("DontFIll")]
    public List<WeaponSlotPrefab> InstantiatedWeaponSlots = new List<WeaponSlotPrefab>();

    public void Unlockweapon(WeaponSlotPrefab WSP)
    {
        if(WSP.Cost <= CurrentCoinsAmount)
        {
            CurrentCoinsAmount -= WSP.Cost;
            WSP.isPurchased = true;
            WSP.LockScreen.SetActive(false);
            CoinsText.text = "GP: " + CurrentCoinsAmount.ToString();
        }
    }
    public void UnequipAllWeapons()
    {
        foreach(WeaponSlotPrefab WSP in InstantiatedWeaponSlots)
        {
            WSP.UnequipGun();
        }
    }
    public void SelectWeapon(WeaponSlotPrefab WSP)
    {
        if(currentWeaponEquipped)
        {
            currentWeaponEquipped.RelatedWeapon.ActiveWeapon.SetActive(false);
        }
        UnequipAllWeapons();
        WSP.EquipGun();
        currentWeaponEquipped = WSP;

        Weapon W = WSP.RelatedWeapon;
        GunAnimator = W.GunAnimator;
        FireAnimClip = W.FireAnimClip;
        maxAmmo = W.maxAmmo;
        currentAmmo = W.currentAmmo;
        ReloadAnims = W.ReloadAnims;
        ActiveWeapon = W.ActiveWeapon;
        weaponMove = W.weaponMovement;


    }

    public void LoadWeaponDetails()
    {
        shootingScript.MainWeaponAnimator = GunAnimator;
        shootingScript.FireAnimClip = FireAnimClip;
        shootingScript.maxAmmo = maxAmmo;
        shootingScript.CurrentAmmo = currentAmmo;
        shootingScript.ReloadAnimClips = ReloadAnims;
        shootingScript.weaponMovement = weaponMove;
        ActiveWeapon.SetActive(true);
    }
    private void Awake()
    {
        CoinsText.text = "GP: " + CurrentCoinsAmount.ToString();
        foreach(Weapon W in Weapons)
        {
            GameObject GO = Instantiate(WeaponPrefab);
            GO.transform.SetParent(WeaponPrefabParent.transform);
            WeaponSlotPrefab WSP = GO.GetComponent<WeaponSlotPrefab>();
            InstantiatedWeaponSlots.Add(WSP);
            WSP.IconImage.sprite = W.GunImage;
            WSP.EquipButton.onClick.AddListener(delegate { SelectWeapon(WSP); });
            WSP.BuyButton.onClick.AddListener(delegate { Unlockweapon(WSP); });
            WSP.WeaponTitleText.text = W.GunName;
            WSP.WeaponDescText.text = W.GunDesc;
            WSP.Cost = W.GunCost;
            WSP.CostText.text = "Cost: " + WSP.Cost.ToString();
            WSP.RelatedWeapon = W;

            if (W.TotalShots == -1)
            {
                WSP.WeaponDetailsText.text = "Shots: Inf\n";
            }
            else WSP.WeaponDetailsText.text = "Shots: " + W.TotalShots.ToString() + "\n";
            if(W.ReloadCycles <= 0)
            {
                WSP.WeaponDetailsText.text += "Reload Cycles: None";
                
            }
            else
            {
                WSP.WeaponDetailsText.text += "Reload Cycles: " + W.ReloadCycles.ToString();
            }

            if (W.GunUnlocked)
            {
                WSP.isPurchased = true;
                WSP.LockScreen.SetActive(false);
            }
            else
            {
                WSP.LockScreen.SetActive(true);
            }
            if(W.GunEquippedOnStart)
            {
                SelectWeapon(WSP);
            }

        }
    }
}
[Serializable]
public class Weapon
{
    public string GunName, GunDesc;
    public int TotalShots, ReloadCycles;
    public Sprite GunImage;
    public int GunCost;
    public bool GunEquippedOnStart, GunUnlocked;

    [Header("ForSelectionChange")]
    public Animator GunAnimator;
    public AnimationClip FireAnimClip;
    public int maxAmmo, currentAmmo;
    public List<WeaponReloadPart> ReloadAnims = new List<WeaponReloadPart>();
    public GameObject ActiveWeapon;
    public WeaponMovement weaponMovement;
}