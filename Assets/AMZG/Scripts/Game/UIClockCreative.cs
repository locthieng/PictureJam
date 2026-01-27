using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIClockCreative : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float countdownTime;
    [SerializeField] public UIProgressCreative uiProgressCreative;
    private float currentTime;
    public bool isRunning = false;
    public bool isActive = false;
    public bool isHighLight;

    public GameObject vienDo;

    void OnEnable()
    {
        ResetAndStart();
    }

    void Update()
    {
        if (!isRunning || !isActive) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) currentTime = 0;
            UpdateCountdownText(currentTime);

            if (currentTime < 3 && !isHighLight)
            {
                isHighLight = true;
                //StartCoroutine(HighLight());
            }    
        }
        else
        {
            isRunning = false;
            ShowClock(false);

            GameUIController.Instance.isFreeze = false;
            GameUIController.Instance.SetClock(true);
            GameUIController.Instance.SetInteractBoosterUI(true);
        }
    }

    public void ShowClock(bool b)
    {
        isRunning = b;
        isActive = b;
        uiProgressCreative.SetProgress(0f, countdownTime, () =>
        {
            uiProgressCreative.fill.fillAmount = 1;
        });
        gameObject.SetActive(b);
        uiProgressCreative.gameObject.SetActive(b);
    }

    private void ResetAndStart()
    {
        currentTime = countdownTime;
        isRunning = true;
        UpdateCountdownText(currentTime);
    }

    private void UpdateCountdownText(float timeRemaining)
    {
        timeRemaining = Mathf.Max(0, timeRemaining);
        TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
        //timeText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
        timeText.text = Mathf.FloorToInt(timeRemaining).ToString();
    }

    IEnumerator HighLight()
    {
        yield return null;
        vienDo.SetActive(true);
        vienDo.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        uiProgressCreative.fill.color = Color.white;
        LeanTween.value(vienDo,0f, 1f, 0.3f).setOnUpdate((float f) =>
        {
            vienDo.GetComponent<Image>().color = new Color(1, 1, 1, f);
        });
        yield return new WaitForSeconds(0.3f);
        uiProgressCreative.fill.color = Color.red;
        LeanTween.value(vienDo, 1f, 0f, 0.3f).setOnUpdate((float f) =>
        {
            vienDo.GetComponent<Image>().color = new Color(1, 1, 1, f);
        });
        yield return new WaitForSeconds(0.3f);
        vienDo.SetActive(false);
        StartCoroutine(HighLight());
    }    
}