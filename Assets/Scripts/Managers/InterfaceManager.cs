using TMPro;
using System;
using DamageNumbersPro;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace Emir
{
    public class InterfaceManager : Singleton<InterfaceManager>
    {
        #region Serializable Fields

        [Header("Transforms")] [SerializeField]
        private RectTransform m_canvas;

        [SerializeField] private RectTransform m_Touchcanvas;
        [SerializeField] private RectTransform m_currencySlot;
        [SerializeField] private DamageNumber numberPrefab;

        [Header("Panels")]
        // [SerializeField] private UIWinPanel m_winPanel;
        // [SerializeField] private UILosePanel m_losePanel;
        [Header("Texts")]
        [SerializeField]
        private TextMeshProUGUI m_currencyText;

        [SerializeField] private TextMeshProUGUI m_levelText;

        [SerializeField] private TextMeshProUGUI WaveCompleteText;

        [Header("Canvas Groups")] [SerializeField]
        private CanvasGroup m_menuCanvasGroup;

        [SerializeField] private CanvasGroup m_gameCanvasGroup;
        [SerializeField] private CanvasGroup m_commonCanvasGroup;
        [SerializeField] private CanvasGroup SettingsGroup;
        [SerializeField] private CanvasGroup MenuGroup;
        [SerializeField] private CanvasGroup WinGroup;
        [SerializeField] private CanvasGroup LoseGroup;
        [SerializeField] private GameObject SettingsIcon;

        [Header("Prefabs")] [SerializeField] private RectTransform m_currencyPrefab;

        #endregion

        #region Private Fields

        private int CurrentCurrency;
        private bool SettingsBool = true;

        #endregion

        /// <summary>
        /// Awake.
        /// </summary>
        private void Start()
        {
            DOVirtual.DelayedCall(0.15f, () =>
            {
                CurrentCurrency = GameManager.Instance.GetCurreny();
                OnGameStateChanged(GameManager.Instance.GetGameState());
                OnPlayerCurrencyUpdated();
            });
        }


        public void DamageNumberPopUp(Vector3 Position, float NumberValue)
        {
            DamageNumber damageNumber = numberPrefab.Spawn(Position, NumberValue);
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void FlyCurrencyFromWorld(Vector3 worldPosition)
        {
            Camera targetCamera = CameraManager.Instance.GetCamera();
            Vector3 screenPosition = GameUtils.WorldToCanvasPosition(m_canvas, targetCamera, worldPosition);
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.anchoredPosition = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.5F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="screenPosition"></param>
        public void FlyCurrencyFromScreen(Vector3 screenPosition)
        {
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.position = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.5F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function called when game state changed.
        /// </summary>
        /// <param name="e"></param>
        public void OnGameStateChanged(EGameState GameState)
        {
            switch (GameState)
            {
                case EGameState.STAND_BY:

                    SettingsIcon.SetActive(true);
                    WeaponUIController.Instance.GetWeaponIcon().gameObject.SetActive(true);
                    UpgradeUIController.Instance.GetUpgaradeIcon().gameObject.SetActive(true);
                    LevelTextUpdated();
                    break;
                case EGameState.STARTED:

                    GameUtils.SwitchCanvasGroup(m_menuCanvasGroup, m_gameCanvasGroup);

                    break;
                case EGameState.WIN:

                    // m_winPanel.Initialize();
                    // m_Touchcanvas.gameObject.SetActive(false);
                    GameUtils.SwitchCanvasGroup(null, WinGroup);

                    break;
                case EGameState.LOSE:

                    // m_losePanel.Initialize();
                    // m_Touchcanvas.gameObject.SetActive(false);
                    GameUtils.SwitchCanvasGroup(null, LoseGroup);

                    break;
            }
        }

        public void LevelTextUpdated()
        {
            m_levelText.text = string.Format("LEVEL {0}", LevelService.GetCachedLevel(1));
        }

        public void WaveTextUpdated(int value)
        {
            WaveCompleteText.gameObject.transform.DOScale(0, 0.1f);
            WaveCompleteText.text = string.Format("Wave {0} Completed", value);
            WaveCompleteText.gameObject.SetActive(true);
            WaveCompleteText.gameObject.transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
            {
                WaveCompleteText.gameObject.transform.DOScale(0, 0.5f).OnComplete(() =>
                {
                    WaveCompleteText.gameObject.SetActive(false);
                });
            });
        }

        /// <summary>
        /// This function called when player currency updated.
        /// </summary>
        /// <param name="e"></param>
        public void OnPlayerCurrencyUpdated()
        {
            string currencyText = m_currencyText.text;

            currencyText = currencyText.Replace(".", String.Empty);
            currencyText = currencyText.Replace(",", String.Empty);
            currencyText = currencyText.Replace("$", String.Empty);
            currencyText = currencyText.Replace("K", String.Empty);
            currencyText = currencyText.Replace("M", String.Empty);
            currencyText = currencyText.Replace("#", String.Empty);

            int cachedCurrency = CurrentCurrency;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => cachedCurrency, x => cachedCurrency = x, GameManager.Instance.GetCurreny(),
                CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION));

            sequence.OnUpdate(() => { m_currencyText.text = cachedCurrency.ConvertMoney().ToString(); });
            sequence.OnComplete(() => { CurrentCurrency = GameManager.Instance.GetCurreny(); });
            sequence.SetId(m_currencyText.GetInstanceID());
            sequence.Play();
        }

        /// <summary>
        /// This function helper for change settings panel state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeSettingsPanelState()
        {
            // Debug.Log("dawdawd");
            // if (DOTween.IsTweening(GetSettingsGroup().GetInstanceID()))
            //     return;

            Sequence sequence = DOTween.Sequence();


            sequence.Join(SettingsComponent.Instance.GetMaskObj().DOLocalMoveY(SettingsBool ? 9.75f : 290f, 0.25f));
            sequence.OnStart(() =>
            {
                SettingsBool = !SettingsBool;
                if (SettingsBool)
                {
                    SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
                    HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                }
            });
            // sequence.OnComplete(() => { GetSettingsGroup().blocksRaycasts = SettingsBool; });
            // sequence.SetId(GetSettingsGroup().GetInstanceID());
            sequence.Play();
        }

        public CanvasGroup GetSettingsGroup()
        {
            return SettingsGroup;
        }

        public RectTransform GetGameCanvas()
        {
            return m_Touchcanvas;
        }

        public CanvasGroup GetMenuCanvas()
        {
            return m_menuCanvasGroup;
        }


        public CanvasGroup GetMenuCanvasGroup()
        {
            return MenuGroup;
        }
    }
}