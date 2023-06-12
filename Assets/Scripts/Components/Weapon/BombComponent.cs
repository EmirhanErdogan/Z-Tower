using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using UnityEngine;

public class BombComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private ParticleSystem TrailParticle;
    [SerializeField] private ParticleSystem ExplasionParticle;
    [SerializeField] private Collider Coll;

    #endregion

    #region Private Fields

    private bool IsExplosion = true;
    private bool ColliderActive = false;
    private bool IsActive = false;
    private float Timer = 0;
    private float CurrentDamage;
    private bool IsFinish = false;

    #endregion


    public void JumpTarget(float Damage, Vector3 TargetPos, float speed)
    {
        // Gidilecek mesafeyi hesapla
        float distance = Vector3.Distance(transform.position, TargetPos);


        // Hareket süresini hesapla
        float time = distance / speed;

        // Hareketi başlat
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMove(TargetPos, time).SetEase(Ease.InSine));
        sequence.OnStart(() => { GetTrailParticle().Play(); });
        sequence.OnComplete(() =>
        {
            IsExplosion = false;
            GetExplosionParticle().Play();
            GetTrailParticle().Stop();
            CameraManager.Instance.CameraShake();
            Coll.enabled = true;
            ColliderActive = true;
            DOVirtual.DelayedCall(0.01f, () => { IsExplosion = true; });
        });
        sequence.Play();
    }


    #region Getters

    public ParticleSystem GetTrailParticle()
    {
        return TrailParticle;
    }

    public ParticleSystem GetExplosionParticle()
    {
        return ExplasionParticle;
    }

    #endregion

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

    #region Active

    public bool GetIsActive()
    {
        return IsActive;
    }

    public void SetIsActive(bool value)
    {
        IsActive = value;
    }

    public void SetCurrentDamage(float Damage)
    {
        CurrentDamage = Damage;
    }

    public float GetCurrentDamage()
    {
        return CurrentDamage;
    }

    public void SetIsExplosion(bool value)
    {
        IsExplosion = value;
    }

    public bool GetIsExplosion()
    {
        return IsExplosion;
    }

    public void SetIsFinish(bool value)
    {
        IsFinish = value;
    }

    public bool GetIsFinish()
    {
        return IsFinish;
    }

    #endregion
}