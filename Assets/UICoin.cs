using Ranged;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICoin : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI[] txtCoin;
    [SerializeField] private Transform coinIconPrefab;
    [SerializeField] private Transform coinStartIcon;
    [SerializeField] private Transform coinEndIcon;
    [SerializeField] private AnimationCurve flyCurve;
    private rfloat randMoneyMoveRadius = new rfloat(3f, 4f);
    private Vector3 movePos;
    private float angle;

    public void ShowFlyingCoinsFrom(Transform coinStart, int total, Action callback)
    {
        coinStartIcon = coinStart;
        coinEndIcon = GlobalController.CurrentStage == StageScreen.Home ? coinEndHome : coinEndInGame;
        ShowFlyingCoins(total, callback);
    }

    [SerializeField] private Transform coinEndHome;
    [SerializeField] private Transform coinEndInGame;
    [SerializeField] private Transform flyingCoinsContainer;
    [SerializeField] private int minFlyingCoins = 10;
    [SerializeField] private AudioClip sfxCoin;

    public void ShowFlyingCoins(int total, Action callback)
    {
        if (total > 10) total = 18;
        //if (total < minFlyingCoins) total = minFlyingCoins;
        float gapAngle = Mathf.PI * 2 / total;
        for (int i = 0; i < total; i++)
        {
            Transform m = Instantiate(coinIconPrefab, flyingCoinsContainer);
            m.position = coinStartIcon.position;
            angle = gapAngle * i;
            float moneyMoveDelay = 0.1f + 0.05f * i;
            bool callBackActive = i == 0;
            movePos = m.position;
            movePos.x += Mathf.Sin(angle) * randMoneyMoveRadius.RandomValue;
            movePos.y += Mathf.Cos(angle) * randMoneyMoveRadius.RandomValue;
            m.LeanMove(movePos, UnityEngine.Random.Range(0.5f, 0.8f)).setDelay(UnityEngine.Random.Range(0.0f, 0.1f)).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
            {
                //m.LeanScale(Vector3.one * 0.5f, 0.3f)/*.setDelay(moneyMoveDelay - 0.1f)*/;
                SoundController.Instance.PlaySingle(sfxCoin);
                m.LeanMove(coinEndIcon.position, UnityEngine.Random.Range(0.7f, 1.2f))/*.setDelay(moneyMoveDelay)*/.setEase(flyCurve).setOnComplete(() =>
                {
                    // Update total money on first item arrival
                    if (callBackActive)
                    {
                        callback?.Invoke();
                    }
                    Destroy(m.gameObject);
                    coinEndIcon.localScale = Vector3.one * 1.2f;
                    LeanTween.cancel(coinEndIcon.gameObject);
                    LeanTween.scale(coinEndIcon.gameObject, Vector3.one, 0.1f);
                });
            });
        }
    }

    public void UpdateCoin(TMPro.TextMeshProUGUI txtCoin, int previousValue, int value, float duration = 1f, Action callback = null)
    {
        if (txtCoin == null)
        {
            txtCoin = GlobalController.CurrentStage == StageScreen.Home ? this.txtCoin[0] : this.txtCoin[1];
        }
        // Effects
        LeanTween.cancel(txtCoin.gameObject);
        LeanTween.scale(txtCoin.gameObject, Vector3.one * 1.5f, 0.05f).setOnComplete(() =>
        {
            LeanTween.scale(txtCoin.gameObject, Vector3.one, 0.25f);
        });
        LeanTween.value(previousValue, value, duration).setOnUpdate((float f) =>
        {
            txtCoin.text = f >= 1000 ? (f / 1000f).ToString("0.0") + "k" : f.ToString("0"); ;
            //txtCoinCollected.text = (f - previousValue).ToString("0");
        }).setOnComplete(callback);
    }

    public void UpdateCoin(TMPro.TextMeshProUGUI txtCoin, int changeValue, float duration = 0.4f, Action callback = null)
    {
        if (txtCoin == null)
        {
            txtCoin = GlobalController.CurrentStage == StageScreen.Home ? this.txtCoin[0] : this.txtCoin[1];
        }
        // Effects
        LeanTween.cancel(txtCoin.gameObject);
        LeanTween.scale(txtCoin.gameObject, Vector3.one * 1.1f, 0.05f).setOnComplete(() =>
        {
            LeanTween.scale(txtCoin.gameObject, Vector3.one, 0.25f);
        });
        LeanTween.value(CoinSystem.Instance.Coin - changeValue, CoinSystem.Instance.Coin, duration).setOnUpdate((float f) =>
        {
            txtCoin.text = f >= 1000 ? (f / 1000f).ToString("0.0") + "k" : f.ToString("0"); ;
        }).setEase(LeanTweenType.linear).setOnComplete(callback);
    }
}
