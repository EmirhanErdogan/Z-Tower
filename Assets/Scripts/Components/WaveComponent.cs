using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveComponent
{
    #region Serializable Fields

    public int Index;
    public List<EnemyComponent> Enemys = new List<EnemyComponent>();

    #endregion
}