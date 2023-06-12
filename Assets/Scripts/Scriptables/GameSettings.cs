using UnityEngine;
using System.Collections.Generic;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/GameSettings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Datas")] [ContextMenuItem("Update", "FindLevels")]
        public Level[] Levels;

        [Header("Values")] public float BulletDestroyTime;

        [Header("Prefabs")] public BulletComponent BulletPrefab;
        public EnemyBombComponent BombPrefab;
        public BombComponent PlayerBombPrefab;
        public List<EnemyComponent> EnemysPrefab;

        [Header("Colors")] public Color UnselectedColor;
#if UNITY_EDITOR

        /// <summary>
        /// This function helper for update levels list.
        /// </summary>
        public void FindLevels()
        {
            Levels = null;

            List<Level> foundLevels = new List<Level>();
            Object[] objects = Resources.LoadAll(CommonTypes.EDITOR_LEVELS_PATH);

            foreach (Object targetObject in objects)
            {
                if (targetObject is not Level)
                    continue;

                foundLevels.Add(targetObject as Level);
            }

            Levels = foundLevels.ToArray();
        }

#endif
    }
}