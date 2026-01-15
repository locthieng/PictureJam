using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] private GameObject currentMark;
    [SerializeField] private GameObject checkMark;
    public UIButton buttonScript;
    private int level;
    [SerializeField] private GameObject lockMark;
    [SerializeField] private Image avatar;
    [SerializeField] private TMPro.TextMeshProUGUI txtLevel;
    private Sprite sprite;

    public void SetUp(int level, Sprite sprite)
    {
        this.level = level;
        this.sprite = sprite;
        avatar.sprite = sprite;
        txtLevel.text = "Lv." + level;
        if (GlobalController.Instance.ForTesting)
        {
            lockMark.SetActive(false);
            currentMark.SetActive(false);
            checkMark.SetActive(true);
            buttonScript.enabled = true;
        }
        else
        {
            currentMark.gameObject.SetActive(DataController.Instance.Data.LevelIndex == level);
            checkMark.SetActive(DataController.Instance.Data.LevelIndex != level && DataController.Instance.Data.Levels.Contains(level));
            lockMark.SetActive(!DataController.Instance.Data.Levels.Contains(level) && DataController.Instance.Data.LevelIndex != level);
            buttonScript.enabled = DataController.Instance.Data.Levels.Contains(level) || DataController.Instance.Data.LevelIndex == level;
        }
    }

    public void Refresh()
    {
        SetUp(level, sprite);
    }

    public void OnPlay()
    {
        StageController.Instance.ForceStartLevel(level);
        StageController.IsStart = true;
    }

    private void OnLevelUnlocked()
    {
        DataController.Instance.Data.LevelIndex = level;
        DataController.Instance.SaveData();
        OnPlay();
    }

    public void WatchAdsPlayNow()
    {
        GlobalController.Instance.ShowRewardedVideo(OnLevelUnlocked, "UnlockNewLevel");
        //AnalyticsController.Instance.LogCustomEvent("reward_unlock_level_" + level.ToString("000"), "level", level.ToString("000"));
    }
}
