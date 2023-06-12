using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelComponent : Singleton<LevelComponent>
{
    #region Serializable Fields

    [SerializeField] private Transform BulletPool;
    [SerializeField] private List<BulletComponent> Bullets = new List<BulletComponent>();
    [SerializeField] private List<EnemyComponent> Enemys = new List<EnemyComponent>();
    [SerializeField] private List<EWeaponType> ActiveWeaponList = new List<EWeaponType>();
    [SerializeField] private List<WeaponUIScriptable> WeaponDatas = new List<WeaponUIScriptable>();
    [SerializeField] private List<UIUpgradeItem> UpgradeItems = new List<UIUpgradeItem>();
    [SerializeField] private List<WaveComponent> Waves = new List<WaveComponent>();
    [SerializeField] private List<Transform> CreatedRoots = new List<Transform>();
    [SerializeField] private List<EnemyComponent> CurrentWaveEnemys = new List<EnemyComponent>();
    [SerializeField] private Transform EnemyRoot;

    #endregion

    #region Private Fields

    private GameSettings _gameSettings => GameManager.Instance.GetGameSettings();
    private int CurrentIndex = 0;

    #endregion

    private void Start()
    {
        CreateBullet();
        ActiveWeaponInit();
        UpgradeInitialize();
        CreateWave();
    }

    private void ActiveWeaponInit()
    {
        if (PlayerPrefs.GetFloat(CommonTypes.IS_START_DATA_KEY) == 0)
        {
            //ilk açılma
            foreach (var Data in WeaponDatas)
            {
                Data.IsBuy = false;
            }

            WeaponDatas.First(x => x.WeaponType == EWeaponType.PISTOL).IsBuy = true;
            ActiveWeaponList.Clear();
            ActiveWeaponList.Add(EWeaponType.PISTOL);
            WeaponController.Instance.SelectWeapon(WeaponController.Instance.GetWeapons()
                .First(x => x.GetWeaponType() == EWeaponType.PISTOL));
            PlayerPrefs.SetString(CommonTypes.LAST_WEAPON_DATA, EWeaponType.PISTOL.ToString());
            
            DOVirtual.DelayedCall(0.25f, () =>
            {
                WeaponUIController.Instance.GetOutline().enabled = true;
                WeaponUIController.Instance.GetText().text = "Equipped";
            });
        }
        else
        {
            //load etme
            ActiveWeaponList.Clear();
            Load();
        }
    }

    private void UpgradeInitialize()
    {
        if (PlayerPrefs.GetFloat(CommonTypes.IS_START_DATA_KEY) == 0)
        {
            //ilk defa açılma lvl 0 olacak
            foreach (var UpgradeUIItem in UpgradeUIController.Instance.GetUpgradeUIs())
            {
                UpgradeUIItem.GetData().Level = 0;
            }
        }
        else
        {
        }
    }

    public async void CreateWave()
    {
        int counter = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
            int randomRoot = Random.Range(0, CreatedRoots.Count);
            EEnemyType enemyTypr = Waves[CurrentIndex].Enemys[counter].GetEnemyType();
            EnemyComponent enemyPrefab = GameManager.Instance.GetGameSettings().EnemysPrefab
                .First(x => x.GetEnemyType() == enemyTypr);
            EnemyComponent enemy = Instantiate(enemyPrefab, CreatedRoots[randomRoot].position, quaternion.identity,
                GetEnemyRoot());
            CurrentWaveEnemys.Add(enemy);
            GetEnemys().Add(enemy);

            counter++;
            if (counter >= Waves[CurrentIndex].Enemys.Count)
            {
                CurrentIndex++;
                break;
            }
        }
    }


    #region Bullet

    public async void CreateBullet()
    {
        int Counter = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.03f));
            BulletComponent Bullet = Instantiate(_gameSettings.BulletPrefab);
            Bullet.transform.SetParent(BulletPool);
            Bullet.transform.localPosition = Vector3.zero;
            GetBullets().Add(Bullet);
            Bullet.gameObject.SetActive(false);
            Counter++;
            if (Counter >= 100) break;
        }
    }

    public BulletComponent GetBullet()
    {
        BulletComponent Bullet = GetBullets().First();
        GetBullets().Remove(Bullet);
        Bullet.gameObject.SetActive(true);
        Bullet.SetIsActive(true);
        return Bullet;
    }

    #endregion

    #region Getters

    public List<WaveComponent> GetWaves()
    {
        return Waves;
    }

    public int GetCurrentWaveIndex()
    {
        return CurrentIndex;
    }

    public Transform GetEnemyRoot()
    {
        return EnemyRoot;
    }

    public List<EWeaponType> GetActiveWeapons()
    {
        return ActiveWeaponList;
    }

    public List<EnemyComponent> GetWaveEnemys()
    {
        return CurrentWaveEnemys;
    }

    public List<EnemyComponent> GetEnemys()
    {
        return Enemys;
    }

    public List<BulletComponent> GetBullets()
    {
        return Bullets;
    }

    public List<UIUpgradeItem> GetItems()
    {
        return UpgradeItems;
    }

    #endregion

    #region Control Systems

    public async void EnemyLevelControl(float delay = 0.25f)
    {
        //LevelComplete Event
        //if içinde son wawe de mi diye control edilecek
        if (CurrentIndex >= Waves.Count)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            foreach (var enemy in GetEnemys())
            {
                if (enemy == null)
                {
                    GetEnemys().Remove(enemy);
                }
            }

            if (GetEnemys().Count < 1 && GetEnemys().Count < 1)
            {
                if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
                Debug.Log("Oyunu Kazandın");
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                GameManager.Instance.GetPlayerView().GetCrossHair().enabled = false;
                GameManager.Instance.ChangeGameState(EGameState.WIN);
                GameManager.Instance.OnGameStateChanged(EGameState.WIN);
                InterfaceManager.Instance.OnGameStateChanged(EGameState.WIN);
                GameManager.Instance.SetCurrency(500);
                InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            }
        }
    }

    public async void EnemyWaveControl(float delay = 0.25f)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        foreach (var enemy in CurrentWaveEnemys)
        {
            if (enemy == null)
            {
                GetEnemys().Remove(enemy);
            }
        }

        if (GetWaveEnemys().Count < 1 && GetEnemys().Count < 1)
        {
            if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
            Debug.Log("wave i Kazandın");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            if (CurrentIndex < Waves.Count)
            {
                //CreateWave();
                GameManager.Instance.SetCurrency(100);
                InterfaceManager.Instance.OnPlayerCurrencyUpdated();
                UIUpgrade.Instance.Initialize();
            }

            //wave text gelecek
            InterfaceManager.Instance.WaveTextUpdated(CurrentIndex);
        }
    }

    #endregion

    #region SaveSystem

    public void Save()
    {
        Saveweapon();
        SaveUpgradeUIData();
        PlayerPrefs.SetString(CommonTypes.LAST_WEAPON_DATA,
            GameManager.Instance.GetPlayerView().GetWeapon().GetWeaponType().ToString());
    }

    private void Load()
    {
        LoadWeaponDatas();
        LoadUpgradeUIData();
    }


    private void LoadWeaponDatas()
    {
        LoadWeapon();

        CurrentWeaponLoad();
    }

    private void CurrentWeaponLoad()
    {
        WeaponComponent Weapon = WeaponController.Instance.GetWeapons().First(x =>
            x.GetWeaponType().ToString() == PlayerPrefs.GetString(CommonTypes.LAST_WEAPON_DATA).ToString());
        GameManager.Instance.GetPlayerView().SetWeapon(Weapon);
        WeaponController.Instance.SelectWeapon(Weapon);
        WeaponUI WeaponUITarget = WeaponUIController.Instance.GetWeaponUIs()
            .First(x => x.GetWeaponType() == Weapon.GetWeaponType());
        WeaponUIController.Instance.GetWeaponUIs().First(x => x.GetWeaponType() == EWeaponType.PISTOL).GetOutline()
            .enabled = false;
        WeaponUIController.Instance.GetWeaponUIs().First(x => x.GetWeaponType() == EWeaponType.PISTOL).GetText().text =
            "Unequipped";
        WeaponUITarget.GetText().text = "Equipped";
        WeaponUITarget.GetOutline().enabled = true;
        WeaponUIController.Instance.SetCurrentOutline(WeaponUITarget.GetOutline());
        WeaponUIController.Instance.SetCurrentText(WeaponUITarget.GetText());
    }

    private void LoadWeapon()
    {
        Debug.Log($"Loading Weapon");
        var storedWeaponJson = PlayerPrefs.GetString(CommonTypes.WEAPON_DATA);
        var existingWeaponDatas = string.IsNullOrEmpty(storedWeaponJson)
            ? Array.Empty<EWeaponType>()
            : JsonConvert.DeserializeObject<EWeaponType[]>(storedWeaponJson);

        if (existingWeaponDatas.Length == 0) return;
        foreach (var existingWeaponData in existingWeaponDatas)
        {
            ActiveWeaponList.Add(existingWeaponData);
        }
    }

    private void LoadUpgradeUIData()
    {
        Debug.Log($"Loading Upgrade Data");
        var storedUpgradeDataJson = PlayerPrefs.GetString(CommonTypes.UPGRADE_UI_DATA);
        var existingUpgradeDatas = string.IsNullOrEmpty(storedUpgradeDataJson)
            ? Array.Empty<ExistingUpgradeData>()
            : JsonConvert.DeserializeObject<ExistingUpgradeData[]>(storedUpgradeDataJson);

        if (existingUpgradeDatas.Length == 0) return;
        foreach (var existingUpgradeData in existingUpgradeDatas)
        {
            // ActiveWeaponList.Add(existingUpgradeData);
            UpgradeUI UpgradeData = UpgradeUIController.Instance.GetUpgradeUIs()
                .First(x => x.GetData().Upgrade == existingUpgradeData.Upgradeype);
            UpgradeData.GetData().Level = existingUpgradeData.Id;
        }
    }

    private void Saveweapon()
    {
        Debug.Log($"Saving Weapon");
        var myList = CreateExistingWeaponData();
        var json = JsonConvert.SerializeObject(myList);
        PlayerPrefs.SetString(CommonTypes.WEAPON_DATA, json);
    }

    private void SaveUpgradeUIData()
    {
        Debug.Log($"Saving UpgradeData");
        var myList = CreateExistingUpgradeData();
        var json = JsonConvert.SerializeObject(myList);
        PlayerPrefs.SetString(CommonTypes.UPGRADE_UI_DATA, json);
    }

    private EWeaponType[] CreateExistingWeaponData()
    {
        Debug.Log($"Creating Existing Weapon Data");

        var newStoredSlotUI = new List<EWeaponType>();
        foreach (var SlotUI in ActiveWeaponList)
        {
            newStoredSlotUI.Add(SlotUI);
        }

        return newStoredSlotUI.ToArray();
    }

    private ExistingUpgradeData[] CreateExistingUpgradeData()
    {
        Debug.Log($"Creating Existing Upgrade UI Data");

        var newStoredUpgrade = new List<ExistingUpgradeData>();
        foreach (var UpgradeData in UpgradeUIController.Instance.GetUpgradeUIs())
        {
            newStoredUpgrade.Add(new ExistingUpgradeData()
            {
                Upgradeype = UpgradeData.GetData().Upgrade,
                Id = UpgradeData.GetData().Level
            });
        }

        return newStoredUpgrade.ToArray();
    }

    #endregion
}