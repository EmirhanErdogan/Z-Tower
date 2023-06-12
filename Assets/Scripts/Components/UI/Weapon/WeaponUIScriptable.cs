using UnityEngine;
using System.Collections.Generic;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/WeaponUIData", fileName = "GameSettings", order = 0)]
    public class WeaponUIScriptable : ScriptableObject
    {
        public EWeaponType WeaponType;
        public float Price;
        public bool IsBuy;
    }
}