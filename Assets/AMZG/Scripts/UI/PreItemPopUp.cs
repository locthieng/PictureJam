using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreItemPopUp : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject content;

    private BoosterItemData data;

    public void SetUp(BoosterItemData data, string title = null, string content = null)
    {
        this.data = data;
    }


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

    public void ButtonPlay()
    {
    }


}
