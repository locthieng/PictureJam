using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; set; }
    public LevelController Level { get; set; }
    public int CurrentLevel { get; set; }
    public bool IsOver { get; set; }
    private float playTimeInSeconds;

    [SerializeField] private Camera cameraGame;
    [SerializeField] private bool noHomeScreen;
    [SerializeField] private bool useSavedLevel;
    [SerializeField] private AudioClip sfxWin;
    public int LevelLimit;
    private int mapIndex;
    private int numGoalsDone;
    private int numTotalGoals;

    public bool IsWaitingForSkinOptions;
    public static bool IsStart { get; set; }
    private const string PlayerEnteredKey = "PlayerEntered";

    private void Awake()
    {
        if (GlobalController.StartSceneName == SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene("Splash");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        showingInter = true;
        StartCoroutine("CoStart");
    }

    IEnumerator CoStart()
    {
        bool hasEnteredBefore = PlayerPrefs.GetInt(PlayerEnteredKey, 0) == 1;
        yield return new WaitForSeconds(0);
        IsOver = false;
        if (!noHomeScreen)
        {
            if (hasEnteredBefore)
            {
                //Debug.Log("Player entered the game for the second time");
                if (GlobalController.CurrentStage == StageScreen.InGame)
                {
                    StartLevel();
                }
                else
                {
                    GlobalController.CurrentStage = StageScreen.Home;
                }
                GameUIController.Instance.SwitchStageUI();
            }
            else
            {
                //Debug.Log("Player entered the game for the first time");
                IsStart = true;
                PlayerPrefs.SetInt(PlayerEnteredKey, 1);
                PlayerPrefs.Save();

                GlobalController.CurrentStage = StageScreen.InGame;
                GameUIController.Instance.SwitchStageUI();
                StartLevel();
            }
        }
        else
        {

            GlobalController.CurrentStage = StageScreen.InGame;
            GameUIController.Instance.SwitchStageUI();
            StartLevel();
        }
    }

    private void StartLevel()
    {
        GlobalController.Instance.ShowBanner();
        if (useSavedLevel)
        {
            if (DataController.Instance != null)
            {
                if (GlobalController.ReplayingLevel > 0)
                {
                    GlobalController.CurrentLevelIndex = GlobalController.ReplayingLevel;
                }
            }
            else
            {
                GlobalController.CurrentLevelIndex = 1;
            }
        }
        mapIndex = LevelController.Instance.LoadLevel(GlobalController.CurrentLevelIndex, LevelLimit);
        StartCoroutine(CoStartLevel());
    }

    IEnumerator CoStartLevel()
    {
        // Setup level
        LevelController.Instance.SetUpLevel();
        yield return new WaitForSeconds(0.02f);
        if (LevelController.Instance.Level != null)
        {
            LevelController.Instance.Level.SetUp();
            numGoalsDone = 0;
            GameUIController.Instance.UpdateLevelProgress(0);
            GameUIController.Instance.ShowInGameUI(GlobalController.CurrentLevelIndex);
            yield return new WaitForSeconds(0.02f);
            LevelController.Instance.Level.StartLevel();
            playTimeInSeconds = Time.realtimeSinceStartup;
        }
    }

    public void StartGame()
    {
        IsStart = true;
        GlobalController.CurrentStage = StageScreen.InGame;
        GameUIController.Instance.ShowLevelBreak(StartLevel);
    }

    public void GetBackToLevelSelector()
    {
        IsStart = false;
        GlobalController.CurrentStage = StageScreen.LevelSelector;
        GameUIController.Instance.ShowLevelBreak(() =>
        {
            if (LevelController.Instance.Level != null)
            {
                Destroy(LevelController.Instance.Level.gameObject);
            }
            GameUIController.Instance.SwitchStageUI();
            GameUIController.Instance.ShowLevelSelector();
        });
    }

    public void GetBackHome()
    {
        GlobalController.CurrentStage = StageScreen.Home;
        ReloadScene();
        if (showingInter)
        {
            GlobalController.Instance.ShowInterstitial();
        }
    }

    public void UpdateScore()
    {
        numGoalsDone++;
        GameUIController.Instance.UpdateLevelProgress(numGoalsDone / (float)numTotalGoals);
        if (numGoalsDone == numTotalGoals)
        {
            End(true);
        }
    }

    public void End(bool win)
    {
        if (IsOver) return;
        IsOver = true;
        StartCoroutine(CoEnd(win));
    }

    IEnumerator CoEnd(bool win)
    {
        if (win)
        {
            //yield return new WaitForSeconds(2f);
            if (!DataController.Instance.Data.Levels.Contains(GlobalController.CurrentLevelIndex) || GlobalController.CurrentLevelIndex == 1)
            {
                DataController.Instance.Data.Levels.Add(GlobalController.CurrentLevelIndex);
                //AnalyticsController.Instance.LogLevelComplete(GlobalController.CurrentLevelIndex, (int)(Time.realtimeSinceStartup - playTimeInSeconds), GlobalController.ReplayCount);
                //AnalyticsController.Instance.LogCustomEvent("checkpoint_" + (GlobalController.CurrentLevelIndex).ToString("000"), "", "");
                //AppsflyerController.Instance.LogCustomEvent(AFInAppEvents.LEVEL_ACHIEVED, AFInAppEvents.LEVEL, (GlobalController.CurrentLevelIndex).ToString("000"));
            }
            DataController.Instance.Data.LevelIndex = GlobalController.CurrentLevelIndex = LevelController.Instance.GetNextLevelInOrder();
            DataController.Instance.SaveData();
            StartCoroutine(CoShowEndGameUI(true));
            GlobalController.ReplayCount = 0;
        }
        else
        {
            yield return new WaitForSeconds(0f);
            //AnalyticsController.Instance.LogLevelFail(GlobalController.CurrentLevelIndex, (int)(Time.realtimeSinceStartup - playTimeInSeconds), GlobalController.ReplayCount);
            StartCoroutine(CoShowEndGameUI(false));
        }
    }

    int earning;
    int earningX;
    public bool showingInter { get; set; }

    public int BonusEarning;

    public void WatchAdsEarnX()
    {
        GlobalController.Instance.ShowRewardedVideo(EarnRewardX, "BonusCoin");
    }

    private void EarnRewardX()
    {
        //AnalyticsController.Instance.LogEarnCurrency("cash", earningX, "x5watchads");
        DataController.Instance.SaveData();
        showingInter = false;
    }

    IEnumerator CoShowEndGameUI(bool win)
    {
        //GameUIController.Instance.SetEndResultLabelActive(win, true);
        if (win)
        {
            SoundController.Instance.PlaySingle(sfxWin);
            GameUIController.Instance.ShowConfetti();
            yield return new WaitForSeconds(0.5f);
            GameUIController.Instance.ShowGameEnd(win);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            GameUIController.Instance.ShowGameEnd(win);
        }
        CameraController.Instance.EndCam(ShowEndAds);
    }

    public void WatchAdsHint()
    {
        GlobalController.Instance.ShowRewardedVideo(LevelController.Instance.ShowHint, "Hint");
    }

    private void ShowEndAds()
    {
        GlobalController.Instance.ShowMREC();
    }

    public void WatchAdsSkipLevel()
    {
        GlobalController.Instance.ShowRewardedVideo(SkipLevel, "SkipLevel");
        //AnalyticsController.Instance.LogCustomEvent("skip_level_" + GlobalController.CurrentLevelIndex.ToString("000"), "level", GlobalController.CurrentLevelIndex.ToString("000"));
    }

    private void SkipLevel()
    {
        showingInter = false;
        //AnalyticsController.Instance.LogLevelSkip(DataController.Instance.Data.LevelIndex, (int)(Time.realtimeSinceStartup - playTimeInSeconds), GlobalController.ReplayCount);
        if (!DataController.Instance.Data.Levels.Contains(DataController.Instance.Data.LevelIndex))
        {
            DataController.Instance.Data.Levels.Add(DataController.Instance.Data.LevelIndex);
        }
        GlobalController.ReplayCount = 0;
        GlobalController.CurrentLevelIndex++;

        if (GlobalController.CurrentLevelIndex > DataController.Instance.Data.LevelIndex)
        {
            DataController.Instance.Data.LevelIndex = GlobalController.CurrentLevelIndex;
        }
        DataController.Instance.SaveData();
        NextLevel();
    }

    public void Next()
    {
        //DataController.Instance.Data.Coin += earning;
        CoinSystem.Instance.AddCoin(earning);
        DataController.Instance.SaveData();
        if (DataController.Instance.Data.LevelIndex % 4 == 0 && !GlobalController.IsRated)
        {
            GameUIController.Instance.ShowRateUs(NextLevel, NextLevelAfterRate);
        }
        else
        {
            NextLevel();
        }
    }

    private void NextLevelAfterRate()
    {
        //showingInter = false;
        NextLevel();
    }

    public void NextLevel()
    {
        GlobalController.ReplayingLevel = 0;
        GameUIController.Instance.ShowLevelBreak(ReloadScene);
        if (showingInter)
        {
            GlobalController.Instance.ShowInterstitial();
        }
    }

    public void ForceStartLevel(int level)
    {
        IsStart = true;
        GameUIController.Instance.HideLevelSelector();
        GlobalController.CurrentStage = StageScreen.InGame;
        GlobalController.ReplayingLevel = 0;
        GlobalController.CurrentLevelIndex = level;
        GameUIController.Instance.ShowLevelBreak(() =>
        {
            mapIndex = LevelController.Instance.LoadLevel(GlobalController.CurrentLevelIndex, LevelLimit);
            StartCoroutine(CoStartLevel());
        });
    }

    public void Restart()
    {
        GlobalController.ReplayingLevel = mapIndex;
        GlobalController.ReplayCount++;
        GameUIController.Instance.ShowLevelBreak(ReloadScene);
        if (showingInter)
        {
            GlobalController.Instance.ShowInterstitial();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        //GoogleAdsController.Instance.RequestNativeAd();
    }
}
