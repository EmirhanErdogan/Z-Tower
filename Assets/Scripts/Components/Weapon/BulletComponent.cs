using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Emir;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private ParticleSystem BulletParticle;
    [SerializeField] private Rigidbody _rigidbody;

    #endregion

    #region Private Fields

    private bool IsActive = false;
    private bool IsTrigger = false;
    private bool IsSekti = false;
    private float Timer = 0;
    private float CurrentDamage = 0;
    private float CriticialDamage = 1;
    private EnemyComponent TriggerEnemy = null;

    #endregion

    private void Update()
    {
        BulletTimeControl();
    }


    #region Destroy Bullet

    public void BulletTimeInit()
    {
        Timer = Time.time + GameManager.Instance.GetGameSettings().BulletDestroyTime;
    }

    private void BulletTimeControl()
    {
        if (GetIsActive() is false) return;
        if (Time.time > Timer)
        {
            //destroy Bullet
            SetIsActive(false);
            DestroyBullet();
        }
    }

    private void DestroyBullet(float Delay = 0.75f)
    {
        DOVirtual.DelayedCall(Delay, () => { Destroy(this.gameObject); });
    }

    #endregion

    #region Getters

    public float GetCurrentDamage()
    {
        return CurrentDamage;
    }

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public bool GetIsActive()
    {
        return IsActive;
    }

    public void PlayBulletParticle()
    {
        if (BulletParticle is null) return;
        BulletParticle.Play();
    }

    #endregion

    #region Setters

    public void SetCurrentDamage(float Damage)
    {
        CurrentDamage = Damage;
    }

    public void SetIsActive(bool value)
    {
        IsActive = value;
    }

    #endregion

    #region Criticial Damage

    private void CriticialChange()
    {
        float Change = Random.Range(0f, 100f);
        UpgradeUIScriptable Data = UpgradeUIController.Instance.GetUpgradeUIs()
            .First(x => x.GetData().Upgrade == EUpgradeType.CRITCHANGE).GetData();
        if (Change <= (Data.Values[Data.Level] +
                       GameManager.Instance.GetPlayerView().GetIsCriticialChangeMultiplySkill()))
        {
            UpgradeUIScriptable Data2 = UpgradeUIController.Instance.GetUpgradeUIs()
                .First(x => x.GetData().Upgrade == EUpgradeType.CRITDAMAGE).GetData();
            CriticialDamage = Data2.Values[Data.Level] +
                              GameManager.Instance.GetPlayerView().GetIsCriticialDamageMultiplySkill();
            Debug.Log("cRİTİCİAL Damage");
        }
        else
        {
            CriticialDamage = 1;
        }
    }

    #endregion

    private void ZombieTrigger(Collider other)
    {
        if (other.gameObject.CompareTag(CommonTypes.ZOMBIE_TAG))
        {
            if (IsTrigger is true) return;

            EnemyComponent TargetEnemy =
                LevelComponent.Instance.GetEnemys().FirstOrDefault(x => x.gameObject == other.gameObject);
            if (TargetEnemy is not null)
            {
                TriggerEnemy = TargetEnemy;

                GetRigidbody().velocity = Vector3.zero;
                CriticialChange();
                TargetEnemy.Damages(GetCurrentDamage() * CriticialDamage);

                TargetEnemy.PlayParticle();
                TargetEnemy.SliderUpdated();
                if (SkillComponent.Instance.GetIsRicochetSkill() is true && IsSekti == false)
                {
                    Ricochet(TargetEnemy);
                    IsSekti = true;
                    return;
                }

                if (SkillComponent.Instance.GetIsPiercingSkill() is true ||
                    SkillComponent.Instance.GetIsRicochetSkill() is true) return;
                DestroyBullet(0.01f);
                IsTrigger = true;
            }
        }
    }

    private void GroundTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            DestroyBullet(0);
        }
    }

    private bool Ricochet(EnemyComponent TargetEnemyy)
    {
        //if (SkillComponent.Instance.GetIsRicochetSkill() is true) return false;
        if (LevelComponent.Instance.GetWaveEnemys().Count < 1 ||
            LevelComponent.Instance.GetEnemys().Count < 1) return false;
        int RandomValue = Random.Range(0, 100);
        if (RandomValue < 100)
        {
            Debug.Log("asdasdaricichet");
            //ricochet vuruş yapacak
            GetRigidbody().velocity = Vector3.zero;
            DOVirtual.DelayedCall(0.5f, () => { SetIsActive(false); });
            SetIsActive(false);
            PlayBulletParticle();
            float minDistance = Mathf.Infinity;
            EnemyComponent TargetEnemy = null;
            foreach (var Enemy in LevelComponent.Instance.GetEnemys())
            {
                float distance = Vector3.Distance(Enemy.transform.position, transform.position);
                if (distance < minDistance && Enemy != TargetEnemyy)
                {
                    minDistance = distance;
                    TargetEnemy = Enemy;
                }
            }

            transform.SetParent(null);
            if (TriggerEnemy is not null)
            {
                transform.position = TriggerEnemy.transform.position + Vector3.up;
            }

            Vector3 Direction = (TargetEnemy.transform.position + Vector3.up * 0.75f) - transform.position;
            transform.rotation = Quaternion.LookRotation(Direction);
            GetRigidbody().AddForce(transform.forward *
                                    GameManager.Instance.GetPlayerView().GetWeapon().GetSpeed() * 1.3f);
            BulletTimeInit();

            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZombieTrigger(other);
        GroundTrigger(other);
    }
}