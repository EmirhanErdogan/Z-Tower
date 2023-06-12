using TMPro;
using UnityEngine;
using DG.Tweening;
using Emir;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UIUpgrade : Emir.Singleton<UIUpgrade>
{
    [Header("Upgrade Item Root")] [SerializeField]
    private Transform upgradesRoot;

    [Header("Visual")] [SerializeField] private TMP_Text panelHeaderText;
    [Header("Common")] [SerializeField] private CanvasGroup canvasGroup;

    #region Private Fields

    private List<UIUpgradeItem> upgradeItems = new List<UIUpgradeItem>();
    private bool IsUpgradeOpen = true;
    private CancellationTokenSource token;

    #endregion

    private void Start()
    {
        LoadUpgradeData();
    }

    private void LoadUpgradeData()
    {
        // if (PlayerPrefs.GetFloat(CommonTypes.IS_START_DATA_KEY) == 0)
        // {
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_ADDHOLE, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_HOLESIZE, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_HOLESPEED, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_GETNUMBER, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_COINUPGRADE, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_DIAMONDDROP, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_EXPERİENCE, 1);
        //     PlayerPrefs.SetInt(CommonTypes.UPGRADE_HOLEPULL, 1);
        //
        //     foreach (var Item in LevelComponent.Instance.GetItems())
        //     {
        //         Item.GetSettings().Level = 1;
        //     }
        // }
        // else
        // {
        //     foreach (var Item in LevelComponent.Instance.GetItems())
        //     {
        //         switch (Item.GetSettings().type)
        //         {
        //             case EUpgrade.ADDHOLE:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_ADDHOLE);
        //                 break;
        //             case EUpgrade.HOLESIZE:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_HOLESIZE);
        //                 break;
        //             case EUpgrade.HOLESPEED:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_HOLESPEED);
        //                 break;
        //             case EUpgrade.GETNUMBER:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_GETNUMBER);
        //                 break;
        //             case EUpgrade.COINUPGRADE:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_COINUPGRADE);
        //                 break;
        //             case EUpgrade.DIAMONDDROP:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_DIAMONDDROP);
        //                 break;
        //             case EUpgrade.EXPERİENCEUPGRADE:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_EXPERİENCE);
        //                 break;
        //             case EUpgrade.HOLEPULL:
        //                 Item.GetSettings().Level = PlayerPrefs.GetInt(CommonTypes.UPGRADE_HOLEPULL);
        //                 break;
        //         }
        //     }
        // }
    }

    public async UniTask Initialize() //first time this ui created.
    {
        await UniTask.Delay(50);
        if (IsUpgradeOpen == false) return;
        UpgradeControl();
        ClearUpgrades();
        InitializeUpgrades();
    }


    public void InitializeUpgrades()
    {
        if (GameManager.Instance.GetPlayerView().GetIsGlory() is false)
        {
            if (LevelComponent.Instance.GetCurrentWaveIndex() == 0)
            {
                // İlk Wawedeyse kontrol edilecek
                UpgradeUIScriptable SkillStart = UpgradeUIController.Instance.GetUpgradeUIs()
                    .First(x => x.GetData().Upgrade == EUpgradeType.GETASKILLATSTART).GetData();
                if (SkillStart.Values[SkillStart.Level] == 0) return;
            }
        }
        

        List<UIUpgradeItem> items = new List<UIUpgradeItem>();
        if (LevelComponent.Instance.GetItems().Count >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                Skill:
                int RandomItems = Random.Range(0, LevelComponent.Instance.GetItems().Count);
                UIUpgradeItem ItemUpgrade = LevelComponent.Instance.GetItems()[RandomItems];
                if (items.Contains(ItemUpgrade))
                {
                    goto Skill;
                }

                items.Add(ItemUpgrade);
            }
        }

        if (items.Count < 1)
        {
            CloseUpgradeUI();
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            UIUpgradeItem createdItems = Instantiate(items[i], upgradesRoot);
            GetUpgradeItems().Add(createdItems);
        }

        bool value;
        if (GameManager.Instance.GetGameState() == EGameState.STARTED)
        {
            value = true;
        }
        else
        {
            value = false;
        }

        OpenUpgradeUI(value);
    }

    public void OpenUpgradeUI(bool value)
    {
        GameManager.Instance.ChangeGameState(EGameState.UPGRADE);
        GameUtils.SwitchCanvasGroup(InterfaceManager.Instance.GetMenuCanvas(),
            SkillComponent.Instance.GetSkillCnavasGroup(), 0.01f);
        if (value)
        {
            DOVirtual.DelayedCall(0.15f, () => { Time.timeScale = 0; });
        }

        //show the upgrade ui and hide the menu one.
    }

    public void CloseUpgradeUI()
    {
        if (GameManager.Instance.GetGameState() == EGameState.UPGRADE)
            GameManager.Instance.ChangeGameState(EGameState.STARTED);
        GameUtils.SwitchCanvasGroup(SkillComponent.Instance.GetSkillCnavasGroup(),
            InterfaceManager.Instance.GetMenuCanvasGroup(), 0.2f);
        Time.timeScale = 1;
    }

    public void ClearUpgrades()
    {
        if (GetUpgradeItems().Count < 1) return;

        for (int i = 0; i < GetUpgradeItems().Count; i++)
        {
            Destroy(GetUpgradeItems()[i].gameObject);
        }

        GetUpgradeItems().Clear();
    }

    public List<UIUpgradeItem> GetUpgradeItems()
    {
        return upgradeItems;
    }

    public void UpgradeControl()
    {
    }

    public void ResetIsUpgradeOpen()
    {
        IsUpgradeOpen = !IsUpgradeOpen;
    }
}