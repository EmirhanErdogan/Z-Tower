using AlmostEngine.Screenshot;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;

namespace Emir
{
    public static class CommonTypes
    {
        //GENERICS
        public static int DEFAULT_FPS = 60;
        public static int DEFAULT_THREAD_SLEEP_MS = 100;

        //INTERFACES
        public static float UI_DEFAULT_FLY_CURRENCY_DURATION = 0.5F;

        //SOUNDS
        public static string SFX_CLICK = "CLICK";
        public static string SFX_CURRENCY_FLY = "CURRENCY_FLY";
        public static string SFX_WIN = "WIN";
        public static string SFX_LOSE = "LOSE";
        public static string SFX_FIRE = "FIRE";

        //DATA KEYS
        public static string PLAYER_DATA_KEY = "player_data";
        public static string LEVEL_ID_DATA_KEY = "level_data";
        public static string SOUND_STATE_KEY = "sound_state_data";
        public static string VIBRATION_STATE_KEY = "vibration_state_data";
        public static string CURRENCY_DATA_KEY = "Currency";
        public static string IS_START_DATA_KEY = "IsStart";
        public static string WEAPON_DATA = "WeaponData";
        [CanBeNull] public static string UPGRADE_UI_DATA = "UpgradeUIData";
        public static string LAST_WEAPON_DATA = "LastWeaponData";
        public static string IS_GLORY_SKILL = "GLORY";


        //SOUND TAG
        public static string SOUND_CLICK = "CLICK";
        public static string SOUND_WIN = "WIN";
        public static string SOUND_LEVELUP = "LEVELUP";
        public static string SOUND_LOSE = "LOSE";
        public static string SOUND_MERGE = "MERGE";
        public static string SOUND_DRAG = "DRAG";


        //TAGS
        public static string ZOMBIE_TAG = "Zombie";

#if UNITY_EDITOR

        public static string EDITOR_LEVELS_PATH = "Levels/";
        public static string EDITOR_GAME_SETTINGS_PATH = "GameSettings";

#endif
    }

    public static class GameUtils
    {
        public static float CalculateHealthPercentage(float currentHealth, float maxHealth)
        {
            float healthPercentage = (currentHealth / maxHealth) * 100f;
            return healthPercentage;
        }

        public static void SwitchCanvasGroup(CanvasGroup a, CanvasGroup b, float duration = 0.25F)
        {
            Sequence sequence = DOTween.Sequence();

            if (a != null)
                sequence.Join(a.DOFade(0, duration));
            if (b != null)
                sequence.Join(b.DOFade(1, duration));

            sequence.OnComplete(() =>
            {
                if (a != null)
                    a.blocksRaycasts = false;
                if (b != null)
                    b.blocksRaycasts = true;
            });

            sequence.Play();
        }

        public static Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 worldPosition)
        {
            Vector2 tempPosition = camera.WorldToViewportPoint(worldPosition);

            tempPosition.x *= canvas.sizeDelta.x;
            tempPosition.y *= canvas.sizeDelta.y;

            tempPosition.x -= canvas.sizeDelta.x * canvas.pivot.x;
            tempPosition.y -= canvas.sizeDelta.y * canvas.pivot.y;

            return tempPosition;
        }

        public static Vector3 ReflectDirection(Vector3 Direction, Vector3 Position)
        {
            return Vector3.Reflect(Direction, Position);
        }

        public static string ConvertMoney(this int num)
        {
            return num switch
            {
                >= 100000000 => (num / 1000000).ToString("#,0M"),
                >= 10000000 => (num / 1000001).ToString("0.#") + "M",
                >= 100000 => ((float)num / 1001f).ToString("#,0K"),
                >= 10000 => ((float)num / 1001f).ToString("0.##") + "K",
                >= 1000 => ((float)num / 1000f).ToString("#.##") + "K",
                _ => num.ToString("#,0")
            };
        }

        public static string ConvertMoney(this float num)
        {
            return num switch
            {
                >= 10000000 => (num / 1000001).ToString("#,0M"),
                >= 1000000 => (num / 1000001).ToString("0.#") + "M",
                >= 100000 => ((float)num / 1001f).ToString("#,0K"),
                >= 10000 => ((float)num / 1001f).ToString("0.##") + "K",
                >= 1000 => ((float)num / 1000f).ToString("#.##") + "K",
                _ => num.ToString("#,0")
            };
        }
    }
}