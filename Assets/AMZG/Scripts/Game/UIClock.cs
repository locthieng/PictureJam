using System;
using TMPro;
using UnityEngine;

public class UIClock : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float countdownTime = 360f;

    private float currentTime;
    public bool isRunning = false;
    public bool isActive = false;

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
        }
        else
        {
            isRunning = false;
            StageController.Instance.End(false);
        }
    }

    public void ShowClock(bool b)
    {
        isRunning = b;
        isActive = b;
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
        timeText.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }
}