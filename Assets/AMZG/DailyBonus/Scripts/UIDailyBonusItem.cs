using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DailyBonusType
{
    Coin,
    Booster,
    Skin
}

[Serializable]
public struct DailyBonusItem
{
    public DailyBonusType Type;
    public int Value;
}

public class UIDailyBonusItem : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI txtDay;
    [SerializeField] private TMPro.TextMeshProUGUI txtValue;
    [SerializeField] private Image avatar;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject checkMark;
    [SerializeField] private GameObject currentMark;
    [SerializeField] private ParticleSystem effectGet;
    public DailyBonusItem Data;

    public void SetUp(int day, int currentDay, DailyBonusItem item)
    {
        Data = item;
        txtDay.text = "Day " + day;
        txtValue.text = item.Value.ToString();
        avatar.gameObject.SetActive(item.Type == DailyBonusType.Skin);
        if (item.Type == DailyBonusType.Skin)
        {
            avatar.sprite = AssetController.Instance.ListSkinItemData[item.Value].AvatarSprite;
        }
        coinIcon.SetActive(item.Type == DailyBonusType.Coin);
        checkMark.SetActive(day < currentDay);
        currentMark.SetActive(day == currentDay);
    }

    internal void SetClaimed()
    {
        checkMark.SetActive(true);
        currentMark.SetActive(false);
        if (Data.Type == DailyBonusType.Skin)
        {
            AssetController.Instance.UnlockSkin(Data.Value);
            //AnalyticsController.Instance.LogCustomEvent("skin_unlock_claim_" + Data.Value, "", "");
            effectGet.Play();
        }
    }
}
