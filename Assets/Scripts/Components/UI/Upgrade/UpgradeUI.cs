using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Emir;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ExistingUpgradeData
{
    public EUpgradeType Upgradeype;
    public int Id;
}

public class UpgradeUI : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EUpgradeType _eUpgradeType;
    [SerializeField] private UpgradeUIScriptable Data;
    [SerializeField] private TextMeshProUGUI _pricetext;
    [SerializeField] private TextMeshProUGUI CurrentValuetext;
    [SerializeField] private TextMeshProUGUI TargetValuetext;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private Image MoneyIcon;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private List<Image> LevesIcons = new List<Image>();

    #endregion

    #region Private Fields

    private bool IsActive = true;

    #endregion

    private void Start()
    {
        IconUpdated();
        PriceTextUpdated();
        MaxLevelControl();
        CurrentValueUpdated();
        TargetValueUpdated();
        DOVirtual.DelayedCall(0.25f, () => { UIChangeControl(); });
    }

    public void ClickUpgrade()
    {
        //satın alma işlemi kontrolü
        if (GameManager.Instance.GetCurreny() >= Data.Prices[Data.Level])
        {
            //UPGRADE GELİŞTİRME
            GameManager.Instance.SetCurrency(-(int)Data.Prices[Data.Level]);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            Data.Level++;
            if (Data.Upgrade == EUpgradeType.GETASKILLATSTART)
            {
                GameManager.Instance.GetPlayerView().SetIsGlory();
            }

            CurrentValueUpdated();
            TargetValueUpdated();
            MaxLevelControl();
            IconUpdated();
            PriceTextUpdated();
            GameManager.Instance.UICheck();
            LevelComponent.Instance.Save();
        }
    }

    public void UIChangeControl()
    {
        if (GameManager.Instance.GetCurreny() < Data.Prices[Data.Level])
        {
            //unselected
            ButtonImage.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            MoneyIcon.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            _pricetext.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            InfoText.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            if (CurrentValuetext is not null)
            {
                CurrentValuetext.color = GameManager.Instance.GetGameSettings().UnselectedColor;
                TargetValuetext.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            }

            foreach (var Icon in LevesIcons)
            {
                Icon.color = GameManager.Instance.GetGameSettings().UnselectedColor;
            }
        }
        else
        {
            //selected
            ButtonImage.color = Color.white;
            MoneyIcon.color = Color.white;
            _pricetext.color = Color.black;
            InfoText.color = Color.white;
            if (CurrentValuetext is not null)
            {
                CurrentValuetext.color = Color.black;
                TargetValuetext.color = Color.black;
            }

            foreach (var Icon in LevesIcons)
            {
                Icon.color = Color.white;
            }
        }
    }

    public UpgradeUIScriptable GetData()
    {
        return Data;
    }

    #region Text

    private void PriceTextUpdated()
    {
        _pricetext.text = Data.Prices[Data.Level].ToString();
    }

    private void CurrentValueUpdated()
    {
        if (CurrentValuetext is null) return;
        if (Data.Level == 0)
        {
            CurrentValuetext.text = "1";
        }
        else
        {
            CurrentValuetext.text = Data.Values[Data.Level].ToString();
        }
    }

    private void TargetValueUpdated()
    {
        if (TargetValuetext is null) return;
        if (Data.Level >= Data.Prices.Count - 1)
        {
            TargetValuetext.text = "Max";
        }
        else
        {
            TargetValuetext.text = Data.Values[Data.Level + 1].ToString();
        }
    }

    #endregion

    #region MaxLevelControl

    private void MaxLevelControl()
    {
        if (Data.Level >= Data.Prices.Count - 1)
        {
            //para iconu kapanacak
            MoneyIcon.enabled = false;
            //Para yerinde Max Yazacak
            _pricetext.text = "Max";
            //Max seviyeyse etkileşime girmeyecek
            IsActive = false;
        }
        else
        {
            IsActive = true;
        }
    }

    #endregion

    #region Icon

    private void IconUpdated()
    {
        int ImageValue = GetKalan(Data.Level);
        if (ImageValue == 0)
        {
            if (Data.Level > 0)
            {
                ImageValue = 5;
            }
        }

        for (int i = 0; i < LevesIcons.Count; i++)
        {
            LevesIcons[i].enabled = false;
        }

        for (int i = 0; i < ImageValue; i++)
        {
            LevesIcons[i].enabled = true;
        }
    }

    int GetKalan(int Value)
    {
        int kalan = Value % 5;
        // if (Value % 5 != 0)
        // {
        //     kalan = Value % 5;
        // }

        return kalan;
    }

    #endregion
}