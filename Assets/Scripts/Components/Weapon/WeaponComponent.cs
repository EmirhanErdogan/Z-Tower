using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.DemiLib;
using DG.Tweening;
using Emir;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponComponent : MonoBehaviour
{
    #region Serializable Fields

    [Header("Type")] [SerializeField] private EWeaponType _weaponType;
    [Header("Values")] [SerializeField] private float Range;
    [SerializeField] private float Damage;
    [SerializeField] private float Speed;
    [SerializeField] private float FireRate;

    [Header("References")] [SerializeField]
    private Transform BulletRoot;

    [SerializeField] private Transform BulletRoot2;
    [SerializeField] private Transform BulletRoot3;

    [SerializeField] private ParticleSystem WeaponParticle;

    #endregion

    #region PrivateFields

    private LevelComponent _levelComponent => LevelComponent.Instance;
    private GameSettings _gameSettings => GameManager.Instance.GetGameSettings();
    private PlayerView _player => GameManager.Instance.GetPlayerView();
    private int ShotCount = 1;

    #endregion


    public async void Fire()
    {
        if (LevelComponent.Instance.GetBullets().Count <= 10)
        {
            LevelComponent.Instance.CreateBullet();
            await UniTask.Delay(TimeSpan.FromSeconds(0.11f));
        }

       
        int Shotvalue = 0;
        while (true)
        {
            #region GrenadeLauncher

            if (GetWeaponType() == EWeaponType.GRENADELAUNCHER)
            {
                BombComponent Bomb = Instantiate(GameManager.Instance.GetGameSettings().PlayerBombPrefab);
                Bomb.transform.position = BulletRoot.position;
                Vector3 DirectionBomb = _player.GetTargetPos() - Bomb.transform.position;
                Bomb.transform.rotation = Quaternion.LookRotation(DirectionBomb);
                Bomb.transform.SetParent(null);
                Bomb.SetIsActive(true);
                Bomb.BulletTimeInit();
                Bomb.SetCurrentDamage(GetDamage());
                PlayWeaponParticle();
                Vector3 TargetPos = GameManager.Instance.GetPlayerView().GetTargetPos() + Vector3.down * 1f;
                Bomb.JumpTarget(GetDamage(),
                    GameManager.Instance.GetPlayerView().GetTargetPos(),GetSpeed());
                return;
            }

            #endregion

            BulletComponent Bullet = LevelComponent.Instance.GetBullet();
            Bullet.transform.position = BulletRoot.transform.position;
            PlayWeaponParticle();
            Bullet.PlayBulletParticle();
            Vector3 Direction = (_player.GetTargetPos() + Vector3.up * 0.005f) - Bullet.transform.position;
            Bullet.transform.rotation = Quaternion.LookRotation(Direction);
            BulletRootLookTarget();

            #region Shotgun

            if (GetWeaponType() == EWeaponType.SHOTGUN)
            {
                BulletComponent Bullet2 = LevelComponent.Instance.GetBullet();
                BulletComponent Bullet3 = LevelComponent.Instance.GetBullet();
                Bullet2.transform.position = BulletRoot.transform.position;
                Bullet3.transform.position = BulletRoot.transform.position;
                Bullet2.PlayBulletParticle();
                Bullet3.PlayBulletParticle();
                // Vector3 Bullet2Direction =
                //     (_player.GetTargetPos() + Vector3.up + Vector3.right * 2) - Bullet.transform.position;
                // Vector3 Bullet3Direction =
                //     (_player.GetTargetPos() + Vector3.up + Vector3.left * 2) - Bullet.transform.position;

                Bullet2.transform.rotation = BulletRoot2.transform.rotation;
                Bullet3.transform.rotation = BulletRoot3.transform.rotation;
                Bullet2.transform.SetParent(null);
                Bullet3.transform.SetParent(null);
                Bullet2.SetIsActive(true);
                Bullet3.SetIsActive(true);
                Bullet2.GetRigidbody().AddForce(Bullet2.transform.forward * GetSpeed());
                Bullet3.GetRigidbody().AddForce(Bullet3.transform.forward * GetSpeed());
                Bullet2.BulletTimeInit();
                Bullet3.BulletTimeInit();
            }

            #endregion


            Bullet.transform.SetParent(null);
            Bullet.SetIsActive(true);
            Bullet.GetRigidbody().AddForce(Bullet.transform.forward * GetSpeed());
            Bullet.BulletTimeInit();
            Bullet.SetCurrentDamage(GetDamage());
            SoundManager.Instance.Play(GetWeaponType().ToString());

            Shotvalue++;
            if (Shotvalue >= ShotCount)
            {
                Shotvalue = 0;
                break;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
        }
    }

   

    private void BulletRootLookTarget()
    {
        Vector3 Direction = _player.GetTargetPos() - BulletRoot.position;
        BulletRoot.rotation = Quaternion.LookRotation(Direction);
    }

    #region Setters

    public void SetShotCount(int value)
    {
        ShotCount = value;
    }

    public void GetWeaponType(EWeaponType WeaponType)
    {
        _weaponType = WeaponType;
    }

    public void GetRange(float Value)
    {
        Range = Value;
    }

    public void GetDamage(float Value)
    {
        Damage = Value;
    }

    public void GetFireRate(float Value)
    {
        FireRate = Value;
    }

    #endregion

    #region Getters

    public EWeaponType GetWeaponType()
    {
        return _weaponType;
    }

    public float GetRange()
    {
        return Range;
    }

    public float GetDamage()
    {
        return Damage;
    }

    public void SetFireRate(float value)
    {
        FireRate -= value;
    }

    public float GetFireRate()
    {
        return FireRate;
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public void PlayWeaponParticle()
    {
        if (WeaponParticle is null) return;
        WeaponParticle.Play();
    }

    #endregion
}