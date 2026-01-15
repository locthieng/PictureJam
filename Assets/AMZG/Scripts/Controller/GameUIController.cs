using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StageScreen
{
    None,
    Home,
    InGame,
    LevelSelector
}

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; set; }
    [SerializeField] private CanvasScaler[] canvasScalers;
    [SerializeField] private Image cover;
    [SerializeField] private UIRateUs rateUI;
    [SerializeField] private GameObject btnToggleUI;
    [SerializeField] private GameObject[] uiToToggles;
    [Header("Start UI")]
    [SerializeField] private GameObject shopNoti;
    [SerializeField] private CanvasGroup startUI;
    [Header("In Game UI")]
    [SerializeField] private Text[] txtProgress;
    [SerializeField] private TMPro.TextMeshProUGUI txtCoin;
    [SerializeField] private CanvasGroup inGameUI;
    [SerializeField] private UIDailyBonus uiDailyBonus;
    [Header("End UI")]
    [SerializeField] private Image resultCover;
    [SerializeField] private CanvasGroup resultWin;
    [SerializeField] private CanvasGroup resultFail;
    [SerializeField] private ParticleSystem eVictory;

    [SerializeField] private TMPro.TMP_Text levelCurrent;

    [Header("Setting")]
    [SerializeField] private CanvasGroup settingUI;

    private bool isUION;

    public static bool IsUIMatchWidth
    {
        get
        {
            return GlobalController.ScreenRatio < GlobalController.FixedStageResolution.x / GlobalController.FixedStageResolution.y;
        }
    }

    private void Start()
    {
        Instance = this;
        isUION = !GlobalController.Instance.ForTesting;
        btnToggleUI.SetActive(GlobalController.Instance.ForTesting);
        for (int i = 0; i < canvasScalers.Length; i++)
        {
            canvasScalers[i].matchWidthOrHeight = IsUIMatchWidth ? 0 : 1;
        }
        UpdateNoAdsButtons();
        cover.gameObject.SetActive(true);
    }

    public void ToggleUI()
    {
        isUION = !isUION;
        for (int i = 0; i < uiToToggles.Length; i++)
        {
            uiToToggles[i].SetActive(isUION);
        }
    }

    public void SwitchStageUI()
    {
        switch (GlobalController.CurrentStage)
        {
            case StageScreen.None:
                break;
            case StageScreen.Home:
                inGameUI.alpha = 0;
                inGameUI.blocksRaycasts = false;
                startUI.alpha = 1;
                startUI.blocksRaycasts = true;
                break;
            case StageScreen.InGame:
                inGameUI.alpha = 1;
                inGameUI.blocksRaycasts = true;
                startUI.alpha = 0;
                startUI.blocksRaycasts = false;
                break;
            case StageScreen.LevelSelector:
                inGameUI.alpha = 0;
                inGameUI.blocksRaycasts = false;
                startUI.alpha = 0;
                startUI.blocksRaycasts = false;
                break;
            default:
                break;
        }
        LeanTween.alpha(cover.rectTransform, 0, 0.4f).setOnComplete(() =>
        {
            cover.raycastTarget = false;
            if (GlobalController.CurrentStage == StageScreen.Home)
            {
                uiDailyBonus.CheckAndShow();
            }
        });
        eVictory.Stop();
    }

    public void ShowInGameUI(int level)
    {
        SwitchStageUI();
        levelCurrent.text = "Level " + level.ToString();
    }

    public void UpdateCoin(TMPro.TextMeshProUGUI txtCoin, int previousValue, int value, float duration = 1f, Action callback = null)
    {
        if (txtCoin == null)
        {
            txtCoin = this.txtCoin;
        }
        // Effects
        LeanTween.cancel(txtCoin.gameObject);
        LeanTween.scale(txtCoin.gameObject, Vector3.one * 1.5f, 0.05f).setOnComplete(() =>
        {
            LeanTween.scale(txtCoin.gameObject, Vector3.one, 0.25f);
        });
        LeanTween.value(previousValue, value, duration).setOnUpdate((float f) =>
        {
            txtCoin.text = f.ToString("0");
            //txtCoinCollected.text = (f - previousValue).ToString("0");
        }).setOnComplete(callback);
    }

    public void UpdateShopNoti()
    {
        for (int i = 0; i < AssetController.Instance.ListSkinItemData.Count; i++)
        {
            if (AssetController.Instance.ListSkinItemData[i].IsUnlocked &&
                !DataController.Instance.Data.SkinIDs.Contains(AssetController.Instance.ListSkinItemData[i].ID))
            {
                shopNoti.SetActive(true);
                return;
            }
        }
        for (int i = 0; i < AssetController.Instance.ListWeaponItemData.Count; i++)
        {
            if (AssetController.Instance.ListWeaponItemData[i].IsUnlocked &&
                !DataController.Instance.Data.WeaponIDs.Contains(AssetController.Instance.ListWeaponItemData[i].ID))
            {
                shopNoti.SetActive(true);
                return;
            }
        }
        shopNoti.SetActive(false);
    }

    public void UpdateLevelProgress(float progress)
    {
    }

    public void ShowConfetti()
    {
        if (eVictory != null)
        {
            eVictory.Play();
        }
    }

    public void ToggleSound(bool isOn)
    {
        GlobalController.IsSoundOn = !GlobalController.IsSoundOn;
        if (GlobalController.IsSoundOn)
        {
            SoundController.Instance.Unmute();
        }
        else
        {
            SoundController.Instance.Mute();
        }
        PlayerPrefs.SetInt("IsSoundOn", GlobalController.IsSoundOn ? 1 : 0);
    }

    public void SetEndResultLabelActive(bool win, bool isActive)
    {
        if (win)
        {
            LeanTween.alphaCanvas(resultWin, 1, isActive ? 0.2f : 0);
        }
        else
        {
            LeanTween.alphaCanvas(resultFail, 1, isActive ? 0.2f : 0);
        }
    }

    public void ShowLevelBreak(Action callback, float duration = 0.3f, float delay = 0f, bool hideCover = false)
    {
        cover.raycastTarget = true;
        LeanTween.alpha(cover.rectTransform, 1, duration).setOnComplete(() =>
        {
            levelCurrent.text = "";
            if (hideCover)
            {
                LeanTween.alpha(cover.rectTransform, 0, duration).setOnComplete(() =>
                {
                    cover.raycastTarget = false;
                }).setDelay(delay);
            }
            callback?.Invoke();
        });
    }

    public void ShowRateUs(Action onClose, Action onRatePositive)
    {
        rateUI.OnClose = onClose;
        rateUI.OnRatePositive = onRatePositive;
        rateUI.Show();
    }

    public void ShowGameEnd(bool win)
    {
        //LeanTween.alpha(resultCover.rectTransform, 0.5f, 0.2f);
        LeanTween.scale(resultCover.gameObject, Vector3.one * 1, 0.4f);
        inGameUI.blocksRaycasts = false;
        LeanTween.alphaCanvas(inGameUI, 0, 0.1f);
        if (win)
        {
            LeanTween.alphaCanvas(resultWin, 1, 0.2f).setOnComplete(() =>
            {
                resultWin.blocksRaycasts = true;
            });
        }
        else
        {
            LeanTween.alphaCanvas(resultFail, 1, 0.2f).setOnComplete(() =>
            {
                resultFail.blocksRaycasts = true;
            });
        }
    }

    public LevelSelectorUI levelSelectorUI;

    public void ShowLevelSelector()
    {
        levelSelectorUI.Show();
        GlobalController.Instance.ShowMREC();
    }

    public void HideLevelSelector()
    {
        GlobalController.CurrentStage = StageScreen.Home;
        levelSelectorUI.Hide();
        GlobalController.Instance.ShowBanner();
    }

    [SerializeField] private GameObject btnHint;

    internal void SetButtonHintActive(bool isActive)
    {
        btnHint.SetActive(isActive);
    }

    [SerializeField] private GameObject[] btnNoAds;

    internal void UpdateNoAdsButtons()
    {
        for (int i = 0; i < btnNoAds.Length; i++)
        {
            btnNoAds[i].SetActive(!DataController.Instance.Stats.NoAds);
        }
    }
}
