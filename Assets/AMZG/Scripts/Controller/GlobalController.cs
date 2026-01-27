using UnityEngine;
using UnityEngine.Events;
using System;
//using static MaxSdkCallbacks;

#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
//using Google.Play.Review;
#endif

public class MissionSuccessEvent : UnityEvent<int> { }

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance { get; set; }
    public static Vector2 ScreenResolution = new Vector2(1080, 1920);
    public static float StageCameraSize = 5;
    public static Vector2 FixedStageResolution = new Vector2(1080, 1920);
    public static float ScreenRatio { get { return Screen.width / (float)Screen.height; } }

    public static bool IsTutDone { get; set; }
    public static bool IsHapticOn { get; set; }
    public static bool IsSoundOn { get; set; }
    public static bool IsMusicOn { get; set; }
    public static bool IsDailyShown { get; set; }
    public static int LoginDay { get; set; }
    public static int RewardDay { get; set; }

    public static bool IsRated { get; set; }
    public static int PlayTimes { get; set; }
    public static Vector2 ScreenSize
    {
        get
        {
            return new Vector2(Screen.width * StageCameraSize * 2 / Screen.height, StageCameraSize * 2);
        }
    }
    public static int TotalLoseTimes;
    public static int TotalDistance;
    public static int CurrentLevelIndex;
    public static int CurrentLevelSpecialIndex;
    public static int CurrentLevelInGame;
    public static string StartSceneName = "Game";
    public static int ReplayingLevel;
    public static int ReplayCount { get; internal set; }
    public static StageScreen CurrentStage { get; internal set; }
    public static LevelType CurrentLevelType;

    public static bool IsBgmOn { get; internal set; }
    public bool ForTesting = false;
    public static int NumLevelsTillSpecial;
    public static int SpecialLevelGap = 8;
    [HideInInspector] public int bonusTime = 10;
    [HideInInspector]
    //public float adDuration;
    public bool IsInfiniteLife { get; set; }
    //public int home_after_win_milestone;

    public static int MaxLife = 5;
    public static int LifeRefillTime = 900; // second(s)

    public static bool IsRu
    {
        get
        {
            return System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName == "RU";
        }
    }

    private void Awake()
    {
        Instance = this;
        // Re-adjust stage camera
        LeanTween.init(2000);
        Application.targetFrameRate = 60;
        StartSceneName = "Game";
        CurrentStage = StageScreen.Home;
    }

    private void Start()
    {
        Debug.Log("Country Code: " + System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName);
        if (DataController.Instance.Data.LevelIndex <= 0)
        {
            DataController.Instance.Data.LevelIndex = 1;
        }
        CurrentLevelIndex = DataController.Instance.Data.LevelIndex;
    }

    // Ads
    public void ShowBanner()
    {
        if (!DataController.Instance.Stats.NoAds && !ForTesting)
        {
            //if (IsRu)
            //{
            //    MAXAdsController.Instance.ShowBanner();
            //}
            //else
            //{
            //    GoogleAdsController.Instance.LoadBannerAd(false);
            //}
        }
    }

    public void HideBanner()
    {
        if (IsRu)
        {
            //MAXAdsController.Instance.HideBanner();
        }
        else
        {
        }
    }

    public void ShowMREC()
    {
        if (!DataController.Instance.Stats.NoAds && !ForTesting)
        {
            //if (IsRu)
            //{
            //    MAXAdsController.Instance.ShowMREC();
            //}
            //else
            //{
            //    GoogleAdsController.Instance.LoadBannerAd(true);
            //}
        }
    }

    public void ShowInterstitial()
    {
        if (!DataController.Instance.Stats.NoAds && !ForTesting)
        {
            //MAXAdsController.Instance.ShowInter();
        }
    }

    public void ShowRewardedVideo(UnityAction callback, string placement)
    {
        UnityEvent e = new UnityEvent();
        e.AddListener(callback);
#if UNITY_EDITOR || UNITY_STANDALONE
        e.Invoke();
#else
        if (ForTesting)
        {
            e.Invoke();
        }
        else
        {
            //MAXAdsController.Instance.ShowRewarded(e, placement);
        }
#endif
    }

    public bool ShowAOA(Action onClose)
    {
        //if (!DataController.Instance.Stats.NoAds && !ForTesting)
        //{
        //    if (IsRu)
        //    {
        //        return MAXAdsController.Instance.ShowAOA(onClose);
        //    }
        //    else
        //    {
        //        return GoogleAdsController.Instance.ShowAOAAdIfAvailable(onClose);
        //    }
        //}
        //else
        //{
            return false;
        //}
    }

    private int appOpen;

    public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded
        appOpen = PlayerPrefs.GetInt("AppOpenCount", 0);
        if (!paused)
        {
            ShowAOA(null);
        }
    }

    // Social
    public void ShareScore(UnityEvent e)
    {
        //FacebookController.Instance.Share();
    }

    public void RateGame()
    {
        IsRated = true;
        PlayerPrefs.SetInt("RateGame", 1);
#if UNITY_IOS
        if (Device.RequestStoreReview())
        {
        }
        else
        {
        }
#elif UNITY_ANDROID
        //ReviewManager reviewManager = new ReviewManager();
        //var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

        //// define a callback after the preloading is done
        //playReviewInfoAsyncOperation.Completed += playReviewInfoAsync =>
        //{
        //    if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
        //    {
        //        // display the review prompt
        //        var playReviewInfo = playReviewInfoAsync.GetResult();
        //        reviewManager.LaunchReviewFlow(playReviewInfo);
        //    }
        //    else
        //    {
        //        // handle error when loading review prompt
        //    }
        //};
#endif
    }

    public void MoreGames()
    {
    }

    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            //GoogleServiceController.Instance.SignIn(Social.ShowLeaderboardUI);
        }
    }

    public static void LikePageInstagram(MissionSuccessEvent e)
    {
        // on success
        if (e != null)
        {
            e.Invoke(0);
        }
    }

    public static void FollowTwitter(MissionSuccessEvent e)
    {
        // on success
        if (e != null)
        {
            e.Invoke(1);
        }
    }

    public static void SubscribeYoutube(MissionSuccessEvent e)
    {
        // on success
        if (e != null)
        {
            e.Invoke(2);
        }
    }

    public static void LikePageFacebook(MissionSuccessEvent e)
    {
        // on success
        if (e != null)
        {
            e.Invoke(3);
        }
    }

    // IAP
    //    public void PurchaseDiamond(DiamondPackID value, IAPDiamondPackEvent e)
    //    {

    //#if UNITY_EDITOR || UNITY_STANDALONE
    //        e.Invoke(value);
    //#else
    //        IAPController.Instance.BuyDiamondPack(value, e);
    //#endif
    //    }

    public void PurchaseNoAds(Action e)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        e.Invoke();
#else
        IAPController.Instance.BuyNoAds(e);
#endif
    }

    public void RestorePurchase()
    {
        //IAPController.Instance.RestorePurchases();
    }
}
