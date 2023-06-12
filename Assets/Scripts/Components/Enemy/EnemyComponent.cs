using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EEnemyType EnemyType;

    [Header("References")] [SerializeField]
    private Animator _animator;

    [SerializeField] private SkinnedMeshRenderer Renderer;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private ParticleSystem _deadparticle;
    [SerializeField] private ParticleSystem fireparticle;
    [SerializeField] private Transform SliderRoot;
    [SerializeField] private Slider _slider;


    [Header("Values")] [SerializeField] private float Health;
    [SerializeField] private float Damage;
    [SerializeField] private float Speed;
    [SerializeField] private float AttackDelay;
    [SerializeField] private float AttackRadius;

    #endregion

    #region Private Fields

    private bool IsDeath = false;
    private bool IsAttack = false;
    private bool IsFreezeSkill = false;
    private bool IsFireSkill = false;
    private float IsAttackTimer = 0;
    private float HeadShotValue = 1;
    private float originalSpeed;
    private float originalAnimSpeed;
    private bool IsCloneAttackEvent = false;

    #endregion

    private void Start()
    {
        SliderLookCamera();
        AgentInitialized();
        SliderInit();
        originalSpeed = Speed;
        originalAnimSpeed = GetAnimator().speed;
    }

    private void Update()
    {
        MovePlayer();
        AttackStateControl();
        AttackDistanceControl();
    }


    private void MovePlayer()
    {
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (IsDeath is true) return;

        #region Walk Animation Geçiş

        if (GameManager.Instance.GetGameState() == EGameState.STARTED)
        {
            GetAnimator().SetTrigger("Walk");
        }

        #endregion

        Vector3 TargetPos = new Vector3(GameManager.Instance.GetPlayerView().transform.position.x, 0,
            GameManager.Instance.GetPlayerView().transform.position.z);
        _agent.destination = TargetPos;

        SliderLookCamera();
    }

    private void SliderLookCamera()
    {
        SliderRoot.transform.LookAt(CameraManager.Instance.GetVirtualCamera().transform);
    }


    private void AgentInitialized()
    {
        GetAgent().speed = Speed;
    }

    private void SliderInit()
    {
        GetSlider().minValue = 0;
        GetSlider().maxValue = Health;
        GetSlider().value = Health;
    }

    public void SliderUpdated()
    {
        if (IsDeath is true) return;
        _slider.DOValue(Health, 0.25f);
    }

    private void HeadShotEvent()
    {
        //head shot efekti çıkacak sliderda
    }

    public void Damages(float DamageValue)
    {
        if (IsDeath is true) return;
        if (SkillComponent.Instance.GetIsHeadShotSkill() is true)
        {
            int RandomShot = Random.Range(1, 100);
            if (RandomShot < 50)
            {
                HeadShotValue = 25;
                HeadShotEvent();
                HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);
            }
            else
            {
                HeadShotValue = 1;
                HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
            }
        }
        else
        {
            HeadShotValue = 1;
            HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
        }

        FreezeSkill();
        FireSkill();
        Health -= DamageValue * GameManager.Instance.GetPlayerView().GetDamageMultiply() * HeadShotValue;
        int PopUpValue = (int)(DamageValue * GameManager.Instance.GetPlayerView().GetDamageMultiply() * HeadShotValue);
        InterfaceManager.Instance.DamageNumberPopUp(transform.position + Vector3.up * 2,
            PopUpValue);
        IsDeathControl();
    }

    private void IsDeathControl()
    {
        if (IsDeath is true) return;
        if (Health <= 0)
        {
            IsDeath = true;
            GetAgent().enabled = false;
            GameManager.Instance.SetCurrency(20);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            GetAnimator().SetTrigger("Dead");
            LevelComponent.Instance.GetEnemys().Remove(this);
            LevelComponent.Instance.GetWaveEnemys().Remove(this);
            SliderComponent.Instance.SetSliderValue(1);
            SliderComponent.Instance.SliderUpdated();
            GameManager.Instance.GetPlayerView().HealtIncreaseByKillEnemy();
            LevelComponent.Instance.EnemyLevelControl();
            LevelComponent.Instance.EnemyWaveControl();
            HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);

            DOVirtual.DelayedCall(0.25f, () => { SliderRoot.gameObject.SetActive(false); });
            DOVirtual.DelayedCall(2f, () => { GetDeadParticle().Play(); });

            Renderer.material.DOColor(Color.gray, 1f);

            DOVirtual.DelayedCall(4f, () => { Destroy(this.gameObject); });
            if (SkillComponent.Instance.GetIsLifeSteal())
            {
                GameManager.Instance.GetPlayerView().SetHealthIncrease(5);
                GameManager.Instance.GetPlayerView().SliderUpdated();
            }

            if (SkillComponent.Instance.GetIsSmartSkill())
            {
                GameManager.Instance.SetCurrency(10);
                InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            }
        }
    }

    public void PlayParticle()
    {
        GetParticle().Play();
    }

    public void AttackDistanceControl()
    {
        float Distance = Vector3.Distance(new Vector3(GameManager.Instance.GetPlayerView().transform.position.x, 0,
            GameManager.Instance.GetPlayerView().transform.position.z), transform.position);
        if (Distance < AttackRadius)
        {
            if (IsAttack is true) return;
            IsAttack = true;
            IsAttackTimer = Time.time + AttackDelay;
            GetAnimator().SetTrigger("Attack");
            GetAgent().speed = 0;
        }
    }

    public void Attack(Collider other)
    {
        if (other.gameObject.CompareTag("Fence"))
        {
            if (IsAttack is true) return;
            IsAttack = true;
            IsAttackTimer = Time.time + AttackDelay;
            GetAnimator().SetTrigger("Attack");
            GetAgent().speed = 0;
        }
    }

    private void AttackEvent()
    {
        if (IsCloneAttackEvent is true) return;
        IsCloneAttackEvent = true;
        CameraManager.Instance.CameraShake();
        GameManager.Instance.GetPlayerView().HealthDecrease(GetDamage());
        GameManager.Instance.GetPlayerView().SliderUpdated();
        Invoke("ReturnIsCloneAttackEvent", 0.25f);
    }

    private void ReturnIsCloneAttackEvent()
    {
        IsCloneAttackEvent = false;
    }

    private void BomberAttackEvent()
    {
        EnemyBombComponent Bomb = Instantiate(GameManager.Instance.GetGameSettings().BombPrefab);
        Bomb.transform.position = transform.position;
        Bomb.JumpTarget(GetDamage(), GameManager.Instance.GetPlayerView().transform.position + Vector3.up);
    }

    private void AttackStateControl()
    {
        if (IsAttack is true)
        {
            if (Time.time > IsAttackTimer)
            {
                IsAttack = false;
            }
        }
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     Attack(other);
    //     if (other.gameObject.CompareTag("Bomb"))
    //     {
    //         if (other.TryGetComponent<BombComponent>(out BombComponent Bomb))
    //         {
    //             if (Bomb.GetIsExplosion() == true) return;
    //             if (Bomb.GetIsFinish() is true) return;
    //             DOVirtual.DelayedCall(0.05f, () =>
    //             {
    //                 Damages(Bomb.GetCurrentDamage());
    //                 SliderUpdated();
    //             });
    //             //Bomb.SetIsFinish(true);
    //         }
    //     }
    // }

    private void FireSkill()
    {
        if (IsFireSkill is true) return;
        if (SkillComponent.Instance.GetIsFireShotSkill() is true)
        {
            int randomvalue = Random.Range(0, 100);
            if (randomvalue < 50)
            {
                IsFireSkill = true;
                FireDamage();
            }
        }
    }

    private async void FireDamage()
    {
        int Counter = 0;
        fireparticle.Play();
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            Damages(3);
            SliderUpdated();
            Counter++;
            if (Counter >= 5)
            {
                fireparticle.Stop();
                IsFireSkill = false;
                break;
            }
        }
    }

    private void FreezeSkill()
    {
        if (IsFreezeSkill is true) return;
        if (SkillComponent.Instance.GetIsFreezeShotSkill() is true)
        {
            int randomvalue = Random.Range(0, 100);
            if (randomvalue < 30)
            {
                _agent.speed = 0;
                GetAnimator().speed = 0;
                Invoke("ResetSpeed", 3f);
                IsFreezeSkill = true;
            }
        }
    }

    void ResetSpeed()
    {
        _agent.speed = originalSpeed; // orijinal hıza geri dön
        GetAnimator().speed = originalAnimSpeed;
        IsFreezeSkill = false;
    }

    #region Getters

    public EEnemyType GetEnemyType()
    {
        return EnemyType;
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public NavMeshAgent GetAgent()
    {
        return _agent;
    }

    public ParticleSystem GetParticle()
    {
        return _particle;
    }

    public ParticleSystem GetDeadParticle()
    {
        return _deadparticle;
    }

    public Transform GetSliderRoot()
    {
        return SliderRoot;
    }

    public Slider GetSlider()
    {
        return _slider;
    }

    public float GetDamage()
    {
        return Damage;
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public float GetHealth()
    {
        return Health;
    }

    #endregion

    #region Setters

    #endregion
}