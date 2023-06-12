using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Emir;
using UnityEngine;

public class SkillComponent : Singleton<SkillComponent>
{
    #region Serializable Fields

    [SerializeField] private CanvasGroup skillCanvasGroup;

    #endregion

    #region Private Fields

    private bool IsLifeSteal;
    private bool IsSmartSkill;
    private bool IsRicochetSkill;
    private bool IsPiercingSkill;
    private bool IsHeadShotSkill;
    private bool IsFreezeShotSkill;
    private bool IsFireShotSkill;
    private bool IsRageSkill;

    #endregion

    public void AttackDamageSkill()
    {
        GameManager.Instance.GetPlayerView().SetDamageMultiply(0.25f);
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.ATTACKDAMAGEBOOST);
        ButtonEvent();
    }

    public void FireRateSkill()
    {
        GameManager.Instance.GetPlayerView().GetWeapon().SetFireRate(0.05f);
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.ATTACKSPEEDBOOST);
        ButtonEvent();
    }

    public void MaxHPSkill()
    {
        GameManager.Instance.GetPlayerView().SetMaxHealthIncrease(100);
        GameManager.Instance.GetPlayerView().SetHealthIncrease(100);
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.INCREASEMAXHP);
        ButtonEvent();
    }

    public void LifeStealSkill()
    {
        //enemy öldürünce xp kazanıyoruz
        IsLifeSteal = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.LIFESTEAL);
        ButtonEvent();
    }

    public void SmartSkill()
    {
        //enemy öldürünce para kazanıyoruz
        IsSmartSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.SMART);
        ButtonEvent();
    }

    public void DoubleShotSkill()
    {
        GameManager.Instance.GetPlayerView().GetWeapon().SetShotCount(2);
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.DOUBLESHOT);
        ButtonEvent();
    }

    public void RicochetSkill()
    {
        IsRicochetSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.RİCOCHETSHOT);
        ButtonEvent();
    }

    public void PiercingShotSkill()
    {
        IsPiercingSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.PIERCINGSHOT);
        ButtonEvent();
    }

    public void HealUpSkill()
    {
        GameManager.Instance.GetPlayerView().SetHealthIncrease(50);
        GameManager.Instance.GetPlayerView().SliderUpdated();
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.HEAL);
        ButtonEvent();
    }

    public void HeadShotSkill()
    {
        IsHeadShotSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.HEADSHOT);
        ButtonEvent();
    }

    public void FreezeShotSkill()
    {
        IsFreezeShotSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.FREEZESHOT);
        ButtonEvent();
    }

    public void FireShotSkill()
    {
        IsFireShotSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.FIRESHOT);
        ButtonEvent();
    }


    public void RageSkill()
    {
        IsRageSkill = true;
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.RAGE);
        ButtonEvent();
    }

    public void ExplodingShotSkill()
    {
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.EXPLODINGSHOT);
        ButtonEvent();
    }

    public void CritMasterSkill()
    {
        GameManager.Instance.GetPlayerView().CriticialDamageChangeIncrease();
        UIUpgrade.Instance.CloseUpgradeUI();
        RemoveSkill(ESkill.CRITMASTER);
        ButtonEvent();
    }

    private void RemoveSkill(ESkill Type)
    {
        UIUpgradeItem Item = LevelComponent.Instance.GetItems().First(x => x.GetSkillType() == Type);
        LevelComponent.Instance.GetItems().Remove(Item);
        Time.timeScale = 1;
    }

    private void ButtonEvent()
    {
        Time.timeScale = 1;
        SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
        HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);
        GameUtils.SwitchCanvasGroup(InterfaceManager.Instance.GetMenuCanvas(), null);
        LevelComponent.Instance.CreateWave();
        SliderComponent.Instance.SliderValueReset();
        SliderComponent.Instance.SliderInit();
        SliderComponent.Instance.SliderTextUpdated();
    }


    #region Getters

    public CanvasGroup GetSkillCnavasGroup()
    {
        return skillCanvasGroup;
    }

    public bool GetIsLifeSteal()
    {
        return IsLifeSteal;
    }

    public bool GetIsSmartSkill()
    {
        return IsSmartSkill;
    }

    public bool GetIsRicochetSkill()
    {
        return IsRicochetSkill;
    }

    public bool GetIsPiercingSkill()
    {
        return IsPiercingSkill;
    }

    public bool GetIsHeadShotSkill()
    {
        return IsHeadShotSkill;
    }

    public bool GetIsFreezeShotSkill()
    {
        return IsFreezeShotSkill;
    }

    public bool GetIsFireShotSkill()
    {
        return IsFireShotSkill;
    }

    public bool GetIsRageSkill()
    {
        return IsRageSkill;
    }

    #endregion
}