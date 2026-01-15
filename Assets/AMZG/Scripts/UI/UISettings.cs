using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private UIToggleSpriteSwap tgSound;
    [SerializeField] private UIToggleSpriteSwap tgBgm;
    [SerializeField] private UIToggleSpriteSwap tgHaptic;
    [SerializeField] private CanvasGroup canvas;

    private void Start()
    {
        tgSound.OnValueChange.AddListener(OnToggleSound);
        tgBgm.OnValueChange.AddListener(OnToggleBgm);
        tgHaptic.OnValueChange.AddListener(OnToggleHaptic);
    }

    private void OnToggleHaptic(bool isOn)
    {
        PlayerPrefs.SetInt("IsHapticOn", isOn ? 1 : 0);
        GlobalController.IsHapticOn = isOn;
    }

    private void OnToggleBgm(bool isOn)
    {
        PlayerPrefs.SetInt("IsBgmOn", isOn ? 1 : 0);
        GlobalController.IsBgmOn = isOn;
        if (isOn)
        {
            SoundController.Instance.Unmute();
        }
        else
        {
            SoundController.Instance.Mute();
        }
    }

    private void OnToggleSound(bool isOn)
    {
        GlobalController.IsSoundOn = isOn;
        PlayerPrefs.SetInt("IsSoundOn", isOn ? 1 : 0);
    }

    public void Show()
    {
        if (StageController.Instance.IsOver) return;
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        tgSound.IsOn = GlobalController.IsSoundOn;
        tgBgm.IsOn = GlobalController.IsBgmOn;
        tgHaptic.IsOn = GlobalController.IsHapticOn;
        GlobalController.Instance.ShowInterstitial();
        GlobalController.Instance.ShowMREC();
    }

    public void Hide()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        GlobalController.Instance.ShowBanner();
    }
}
