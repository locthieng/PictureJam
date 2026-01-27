using System;
using TMPro;
using UnityEngine;

public class UIClock : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float countdownTime = 600f;
    [SerializeField] private int[] TimeLevel;

    private float currentTime;
    public bool isRunning = false;
    public bool isActive = false;
    private int timeBoosterClock = 10;

    void OnEnable()
    {
        if (GlobalController.Instance != null)
        {
            countdownTime = TimeLevel[GlobalController.CurrentLevelIndex - 1];
        }
        if (StageController.Instance.isBonusTime) countdownTime += timeBoosterClock;

        ResetAndStart(countdownTime);
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
            if (LevelController.Instance.Level.targetPicture > 0)
            {
                LifeSystem.Instance.ConsumeLife();
                StageController.Instance.End(false);
            }    
            else
            { 
                StageController.Instance.End(true);
            }
        }
    }

    public void ShowClock(bool b)
    {
        isRunning = b;
        isActive = b;
    }

    public void BonusTime(float bonus)
    {
        ResetAndStart(bonus);
    }

    private void ResetAndStart(float time)
    {
        currentTime = time;
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