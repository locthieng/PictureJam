using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIRevive : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject content;
    public int CoinToContinue = 900;

    public void Show()
    {

        content.transform.localScale = Vector3.one * 0.6f;
        LeanTween.alphaCanvas(canvas, 1, 0.2f).setOnComplete(() =>
        {
            canvas.blocksRaycasts = true;
        });
        LeanTween.scale(content, Vector3.one, 0.2f);
    }

    public void Close()
    {
        //StageController.Instance.isEarnTime = false;
        LeanTween.alphaCanvas(canvas, 0, 0.2f).setDelay(0.2f).setOnComplete(() =>
        {
            canvas.blocksRaycasts = false;
        });
    }

    public void UseCoinToContinue()
    {
        if (CoinSystem.Instance.coin >= CoinToContinue)
        {
            CoinSystem.Instance.coin -= CoinToContinue;

            canvas.blocksRaycasts = false;
            UICoin.Instance.UpdateCoin(null, -CoinToContinue, 0.4f, () =>
            {
                UICoin.Instance.txtCoin[0].text = CoinSystem.Instance.coin.ToString();
                StageController.Instance.UseCoinToContinue();
                UICoin.Instance.UpdateCoin(null, 0);
                Close();
            });
        }
        else
        {
            // Show Shop

            //GameUIController.Instance.UICoinPlus.Show();
        }
    }    

    public void ButtonNoThanks()
    {
        LeanTween.alphaCanvas(canvas, 0, 0.2f).setDelay(0.2f).setOnComplete(() =>
        {
            canvas.blocksRaycasts = false;
        });
        StageController.Instance.AfterClose(false);
    }



   
}
