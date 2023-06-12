using UnityEngine;
using System.Collections.Generic;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/UpgradeUIData", fileName = "GameSettings", order = 0)]
    
    public class UpgradeUIScriptable : ScriptableObject
    {
        public EUpgradeType Upgrade;
        public int Level;
        public List<float> Prices;
        public List<float> Values;
    }
}