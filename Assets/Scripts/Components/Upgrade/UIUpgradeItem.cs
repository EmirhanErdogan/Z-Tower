using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Emir;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class UIUpgradeItem : MonoBehaviour
{
    [SerializeField] private ESkill skillType;
    [SerializeField] private UpgradeItemSettings settings;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button btn;


    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        switch (skillType)
        {
            case ESkill.ATTACKDAMAGEBOOST:
                btn.onClick.AddListener(SkillComponent.Instance.AttackDamageSkill);
                break;
            case ESkill.ATTACKSPEEDBOOST:
                btn.onClick.AddListener(SkillComponent.Instance.FireRateSkill);
                break;
            case ESkill.INCREASEMAXHP:
                btn.onClick.AddListener(SkillComponent.Instance.MaxHPSkill);
                break;
            case ESkill.LIFESTEAL:
                btn.onClick.AddListener(SkillComponent.Instance.LifeStealSkill);
                break;
            case ESkill.SMART:
                btn.onClick.AddListener(SkillComponent.Instance.SmartSkill);
                break;
            case ESkill.DOUBLESHOT:
                btn.onClick.AddListener(SkillComponent.Instance.DoubleShotSkill);
                break;
            case ESkill.RİCOCHETSHOT:
                btn.onClick.AddListener(SkillComponent.Instance.RicochetSkill);
                break;
            case ESkill.PIERCINGSHOT:
                btn.onClick.AddListener(SkillComponent.Instance.PiercingShotSkill);
                break;
            case ESkill.HEAL:
                btn.onClick.AddListener(SkillComponent.Instance.HealUpSkill);
                break;
            case ESkill.HEADSHOT:
                btn.onClick.AddListener(SkillComponent.Instance.HeadShotSkill);
                break;
            case ESkill.FREEZESHOT:
                btn.onClick.AddListener(SkillComponent.Instance.FreezeShotSkill);
                break;
            case ESkill.FIRESHOT:
                btn.onClick.AddListener(SkillComponent.Instance.FireShotSkill);
                break;
            case ESkill.RAGE:
                btn.onClick.AddListener(SkillComponent.Instance.RageSkill);
                break;
            case ESkill.EXPLODINGSHOT:
                btn.onClick.AddListener(SkillComponent.Instance.ExplodingShotSkill);
                break;
            case ESkill.CRITMASTER:
                btn.onClick.AddListener(SkillComponent.Instance.CritMasterSkill);
                break;
        }
    }

    public TextMeshProUGUI GetText()
    {
        return _text;
    }

    public ESkill GetSkillType()
    {
        return skillType;
    }

    public UpgradeItemSettings GetSettings()
    {
        return settings;
    }
}