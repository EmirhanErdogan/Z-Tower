using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Emir;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EWeaponType _weaponType;
    [SerializeField] private WeaponUIScriptable Data;
    [SerializeField] private Image Icon;
    [SerializeField] private Image Outline;
    [SerializeField] private TextMeshProUGUI PriceText;
    [SerializeField] private TextMeshProUGUI CurrentRangeText;
    [SerializeField] private TextMeshProUGUI CurrentDamageText;

    [SerializeField] private TextMeshProUGUI CurrentFireRateText;
    // [SerializeField] private TextMeshProUGUI TargetRangeText;
    // [SerializeField] private TextMeshProUGUI TargetDamageText;
    // [SerializeField] private TextMeshProUGUI TargetFireRateText;

    #endregion

    private void Start()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (Data.IsBuy is true)
            {
                Icon.color = Color.white;
                PriceText.text = "Unequipped";
            }
            else
            {
                Icon.color = GameManager.Instance.GetGameSettings().UnselectedColor;
                PriceText.text = String.Format("$ {0}", Data.Price.ToString());
            }
        });
    }

    public void ClickWeapon()
    {
        if (Data.IsBuy is false)
        {
            //satın alma işlemi kontrolü
            if (GameManager.Instance.GetCurreny() >= Data.Price)
            {
                //satın alma
                Data.IsBuy = true;
                LevelComponent.Instance.GetActiveWeapons().Add(_weaponType);
                LevelComponent.Instance.Save();
                GameManager.Instance.SetCurrency(-(int)Data.Price);
                InterfaceManager.Instance.OnPlayerCurrencyUpdated();
                PriceText.text = "Unequipped";
                UIChange();
            }
            else
            {
                //satın alamaz
            }
        }
        else if (Data.IsBuy is true)
        {
            //seçme işlemi
            WeaponComponent TargetWeapon =
                WeaponController.Instance.GetWeapons().First(x => x.GetWeaponType() == _weaponType);
            WeaponController.Instance.SelectWeapon(TargetWeapon);
            WeaponUIController.Instance.GetOutline().enabled = false;
            WeaponUIController.Instance.GetText().text = "Unequipped";
            WeaponUIController.Instance.SetCurrentOutline(Outline);
            WeaponUIController.Instance.SetCurrentText(PriceText);
            WeaponUIController.Instance.GetOutline().enabled = true;
            WeaponUIController.Instance.GetText().text = "Equipped";
            PlayerPrefs.SetString(CommonTypes.LAST_WEAPON_DATA, _weaponType.ToString());
        }
    }

    private void UIChange()
    {
        Icon.color = Color.white;
    }

    public EWeaponType GetWeaponType()
    {
        return _weaponType;
    }

    public TextMeshProUGUI GetText()
    {
        return PriceText;
    }

    public Image GetOutline()
    {
        return Outline;
    }
}