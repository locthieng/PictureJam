using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIRemoveAds : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI txtPrice;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject content;
    public bool IsAlreadyShown { get; set; }
    private Action onHidden;

    public void Show(bool force = false)
    {
        Show(null, force);
    }

    public void Show(Action onHidden, bool force = false)
    {
        if (DataController.Instance.Stats.NoAds || (IsAlreadyShown && !force)) return;
        GlobalController.Instance.ShowBanner();
        this.onHidden = onHidden;
        LeanTween.alphaCanvas(canvas, 1, 0.1f);
        content.transform.localScale = Vector3.one * 0.5f;
        LeanTween.scale(content, Vector3.one, 0.2f).setOnComplete(() =>
        {
            canvas.blocksRaycasts = true;
        });
        //if (IAPController.Instance.ProductInfo.Count > 0)
        //{
        //    txtPrice.text = IAPController.Instance.ProductInfo[IAPController.ItemIDs[(int)PurchaseID.NO_ADS]].metadata.localizedPriceString;
        //}
    }

    public void Hide()
    {
        LeanTween.alphaCanvas(canvas, 0, 0.1f).setOnComplete(onHidden);
        canvas.blocksRaycasts = false;
    }

    public void Purchase()
    {
        GlobalController.Instance.PurchaseNoAds(OnComplete);
    }

    private void OnComplete()
    {
        DataController.Instance.Stats.NoAds = true;
        DataController.Instance.SaveStats();
        GlobalController.Instance.HideBanner();
        GameUIController.Instance.UpdateNoAdsButtons();
        //if (LevelController.Instance.Level != null)
        //{
        //    LevelController.Instance.Level.DisableNativeObject();
        //}
        Hide();
    }

    public void RestorePurchase()
    {
        //IAPController.Instance.purchaseSuccessNoAds = OnComplete;
        GlobalController.Instance.RestorePurchase();
    }
}
