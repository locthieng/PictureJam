using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIRevive : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject content;

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

    public void ButtonNoThanks()
    {
        LeanTween.alphaCanvas(canvas, 0, 0.2f).setDelay(0.2f).setOnComplete(() =>
        {
            canvas.blocksRaycasts = false;
        });
        StageController.Instance.AfterClose(false);
    }

   
}
