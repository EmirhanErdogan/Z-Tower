using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Emir;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Transform SpineRoot;
    [SerializeField] private WeaponComponent CurrentWeapon;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI HealthPercantageText;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float Health;
    [SerializeField] private float MaxHealth;
    [SerializeField] private float DamageMultiply = 1;
    [SerializeField] private float DefenceMultiply = 1;
    [SerializeField] private Vector3 TouchOffset;
    [SerializeField] private Image CrossHair;
    [SerializeField] private float CrosshairSensitivity = 0.5f;

    #endregion


    #region Private Fields

    private Vector2 fingerDownPosition;
    private Vector2 fingerLastPosition;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 TargetLook = Vector3.zero;
    private bool IsAttack = false;
    private bool IsDeath = false;
    private bool IsRageActive = false;
    private bool IsGlory = false;
    private float AttackTimer = 0;
    private float CriticialDamage = 0;
    private float CriticialChange = 0;

    #endregion

    private void Start()
    {
        AttackTimer = Time.time + GetWeapon().GetFireRate();
        SliderInitialized();
        UpgradeInitialize();
    }

    #region UpgradeValueInit

    private void UpgradeInitialize()
    {
        UpgradeHealthValueInit();
        UpgradeDamageMultiplyInitialize();
        UpgradeDefenceMultiplyInitialize();
    }

    private void UpgradeHealthValueInit()
    {
        if (UpgradeUIController.Instance.GetUpgradeUIs().First(x => x.GetData().Upgrade == EUpgradeType.MAXHP).GetData()
                .Level == 0)
        {
            //level 0 oluğundan 100 olacak
        }
        else
        {
            //uprade level işlenecek
            UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
                .First(x => x.GetData().Upgrade == EUpgradeType.MAXHP).GetData();
            Health = Data.Values[Data.Level];
            MaxHealth = Data.Values[Data.Level];
        }

        SliderUpdated();
    }

    private void UpgradeDamageMultiplyInitialize()
    {
        UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
            .First(x => x.GetData().Upgrade == EUpgradeType.DAMAGE).GetData();
        DamageMultiply = Data.Values[Data.Level];
    }

    private void UpgradeDefenceMultiplyInitialize()
    {
        UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
            .First(x => x.GetData().Upgrade == EUpgradeType.DEFENCE).GetData();
        DefenceMultiply = Data.Values[Data.Level];
    }

    #endregion

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (IsDeath is true) return;
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (Time.time > AttackTimer)
        {
            IsAttack = false;
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                SpineRotate(touch);
                CrossHair.enabled = true;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                SpineRotate(touch);
                Fire();
            }

            if (touch.phase == TouchPhase.Stationary)
            {
                Fire();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                CrossHair.enabled = false;
            }
        }
        
    }

    private void HealtIncreaseByWawe()
    {
        UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
            .First(x => x.GetData().Upgrade == EUpgradeType.HEALWHENLEVELUP).GetData();
        Health += Data.Values[Data.Level];
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        SliderUpdated();
    }

    public void HealtIncreaseByKillEnemy()
    {
        UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
            .First(x => x.GetData().Upgrade == EUpgradeType.HEARTDROPRATE).GetData();
        Health += Data.Values[Data.Level];
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        SliderUpdated();
    }

    #region Events

    private void Fire()
    {
        if (IsDeath is true) return;
        if (IsAttack is true) return;
        IsAttack = true;
        AttackTimer = Time.time + GetWeapon().GetFireRate();
        GetWeapon().Fire();
    }

    private void SpineRotate(Touch touch)
    {
        if (IsDeath is true) return;

        ray = Camera.main.ScreenPointToRay((Vector3)touch.position + TouchOffset);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            float zOffset = TouchOffset.z * (hit.point.z - SpineRoot.position.z);
            Vector3 offset = new Vector3(TouchOffset.x, TouchOffset.y, zOffset);
            TargetLook = hit.point + offset + Vector3.up * 0.005f;

            // Crosshair'in yeni konumunu hesaplayın
            Vector2 newCrosshairPosition = GameUtils.WorldToCanvasPosition(
                InterfaceManager.Instance.GetGameCanvas(), CameraManager.Instance.GetCamera(), TargetLook);

            // Sensitivity'yi uygulayın
            Vector2 currentCrosshairPosition = CrossHair.rectTransform.anchoredPosition;
            Vector2 smoothedCrosshairPosition =
                Vector2.Lerp(currentCrosshairPosition, newCrosshairPosition, CrosshairSensitivity);

            CrossHair.rectTransform.anchoredPosition = smoothedCrosshairPosition;
        }

        SpineRoot.transform.rotation = Quaternion.LookRotation(TargetLook - SpineRoot.position);
    }

    public void SliderInitialized()
    {
        _slider.minValue = 0;
        _slider.maxValue = MaxHealth;
        _slider.value = Health;
        float Percantage = GameUtils.CalculateHealthPercentage(Health, MaxHealth);
        HealthPercantageText.text = String.Format("%{0}", Percantage);
    }

    public void SliderUpdated()
    {
        _slider.DOValue(Health, 0.5f);
        float Percantage = GameUtils.CalculateHealthPercentage(Health, MaxHealth);
        HealthPercantageText.text = String.Format("%{0}", Percantage);
    }

    #endregion

    #region Getters

    public Image GetCrossHair()
    {
        return CrossHair;
    }

    public bool GetIsGlory()
    {
        if (PlayerPrefs.GetInt(CommonTypes.IS_GLORY_SKILL) == 0)
        {
            IsGlory = false;
        }
        else
        {
            IsGlory = true;
        }

        return IsGlory;
    }

    public float GetIsCriticialDamageMultiplySkill()
    {
        return CriticialDamage;
    }

    public float GetIsCriticialChangeMultiplySkill()
    {
        return CriticialChange;
    }

    public float GetDamageMultiply()
    {
        return DamageMultiply;
    }

    public float GetHealth()
    {
        return Health;
    }

    public Vector3 GetTargetPos()
    {
        return TargetLook;
    }

    public WeaponComponent GetWeapon()
    {
        return CurrentWeapon;
    }

    public void CriticialDamageChangeIncrease()
    {
        CriticialDamage = 0.05f;
        CriticialChange = 5;
    }

    #endregion

    #region Setters

    public void SetIsGlory()
    {
        IsGlory = true;
        PlayerPrefs.SetInt(CommonTypes.IS_GLORY_SKILL, 1);
    }

    public void SetDamageMultiply(float Value)
    {
        DamageMultiply += Value;
    }

    public void SetMaxHealthIncrease(float value)
    {
        MaxHealth += value;
        SliderInitialized();
    }

    public void SetHealthIncrease(float Value)
    {
        Health += Value;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        SliderUpdated();
    }

    public void HealthDecrease(float Damage)
    {
        if (IsDeath is true) return;
        HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);
        Damage -= Damage * DefenceMultiply;
        Health -= Damage;
        PlayerDeadControl();
    }

    public async void PlayerDeadControl()
    {
        float Percantage = (Health * 100) / MaxHealth;
        if (Percantage < 35)
        {
            //rage Skill devreye girecek
            if (SkillComponent.Instance.GetIsRageSkill() is true)
            {
                //damage katlanacak
                SetDamageMultiply(1);
                IsRageActive = true;
            }
        }
        else
        {
            //damage eski hale gelecek
            if (SkillComponent.Instance.GetIsRageSkill() is true)
            {
                if (IsRageActive is true)
                {
                    SetDamageMultiply(-1);
                }
            }
        }

        if (Health <= 0)
        {
            IsDeath = true;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            GetCrossHair().enabled = false;
            GameManager.Instance.ChangeGameState(EGameState.LOSE);
            GameManager.Instance.OnGameStateChanged(EGameState.LOSE);
            InterfaceManager.Instance.OnGameStateChanged(EGameState.LOSE);
            GameManager.Instance.SetCurrency(100);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
        }
    }

    public void SetWeapon(WeaponComponent TargetWeapon)
    {
        CurrentWeapon = TargetWeapon;
    }

    #endregion
}