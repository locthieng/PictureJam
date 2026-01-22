using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyBonus : MonoBehaviour
{
    [SerializeField] private List<DailyBonusItem> listDailyBonusItemData;
    [SerializeField] private UIDailyBonusItem dailyBonusItemPrefab;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private Transform grid;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject btnClaim;
    [SerializeField] private GameObject btnClaimNext;
    [SerializeField] private CanvasGroup btnSkip;
    [SerializeField] private TMPro.TextMeshProUGUI txtCoin;
    private List<UIDailyBonusItem> listDailyBonusItems = new List<UIDailyBonusItem>();
    private DailyBonusItem currentDayData;
    private DailyBonusItem nextDayData;
    private int currentDay;

    public void CheckAndShow()
    {
        if (!GlobalController.IsDailyShown)
        {
            GlobalController.IsDailyShown = true;
            int gap = 1;
            if (!string.IsNullOrEmpty(DataController.Instance.LastLogin))
            {
                DateTime lastLogin = DateTime.Parse(DataController.Instance.LastLogin);
                gap = (DateTime.Today - lastLogin).Days;
            }

            // If more than a day passed since player last logins and user's reward day is before current login day
            if (gap > 0 && GlobalController.RewardDay < GlobalController.LoginDay)
            {
                currentDay = GlobalController.LoginDay % (listDailyBonusItemData.Count - 1);
                Show();
            }
            else // Nothing to do here. DailyBonus will not be shown.
            {
                return;
            }
        }
    }

    public void Show()
    {
        if (listDailyBonusItems.Count == 0)
        {
            for (int i = 0; i < listDailyBonusItemData.Count; i++)
            {
                UIDailyBonusItem dailyItem = Instantiate(dailyBonusItemPrefab, grid);
                dailyItem.SetUp(i, currentDay, listDailyBonusItemData[i]);
                listDailyBonusItems.Add(dailyItem);
            }
            content.transform.localScale = Vector3.one * 0.6f;
            LeanTween.alphaCanvas(canvas, 1, 0.2f).setOnComplete(() =>
            {
                canvas.blocksRaycasts = true;
            });
            LeanTween.scale(content, Vector3.one, 0.2f);
            SetRewardData();
        }
        //StartCoroutine(CoReposition());
    }

    private void SetRewardData()
    {
        currentDayData = listDailyBonusItemData[currentDay];
        if (currentDay < listDailyBonusItemData.Count - 1)
        {
            nextDayData = listDailyBonusItemData[currentDay + 1];
        }
        else
        {
            nextDayData.Value = -1;
        }
    }

    public void Claim()
    {
        btnClaim.SetActive(false);
        listDailyBonusItems[currentDay].SetClaimed();
        switch (currentDayData.Type)
        {
            case DailyBonusType.Coin:
                //GameUIController.Instance.UpdateCoin(txtCoin, DataController.Instance.Data.Coin, currentDayData.Value, 0.3f, ShowClaimNext);
                //DataController.Instance.Data.Coin += currentDayData.Value;
                break;
            case DailyBonusType.Skin:
                StartCoroutine(CoClaimSkin());
                break;
            default:
                break;
        }
        GlobalController.RewardDay = GlobalController.LoginDay;
        PlayerPrefs.SetInt("RewardDay", GlobalController.RewardDay);
        DataController.Instance.SaveData();
    }

    IEnumerator CoReposition()
    {
        yield return null;
        LeanTween.moveLocalY(scroll.content.gameObject, scroll.GetSnapToPositionToBringChildIntoView(listDailyBonusItems[currentDay].GetComponent<RectTransform>()).y, 0.4f);
    }

    private IEnumerator CoClaimSkin()
    {
        yield return new WaitForSeconds(0.3f);
        ShowClaimNext();
    }

    private void ShowClaimNext()
    {
        if (nextDayData.Value > -1)
        {
            btnClaimNext.SetActive(true);
            btnSkip.gameObject.SetActive(true);
            Invoke(nameof(ShowButtonSkip), 3);
        }
        else
        {
            Invoke(nameof(Hide), 1);
        }
    }

    private void ShowButtonSkip()
    {
        btnSkip.LeanAlpha(1, 0.5f).setOnComplete(() =>
        {
            btnSkip.blocksRaycasts = true;
        });
    }

    public void WatchAdsClaimNext()
    {
        GlobalController.Instance.ShowRewardedVideo(ClaimNext, "DailyBonusClaimNext");
    }

    private void ClaimNext()
    {
        CancelInvoke(nameof(ShowButtonSkip));
        btnClaimNext.SetActive(false);
        GlobalController.LoginDay++;
        currentDay = GlobalController.LoginDay % (listDailyBonusItemData.Count - 1);
        SetRewardData();
        nextDayData.Value = -1;
        Claim();
    }

    public void Hide()
    {
        canvas.blocksRaycasts = false;
        LeanTween.alphaCanvas(canvas, 0, 0.2f);
        LeanTween.scale(content, Vector3.one * 0.6f, 0.2f).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
