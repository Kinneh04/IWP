using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
  public enum potionType
    {
        Heal, Rage, Damage
    }

    public potionType typeOfPotion;


    public void UsePotion()
    {
        if (typeOfPotion == potionType.Heal)
        {
            FirstPersonController.Instance.HealPlayer(50);
        }
        else if (typeOfPotion == potionType.Damage)
        {
            PlayerRatingController.Instance.shootingScript.SetDamage += 1;
            FirstPersonController.Instance.PopupNotif("Damage increase!");
        }
        else
        {
            PlayerRatingController.Instance.CurrentFrenzyAmount += 15f;
            FirstPersonController.Instance.PopupNotif("Frenzy increase!");
        }
            PlayerRatingController.Instance.AddRating(10, "Potion", new Color(1, 0, 0.9f, 1));
    }
}
