using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private GameObject btnWatchAds;
    [SerializeField] private GameObject btnUse;
    [SerializeField] private GameObject select;
    [SerializeField] private TMPro.TextMeshProUGUI txtUC;
    [SerializeField] private GameObject btnPurchase;
    [SerializeField] private CanvasGroup btnPurchaseCanvasGroup;
    [SerializeField] private TMPro.TextMeshProUGUI txtCost;
    [SerializeField] private TMPro.TextMeshProUGUI txtUnlockAtLevel;
    [SerializeField] private ItemType type;
    private Shop shop { get; set; }
    public ShopItemData Data { get; set; }

    public void SetUp(ShopItemData data, Shop shop)
    {
        Data = data;
        type = data.Type;
        this.shop = shop;
        avatar.sprite = data.AvatarSprite;
        btnPurchase.SetActive((data.UnlockBy == UnlockConditions.Coin || (data.UnlockBy == UnlockConditions.VideoReward && Data.ValueInCoin > 0))
            && !data.IsUnlocked && Data.LevelLimit <= DataController.Instance.Data.LevelIndex);
        btnWatchAds.SetActive(data.UnlockBy == UnlockConditions.VideoReward && !data.IsUnlocked && Data.LevelLimit <= DataController.Instance.Data.LevelIndex);
        switch (type)
        {
            case ItemType.Skin:
                btnUse.SetActive(data.IsUnlocked && DataController.Instance.Data.SkinID != data.ID);
                select.SetActive(DataController.Instance.Data.SkinID == data.ID);
                break;
            case ItemType.Weapon:
                btnUse.SetActive(data.IsUnlocked && DataController.Instance.Data.WeaponID != data.ID);
                select.SetActive(DataController.Instance.Data.WeaponID == data.ID);
                break;
            default:
                break;
        }
        txtUC.gameObject.SetActive(data.UnlockBy == UnlockConditions.VideoReward && !data.IsUnlocked);
        txtUC.text = (data.UCTotal - data.UCCurrent).ToString();
        int realCost = Data.ValueInCoin > 0 ? (data.UCTotal - data.UCCurrent) * Data.ValueInCoin : data.UCTotal;
        txtCost.text = realCost.ToString();
        if (Data.LevelLimit > 0 && !Data.IsUnlocked)
        {
            txtUnlockAtLevel.gameObject.SetActive(Data.LevelLimit > DataController.Instance.Data.LevelIndex);
            txtUnlockAtLevel.text = "UNLOCK AT\n<size=60>Lv." + Data.LevelLimit + "</size>";
        }
        btnPurchaseCanvasGroup.alpha = (realCost <= DataController.Instance.Data.Coin ? 1 : 0.25f);
        btnPurchaseCanvasGroup.blocksRaycasts = realCost <= DataController.Instance.Data.Coin;
    }

    public void OnSelect()
    {
        if (Data.IsUnlocked)
        {
            switch (type)
            {
                case ItemType.Skin:
                    if (DataController.Instance.Data.SkinID != Data.ID)
                    {
                        shop.OnNewAsset();
                    }
                    DataController.Instance.Data.SkinID = Data.ID;
                    break;
                case ItemType.Weapon:
                    if (DataController.Instance.Data.WeaponID != Data.ID)
                    {
                        shop.OnNewAsset();
                    }
                    DataController.Instance.Data.WeaponID = Data.ID;
                    break;
                default:
                    break;
            }
            DataController.Instance.SaveData();
        }
        switch (type)
        {
            case ItemType.Skin:
                if (shop.CurrentSkinItem != null)
                {
                    shop.CurrentSkinItem.OnDeselect();
                }
                shop.CurrentSkinItem = this;
                break;
            case ItemType.Weapon:
                if (shop.CurrentWeaponItem != null)
                {
                    shop.CurrentWeaponItem.OnDeselect();
                }
                shop.CurrentWeaponItem = this;
                break;
            default:
                break;
        }
        select.SetActive(true);
        Refresh();
    }

    public void OnDeselect()
    {
        select.SetActive(false);
        Refresh();
    }

    public void Refresh()
    {
        SetUp(Data, shop);
        shop.UpdateManequine();
    }

    public void OnUse()
    {
        OnSelect();
        //skinShop.Hide();
    }

    public void OnPurchase()
    {
        int realCost = Data.ValueInCoin > 0 ? (Data.UCTotal - Data.UCCurrent) * Data.ValueInCoin : Data.UCTotal;
        if (realCost > DataController.Instance.Data.Coin)
        {
            shop.OnNotEnoughCoin();
        }
        else
        {
            shop.CollectCoin(-realCost);
            switch (type)
            {
                case ItemType.Skin:
                    AssetController.Instance.UpdateSkinItemUC(Data.ID, Data.UCTotal);
                    AssetController.Instance.ClaimSkinID(Data.ID);
                    DataController.Instance.SaveData();
                    //AnalyticsController.Instance.LogCustomEvent("skin_unlock_coin_" + Data.ID, "", "");
                    break;
                case ItemType.Weapon:
                    AssetController.Instance.UpdateWeaponItemUC(Data.ID, Data.UCTotal);
                    AssetController.Instance.ClaimWeaponID(Data.ID);
                    DataController.Instance.SaveData();
                    //AnalyticsController.Instance.LogCustomEvent("weapon_unlock_coin_" + Data.ID, "", "");
                    break;
                default:
                    break;
            }
            OnUse();
            shop.Refresh();
            shop.OnNewAsset();
        }
    }

    public void WatchAds()
    {
        GlobalController.Instance.ShowRewardedVideo(OnWatchAds,"PurchaseShopItem");
    }

    private void OnWatchAds()
    {
        switch (type)
        {
            case ItemType.Skin:
                AssetController.Instance.UpdateSkinItemUC(Data.ID, ++Data.UCCurrent);
                break;
            case ItemType.Weapon:
                AssetController.Instance.UpdateWeaponItemUC(Data.ID, ++Data.UCCurrent);
                break;
            default:
                break;
        }
        if (Data.UCCurrent >= Data.UCTotal)
        {
            OnUse();
            shop.Refresh();
            shop.OnNewAsset();
            switch (type)
            {
                case ItemType.Skin:
                    AssetController.Instance.ClaimSkinID(Data.ID);
                    AssetController.Instance.UpdateSkinItemUC(Data.ID, ++Data.UCCurrent);
                    //AnalyticsController.Instance.LogCustomEvent("skin_unlock_ads_" + Data.ID, "", "");
                    break;
                case ItemType.Weapon:
                    AssetController.Instance.ClaimWeaponID(Data.ID);
                    AssetController.Instance.UpdateWeaponItemUC(Data.ID, ++Data.UCCurrent);
                    //AnalyticsController.Instance.LogCustomEvent("weapon_unlock_ads_" + Data.ID, "", "");
                    break;
                default:
                    break;
            }
            DataController.Instance.SaveData();
            return;
        }
        Refresh();
    }
}
