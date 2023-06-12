using System.Collections;
using System.Collections.Generic;
using Emir;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIController : Singleton<UpgradeUIController>
{
    #region Serializable Fields

    [SerializeField] private CanvasGroup UpgradeCanvasGroup;

    [SerializeField] private Image UpgradeShopIcon;
    [SerializeField] private List<UpgradeUI> UpgradeUIs = new List<UpgradeUI>();

    #endregion

    #region Private Fields

    private bool IsActive = false;

    #endregion

    public void IconClick()
    {
        if (IsActive is false)
        {
            IsActive = true;
            UpgradeShopIcon.gameObject.SetActive(false);
            WeaponUIController.Instance.GetWeaponIcon().gameObject.SetActive(false);
            GameUtils.SwitchCanvasGroup(null, UpgradeCanvasGroup);
        }
        else if (IsActive is true)
        {
            IsActive = false;
            UpgradeShopIcon.gameObject.SetActive(true);
            WeaponUIController.Instance.GetWeaponIcon().gameObject.SetActive(true);
            GameUtils.SwitchCanvasGroup(UpgradeCanvasGroup, null, 0.1f);
            LevelComponent.Instance.Save();
        }
    }

    #region Getters

    public List<UpgradeUI> GetUpgradeUIs()
    {
        return UpgradeUIs;
    }

    public Image GetUpgaradeIcon()
    {
        return UpgradeShopIcon;
    }

    #endregion
}