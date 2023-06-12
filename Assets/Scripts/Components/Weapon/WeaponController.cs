using System.Collections;
using System.Collections.Generic;
using Emir;
using UnityEngine;

public class WeaponController : Singleton<WeaponController>
{
    #region Serializable Fields

    [SerializeField] private List<WeaponComponent> Weapons = new List<WeaponComponent>();

    #endregion


    public List<WeaponComponent> GetWeapons()
    {
        return Weapons;
    }

    public void SelectWeapon(WeaponComponent TargetWeapon)
    {
        GameManager.Instance.GetPlayerView().SetWeapon(null);
        foreach (WeaponComponent Weapon in GetWeapons())
        {
            Weapon.gameObject.SetActive(false);
        }

        TargetWeapon.gameObject.SetActive(true);
        GameManager.Instance.GetPlayerView().SetWeapon(TargetWeapon);
    }
}