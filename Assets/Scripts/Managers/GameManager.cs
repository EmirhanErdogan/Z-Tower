using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Emir
{
    public class GameManager : Singleton<GameManager>
    {
        #region Serializable Fields

        [Header("Controllers")] [SerializeField]
        private GameSettings m_gameSettings;

        [SerializeField] private PlayerView m_playerView;

        #endregion

        #region Private Fields

        private EGameState gameState = EGameState.NONE;

        #endregion

        /// <summary>
        /// Start.
        /// </summary>
        private void Start()
        {
            Application.targetFrameRate = CommonTypes.DEFAULT_FPS;
            InitializeWorld();
        }

        /// <summary>
        /// This function helper for initialize world.
        /// </summary>
        private void InitializeWorld()
        {
            Level currentLevel = LevelService.GetCurrentLevel();
            Instantiate(currentLevel.LevelPrefab);

            ThemeManager.Instance.Initialize(currentLevel.Theme);
            SetCurrency(50000);
            ChangeGameState(EGameState.STAND_BY);
        }

        /// <summary>
        /// This function helper for start game.
        /// </summary>
        public void StartGame()
        {
            SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
            HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);
            ChangeGameState(EGameState.STARTED);
            InterfaceManager.Instance.OnGameStateChanged(GetGameState());
            WeaponUIController.Instance.GetWeaponIcon().gameObject.SetActive(false);
            UpgradeUIController.Instance.GetUpgaradeIcon().gameObject.SetActive(false);
            UIUpgrade.Instance.Initialize();
            DOVirtual.DelayedCall(2f, () => { PlayerPrefs.SetFloat(CommonTypes.IS_START_DATA_KEY, 1); });
        }

        /// <summary>
        /// This function helper for change current game state.
        /// </summary>
        /// <param name="gameState"></param>
        public void ChangeGameState(EGameState gameState)
        {
            if (this.gameState == EGameState.WIN)
                return;

            if (this.gameState == EGameState.LOSE)
                return;

            if (this.gameState == EGameState.STAND_BY && (gameState == EGameState.WIN || gameState == EGameState.LOSE))
                return;

            this.gameState = gameState;
        }

        public void OnGameStateChanged(EGameState GameState)
        {
            switch (GameState)
            {
                case EGameState.STAND_BY:


                    break;
                case EGameState.STARTED:


                    break;
                case EGameState.WIN:
                    SoundManager.Instance.Play(CommonTypes.SOUND_WIN);

                    break;
                case EGameState.LOSE:
                    SoundManager.Instance.Play(CommonTypes.SOUND_LOSE);
                    break;
            }
        }

        /// <summary>
        /// This function returns related game state.
        /// </summary>
        /// <returns></returns>
        public EGameState GetGameState()
        {
            return gameState;
        }

        /// <summary>
        /// This function returns related player view component.
        /// </summary>
        /// <returns></returns>
        public PlayerView GetPlayerView()
        {
            return m_playerView;
        }

        /// <summary>
        /// This function returns related game settings.
        /// </summary>
        /// <returns></returns>
        public GameSettings GetGameSettings()
        {
            return m_gameSettings;
        }

        /// <summary>
        /// This Function Helper For Set Currency.
        /// </summary>
        /// <param name="currency"></param>
        public void SetCurrency(int currency)
        {
            int Currency = PlayerPrefs.GetInt(CommonTypes.CURRENCY_DATA_KEY) + currency;
            PlayerPrefs.SetInt(CommonTypes.CURRENCY_DATA_KEY, Currency);
            PlayerPrefs.Save();
            UICheck();
        }

        public void UICheck()
        {
            foreach (var UpgradeUI in UpgradeUIController.Instance.GetUpgradeUIs())
            {
                UpgradeUI.UIChangeControl();
            }
        }

        /// <summary>
        /// This Function Returns Related Currency.
        /// </summary>
        /// <returns></returns>
        public int GetCurreny()
        {
            return PlayerPrefs.GetInt(CommonTypes.CURRENCY_DATA_KEY);
        }

        public void NextLevel()
        {
            LevelService.NextLevel();
        }

        public void RetryLevel()
        {
            LevelService.RetryLevel();
        }

        public void DestroyObj(GameObject obj)
        {
            Destroy(obj);
        }
    }
}