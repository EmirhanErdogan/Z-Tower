using Emir;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ESkill
{
    NONE,
    ATTACKDAMAGEBOOST,
    ATTACKDAMAGEBOOSTMINOR,
    ATTACKSPEEDBOOST,
    ATTACKSPEEDBOOSTMINOR,
    INCREASEMAXHP,
    LIFESTEAL,
    DOUBLESHOT,
    SPREADSHOT,
    RİCOCHETSHOT,
    PIERCINGSHOT,
    FREEZESHOT,
    FIRESHOT,
    HEADSHOT,
    EXPLODINGSHOT,
    CRITMASTERCHANGEDAMAGE,
    CRITMASTER,
    HEAL,
    RAGE,
    SMART,
}

[CreateAssetMenu(menuName = "Emir/Game/Skill", fileName = "Skill", order = 1)]
[System.Serializable]
public class UpgradeItemSettings : ScriptableObject
{
    public ESkill type;
    public int Level = 1;
}