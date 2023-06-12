using System.Collections;
using System.Collections.Generic;
using Emir;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIController : Singleton<WeaponUIController>
{
    #region Serializable Fields

    [SerializeField] private CanvasGroup WeaponCanvasGroup;

    [SerializeField] private Image WeaponShopIcon;
    [SerializeField] private Image CurrentOutline;
    [SerializeField] private TextMeshProUGUI CurrentText;
    [SerializeField] private List<WeaponUI> WeaponUIs = new List<WeaponUI>();

    #endregion

    #region Private Fields

    private bool IsActive = false;

    #endregion

    public void IconClick()
    {
        if (IsActive is false)
        {
            IsActive = true;
            WeaponShopIcon.gameObject.SetActive(false);
            UpgradeUIController.Instance.GetUpgaradeIcon().gameObject.SetActive(false);
            GameUtils.SwitchCanvasGroup(null, WeaponCanvasGroup);
        }
        else if (IsActive is true)
        {
            IsActive = false;
            WeaponShopIcon.gameObject.SetActive(true);
            UpgradeUIController.Instance.GetUpgaradeIcon().gameObject.SetActive(true);
            GameUtils.SwitchCanvasGroup(WeaponCanvasGroup, null, 0.1f);
            LevelComponent.Instance.Save();
        }
    }

    #region Getters

    public List<WeaponUI> GetWeaponUIs()
    {
        return WeaponUIs;
    }

    public Image GetWeaponIcon()
    {
        return WeaponShopIcon;
    }

    public void SetCurrentOutline(Image Outline)
    {
        CurrentOutline = Outline;
    }

    public Image GetOutline()
    {
        return CurrentOutline;
    }

    public void SetCurrentText(TextMeshProUGUI Text)
    {
        CurrentText = Text;
    }

    public TextMeshProUGUI GetText()
    {
        return CurrentText;
    }

    #endregion
}