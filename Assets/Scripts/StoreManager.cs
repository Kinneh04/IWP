using System;
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

    [Header("ForRevolverBasedReloads")]
    public WeaponReloadPart StartingReloadAnim, EndingReloadAnim, repeatingReloadAnim;
    public bool isRevolverReloadType = false;
    int revolverReloadTypeIndex = 0;

    public GameObject ActiveWeapon;
    public WeaponMovement weaponMove;
    public Transform ShootPoint;
    public int NumberOfShotsPerShot;
    public AudioClip GunFireAudio;
    [Header("Weapory")]
    public ShootingScript shootingScript;

    [Header("Abilities")]
    public GameObject CurrentlyEquippedAbilityObject;
    public List<ShopAbility> AvailableAbilities = new List<ShopAbility>();
    public List<AbilitySlotPrefab> InstantiatedAbilities;
    public GameObject AbilityPrefab, AbilityPrefabParent;
    public int BurstShotCount;
    public GameObject InstantiatedAbility;

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
        isRevolverReloadType = W.isRevolverReloadType;
        StartingReloadAnim = W.StartingReloadAnim;
        EndingReloadAnim = W.EndingReloadAnim;
        repeatingReloadAnim = W.repeatingReloadAnim;
        ActiveWeapon = W.ActiveWeapon;
        weaponMove = W.weaponMovement;
        ShootPoint = W.ShootPoint;
        GunFireAudio = W.GunFireAudio;
        NumberOfShotsPerShot = W.NumberOfBulletsPerShot;
        BurstShotCount = W.BurstShotCount;

    }
    public void LoadAbilityDetails()
    {
        if(InstantiatedAbility)
        {
            Destroy(InstantiatedAbility);
        }

        GameObject GO = Instantiate(CurrentlyEquippedAbilityObject);
        FirstPersonController.Instance.CurrentAbility = GO.GetComponent<Ability>();
        InstantiatedAbility = GO;
    }

    public void LoadWeaponDetails()
    {
        shootingScript.Revolver = ActiveWeapon;
        shootingScript.MainWeaponAnimator = GunAnimator;
        shootingScript.FireAnimClip = FireAnimClip;
        shootingScript.maxAmmo = maxAmmo;
        shootingScript.CurrentAmmo = currentAmmo;
        shootingScript.ReloadAnimClips = ReloadAnims;
        shootingScript.weaponMovement = weaponMove;
        shootingScript.GunShootFrom = ShootPoint;
        shootingScript.ShootingAudioClip = GunFireAudio;
        shootingScript.NumberOfBulletsPerShot = NumberOfShotsPerShot;
        shootingScript.MaxShotDistance = currentWeaponEquipped.RelatedWeapon.ShootDistance;
        shootingScript.BurstShotCount = BurstShotCount;
        if(isRevolverReloadType)
        {
            shootingScript.isRevolverReloadType = isRevolverReloadType;
            shootingScript.StartingReloadAnim = StartingReloadAnim;
            shootingScript.EndingReloadAnim = EndingReloadAnim;
            shootingScript.repeatingReloadAnim = repeatingReloadAnim;
        }
        ActiveWeapon.SetActive(true);
    }
    private void Awake()
    {
        CoinsText.text = "GP: " + CurrentCoinsAmount.ToString();
        foreach(Weapon W in Weapons)
        {
            if (W.OnPlaceholder) continue;
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

        foreach(ShopAbility SA in AvailableAbilities)
        {
            GameObject GO = Instantiate(AbilityPrefab);
            GO.transform.SetParent(AbilityPrefabParent.transform);
            AbilitySlotPrefab WSP = GO.GetComponent<AbilitySlotPrefab>();
            InstantiatedAbilities.Add(WSP);
            WSP.HeldAbilityObject = SA.AttachedAbility;
            WSP.IconImage.sprite = SA.AbilitySprite;
            WSP.Cost = SA.Cost;
            WSP.isBought = SA.bought;
            WSP.isEquipped = SA.equipped;
            WSP.CostText.text = SA.Cost.ToString();
            WSP.titletext.text = SA.AbilityName;
            WSP.DescriptionText.text = SA.abilityDesc;
            WSP.EquipButton.onClick.AddListener(delegate { SelectAbility(WSP); });
            WSP.BuyButton.onClick.AddListener(delegate { BuyAbility(WSP); });
            if(WSP.isBought)
            {
                WSP.Lockscreen.SetActive(false);
                if(WSP.isEquipped)
                {
                    SelectAbility(WSP);
                }
            }
            else
            {
                WSP.Lockscreen.SetActive(true);
            }
        }
    }

    public void SelectAbility(AbilitySlotPrefab ASP)
    {
        foreach(AbilitySlotPrefab a in InstantiatedAbilities)
        {
            a.UnequipGun();
        }

        ASP.EquipGun();
        CurrentlyEquippedAbilityObject = ASP.HeldAbilityObject;
    }

    public void BuyAbility(AbilitySlotPrefab ASP)
    {
        if(CurrentCoinsAmount > ASP.Cost)
        {
            CurrentCoinsAmount -= ASP.Cost;
            CoinsText.text = "GP: " +CurrentCoinsAmount.ToString();
            ASP.isBought = true;
            ASP.Lockscreen.SetActive(false);
        }
    }
}
[Serializable]
public class Weapon
{

    public bool OnPlaceholder = false;
    public string GunName, GunDesc;
    public int TotalShots, ReloadCycles;
    public Sprite GunImage;
    public int GunCost;
    public bool GunEquippedOnStart, GunUnlocked;
    public int NumberOfBulletsPerShot;
    public AudioClip GunFireAudio;
    [Header("ForSelectionChange")]
    public Animator GunAnimator;
    public AnimationClip FireAnimClip;
    public int maxAmmo, currentAmmo;
    public List<WeaponReloadPart> ReloadAnims = new List<WeaponReloadPart>();
    public int BurstShotCount = 1;
    [Header("ForRevolverBasedReloads")]
    public WeaponReloadPart StartingReloadAnim, EndingReloadAnim, repeatingReloadAnim;
    public bool isRevolverReloadType = false;
    int revolverReloadTypeIndex = 0;

    public GameObject ActiveWeapon;
    public WeaponMovement weaponMovement;
    public Transform ShootPoint;
    public float ShootDistance;
}