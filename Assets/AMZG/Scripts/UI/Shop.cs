using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CoinPackID
{
    smallpouch,
    mediumpouch,
    bigpouch,
    hugepouch
}
public class Shop : MonoBehaviour
{
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform[] grids;
    [SerializeField] private ScrollRect[] scrolls;
    [SerializeField] private CanvasGroup[] tabCanvasGroups;
    [SerializeField] private GameObject[] tabContents;
    [SerializeField] private CharacterLiveAssetController manequine;
    [SerializeField] private ParticleSystem eConfetti;
    [SerializeField] private TMPro.TextMeshProUGUI txtCoin;
    private List<ShopItem> listSkinItems = new List<ShopItem>();
    private List<ShopItem> listWeaponItems = new List<ShopItem>();
    public ShopItem CurrentSkinItem;
    public ShopItem CurrentWeaponItem;

    private void OnEnable()
    {
        SetUp();
        OnTabSelect(1);
        UpdateManequine();
    }

    public void UpdateManequine()
    {
        //manequine.ChangeSkin(DataController.Instance.Data.SkinID);
        //manequine.ChangeWeapon(DataController.Instance.Data.WeaponID);
    }

    public void SetUp()
    {
        for (int i = 0; i < grids[0].childCount; i++)
        {
            Destroy(grids[0].GetChild(i).gameObject);
        }
        for (int i = 0; i < grids[1].childCount; i++)
        {
            Destroy(grids[1].GetChild(i).gameObject);
        }
        listSkinItems.Clear();
        listWeaponItems.Clear();
        for (int i = 0; i < AssetController.Instance.ListSkinItemData.Count; i++)
        {
            ShopItem si = Instantiate(shopItemPrefab, grids[1]);
            si.SetUp(AssetController.Instance.ListSkinItemData[i], this);
            if (AssetController.Instance.ListSkinItemData[i].ID == DataController.Instance.Data.SkinID)
            {
                si.OnSelect();
            }
            listSkinItems.Add(si);
        }
        for (int i = 0; i < AssetController.Instance.ListWeaponItemData.Count; i++)
        {
            ShopItem si = Instantiate(shopItemPrefab, grids[1]);
            si.SetUp(AssetController.Instance.ListSkinItemData[i], this);
            if (AssetController.Instance.ListSkinItemData[i].ID == DataController.Instance.Data.SkinID)
            {
                si.OnSelect();
            }
            listSkinItems.Add(si);
        }
        //txtCoin.text = DataController.Instance.Data.Coin.ToString();
        StartCoroutine(CoReposition());
    }

    IEnumerator CoReposition()
    {
        yield return null;
        LeanTween.value(scrolls[0].horizontalNormalizedPosition, 0, 0.4f).setOnUpdate((float f) =>
        {
            scrolls[0].horizontalNormalizedPosition = f;
        });
        LeanTween.value(scrolls[1].horizontalNormalizedPosition, 0, 0.4f).setOnUpdate((float f) =>
        {
            scrolls[1].horizontalNormalizedPosition = f;
        });
    }

    public void OnNotEnoughCoin()
    {
        //OnTabSelect(1); // Show diamond tab
        Debug.LogError("Not enough coin");
    }

    public void OnTabSelect(int index)
    {
        for (int i = 0; i < tabCanvasGroups.Length; i++)
        {
            tabCanvasGroups[i].alpha = i == index ? 1 : 0.5f;
        }
        for (int i = 0; i < tabContents.Length; i++)
        {
            tabContents[i].SetActive(i == index);
        }
        StartCoroutine(CoReposition());
    }

    public void OnCoinPackPurchase(int id)
    {
        //IAPDiamondPackEvent e = new IAPDiamondPackEvent();
        //e.AddListener(OnDiamondPackPurchaseSuccessful);
        //GlobalController.Instance.PurchaseDiamond((DiamondPackID)id, e);
    }

    private void OnCoinPackPurchaseSuccessful(CoinPackID arg0)
    {
        switch (arg0)
        {
            case CoinPackID.smallpouch:
                CollectCoin(400);
                break;
            case CoinPackID.mediumpouch:
                CollectCoin(900);
                break;
            case CoinPackID.bigpouch:
                CollectCoin(1400);
                break;
            case CoinPackID.hugepouch:
                CollectCoin(2000);
                break;
            default:
                break;
        }
    }

    public void CollectCoin(int value)
    {
        //GameUIController.Instance.UpdateCoin(txtCoin, DataController.Instance == null ? 0 : DataController.Instance.Data.Coin, DataController.Instance.Data.Coin + value);
        //DataController.Instance.Data.Coin += value;
        DataController.Instance.SaveData();
        Refresh();
        GameUIController.Instance.UpdateShopNoti();
    }

    public void OnNewAsset()
    {
        eConfetti.Play();
    }

    public void Refresh()
    {
        for (int i = 0; i < listSkinItems.Count; i++)
        {
            listSkinItems[i].Refresh();
        }
        UpdateManequine();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(canvasGroup, 1, 0.02f);
        canvasGroup.blocksRaycasts = true;
        GlobalController.Instance.ShowInterstitial();
        Refresh();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        //GameUIController.Instance.ShowContent(); // For video ads assets

    }
}
