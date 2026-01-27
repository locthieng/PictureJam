using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIRefillLifePopup : PopUp
{
    [SerializeField] private TextMeshProUGUI txtLifeCount;
    [SerializeField] private TextMeshProUGUI txtLifePlus;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private int price;
    public UIDigitalClock clock;
    public Action OnRefill;

    public void RefreshPopup()
    {
        txtLifeCount.text = LifeSystem.Instance.CurrentLife.ToString();
        txtLifePlus.text = "+" + (LifeSystem.Instance.MaxLife - LifeSystem.Instance.CurrentLife);
        txtPrice.text = price.ToString();
    }

    public void OnPurchase()
    {
        if (CoinSystem.Instance.coin < price)
        {
            //GameUIController.Instance.ShowLog(transform.parent, "Not enough coin!");
            //GameUIController.Instance.ShowCoinPlusPopup(GlobalController.CurrentStage == StageScreen.Home ? "home_feature_popup" : "ingame_feature_popup");
            //GlobalSDKController.Instance.LogEarnResource("heart", "heart", (GlobalController.MaxLife - DataController.Instance.Data.Life).ToString(), "purchase");
            return;
        }
        LifeSystem.Instance.currentLife = LifeSystem.Instance.MaxLife;
        CoinSystem.Instance.coin -= price;
        DataController.Instance.SaveData();
        //GameUIController.Instance.UpdateCoin(null, -price);
        //SetOnHidden(OnRefill);
        Hide();
    }

    public void OnWatchAdsToRefill()
    {
        /*GlobalController.Instance.ShowRewardedVideo(() => {
            GlobalSDKController.Instance.LogEarnResource("heart", "heart", "1", "ad_reward");
            GlobalController.Instance.OnLifeChange(1);
            PageUI.Instance.RefreshLifeInfo();
            SetOnHidden(OnRefill);
            Hide();
        }, GlobalController.CurrentLevelIndex, "heart_popup", "refill_heart");*/
        LifeSystem.Instance.AddLife();
        GameUIController.Instance.uiLife.RefreshUILife();
        //SetOnHidden(OnRefill);
        Hide();
    }

}
