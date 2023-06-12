using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Emir;
using UnityEngine;

public class EnemyBombComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private ParticleSystem TrailParticle;
    [SerializeField] private ParticleSystem ExplasionParticle;

    #endregion


    public void JumpTarget(float Damage, Vector3 TargetPos)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMove(TargetPos,
            1.5f));
        sequence.OnStart(() => { GetTrailParticle().Play(); });
        sequence.OnComplete(() =>
        {
            GetExplosionParticle().Play();
            CameraManager.Instance.CameraShake();
            GameManager.Instance.GetPlayerView().HealthDecrease(Damage);
            GameManager.Instance.GetPlayerView().SliderUpdated();
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
}