using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILife : MonoBehaviour
{
    [SerializeField] private UIDigitalClock lifeClock;
    [SerializeField] private GameObject imageInfinite;
    [SerializeField] private TMPro.TextMeshProUGUI txtLife;
    [SerializeField] private Image imageLifePlus;
    [SerializeField] private UIButton buttonLifeField;

    LifeSystem lifeSystem => LifeSystem.Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        lifeClock.OnEnd = OnLifeClockEnd;
        lifeSystem.OnLifeChanged = RefreshUI;
        lifeSystem.CheckLifeFrozenStatus();
        RefreshUILife();
    }    

    public void RefreshUILife()
    {
        if (lifeSystem.IsInfinite)
        {
            buttonLifeField.enabled = false;
            imageLifePlus.color = new Color(1, 1, 1, 0.0f);
        }
        else
        {
            if (lifeSystem.MaxLife > lifeSystem.CurrentLife)
            {
                buttonLifeField.enabled = true;
                imageLifePlus.color = Color.white;
                if (!string.IsNullOrEmpty(lifeSystem.lastLifeFillTime.ToString()))
                {
                    int lifeRefillProgressTime = (int)(DateTime.Now - DateTime.Parse(lifeSystem.lastLifeFillTime.ToString())).TotalSeconds;
                    int remainLifeRefillTime = lifeSystem.refillSeconds - lifeRefillProgressTime;
                    if (remainLifeRefillTime <= 0)
                    {
                        int refilledLives = lifeRefillProgressTime / lifeSystem.refillSeconds;
                        if (refilledLives > lifeSystem.MaxLife - lifeSystem.CurrentLife)
                        {
                            refilledLives = lifeSystem.MaxLife - lifeSystem.CurrentLife;
                        }
                        remainLifeRefillTime = lifeRefillProgressTime - lifeSystem.refillSeconds * refilledLives;
                        lifeSystem.lastLifeFillTime = DateTime.Now.AddSeconds(-remainLifeRefillTime);
                        //GlobalController.Instance.OnLifeChange(refilledLives);
                        lifeSystem.AddLife(refilledLives);

                    }
                    if (lifeSystem.MaxLife > lifeSystem.CurrentLife && remainLifeRefillTime > 0)
                    {
                        lifeClock.OnEnd = OnLifeClockEnd;
                        lifeClock.SetUpTimer(remainLifeRefillTime);
                        lifeClock.StartTimer();

                    }
                    else if (lifeSystem.MaxLife == lifeSystem.CurrentLife)
                    {
                        buttonLifeField.enabled = false;
                        imageLifePlus.color = new Color(1, 1, 1, 0.0f);
                        lifeClock.SetTimerText("MAX");
                    }
                }
            }
            else
            {
                lifeClock.Stop();
                buttonLifeField.enabled = false;
                imageLifePlus.color = new Color(1, 1, 1, 0.0f);
                lifeClock.SetTimerText("MAX");
            }
        }

        imageInfinite.SetActive(lifeSystem.IsInfinite);
        txtLife.gameObject.SetActive(!lifeSystem.IsInfinite);
        txtLife.text = lifeSystem.CurrentLife.ToString();
        lifeSystem.Save();
    }    

    // nếu hết giờ 
    private void OnLifeClockEnd()
    {
        lifeSystem.AddLife(1,
            ()=>
            {
                // max 
                txtLife.gameObject.SetActive(false);
                imageInfinite.SetActive(true);
                lifeClock.SetTimerText("MAX");
                imageLifePlus.color = new Color(1, 1, 1, 0.0f);
                buttonLifeField.enabled = false;
                lifeClock.Stop();
            },
            () =>
            {
                // chưa max
                txtLife.gameObject.SetActive(true);
                imageInfinite.SetActive(false);
            });
        lifeClock.SetUpTimer(lifeSystem.RefillSeconds);
        lifeClock.StartTimer();
    }

    public void RefreshUI(int currenLife)
    {
        txtLife.text = currenLife.ToString();

        if (currenLife == lifeSystem.MaxLife) lifeClock.SetTimerText("MAX");
    }

    public void ShowUIAddLife()
    {

    }    
}
