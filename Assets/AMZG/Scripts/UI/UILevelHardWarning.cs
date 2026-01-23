using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelHardWarning : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image bg;
    [SerializeField] private Image banner;
    [SerializeField] private Image hardLevel;
    [SerializeField] private float speed;
    
    void Start()
    {
        
    }

    private int tweenId;

    public void ShowWarningLevelHard()
    {
        Refresh();
        canvasGroup.LeanAlpha(1, 1f);
        canvasGroup.blocksRaycasts = true;
        hardLevel.transform.LeanScale(Vector3.one * 0.8f, 2f).setOnComplete(() =>
        {
            canvasGroup.LeanAlpha(0, 0.5f).setOnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                LeanTween.cancel(tweenId);
            });
            
        });
        tweenId = LeanTween.value(banner.rectTransform.offsetMin.x, -1500, 0.1f).setSpeed(speed).setOnUpdate((float f) =>
        {
            Vector2 offsetMin = banner.rectTransform.offsetMin;
            offsetMin.x = f;
            banner.rectTransform.offsetMin = offsetMin;
        }).id;

    }

    private void Refresh()
    {
        
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        hardLevel.transform.localScale = Vector3.one * 1.2f;
        Vector2 offsetMin = banner.rectTransform.offsetMin;
        offsetMin.x = 0;
        banner.rectTransform.offsetMin = offsetMin;
    }
}
