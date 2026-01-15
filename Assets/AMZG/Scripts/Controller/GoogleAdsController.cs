//using System;
//using System.Drawing;
//using GoogleMobileAds.Api;
//using GoogleMobileAds.Ump.Api;
//using Newtonsoft.Json.Linq;
//using UnityEditor.PackageManager;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class GoogleAdsController : Singleton<GoogleAdsController>
//{
//#if UNITY_ANDROID
//    [SerializeField] private string app_id = "";
//    [SerializeField] private string aoa_unit_id = "";
//    [SerializeField] private string collap_ad_unit_id = "";
//    [SerializeField] private string inter_unit_id = "";
//    [SerializeField] private string native_unit_id = "";
//#elif UNITY_IOS
//    [SerializeField] private string app_id = "";
//    [SerializeField] private string aoa_unit_id = "";
//    [SerializeField] private string collap_ad_unit_id = "";
//    [SerializeField] private string inter_unit_id = "";
//    [SerializeField] private string native_unit_id = "";
//#else
//    [SerializeField] private string app_id = "";
//    [SerializeField] private string aoa_unit_id = "";
//    [SerializeField] private string collap_ad_unit_id = "";
//    [SerializeField] private string inter_unit_id = "";
//    [SerializeField] private string native_unit_id = "";
//#endif
//    public bool OverrideAOARemoteSetting;
//    public bool AOAEnable;
//    private AppOpenAd aoaAd;
//    public bool IsInitialized { get; set; }
//    public bool isShowingAOAAd = false;
//    private DateTime loadTime;

//    public bool IsAOAAdAvailable
//    {
//        get
//        {
//            return aoaAd != null;// && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
//        }
//    }

//    public bool ShowConsentForm { get; internal set; }
//    public int CollapAdsLevels = 3;

//    private void Start()
//    {
//        AOAEnable = !GlobalController.IsRu;
//        Init();
//    }

//    public void Init()
//    {
//        Debug.Log("Google Ads initializing...");
//        MobileAds.Initialize(OnInit);
//        // Set tag for under age of consent.
//        // Here false means users are not under age of consent.
//        //ConsentRequestParameters request = new ConsentRequestParameters
//        //{
//        //    TagForUnderAgeOfConsent = false,
//        //};

//        //// Check the current consent information status.
//        //ConsentInformation.Update(request, OnConsentInfoUpdated);
//    }

//    public NativeAd nativeAd;
//    public NativeAd nativeAdForLevelSelector;
//    public NativeAd nativeAdForSpecialLevelPopup;
//    public NativeAd nativeAdForSpecialLevelSelector;
//    private GameObject icon;
//    public bool nativeAdLoaded;

//    public void RequestNativeAd()
//    {
//        if (GlobalController.Instance.ForTesting) return;
//        if (nativeAdLoaded)
//        {
//            return;
//        }
//        Debug.Log("Request native ad");
//        AdLoader adLoader = new AdLoader.Builder(native_unit_id)
//            .ForNativeAd()
//            .Build();
//        adLoader.OnNativeAdLoaded += HandleNativeAdLoaded;
//        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
//        adLoader.LoadAd(new AdRequest());
//    }

//    public void RequestNativeAdForLevelSelector()
//    {
//        if (GlobalController.Instance.ForTesting) return;
//        if (nativeAdForLevelSelector != null)
//        {
//            return;
//        }
//        Debug.Log("Request native ad for level selector");
//        AdLoader adLoader = new AdLoader.Builder(native_unit_id)
//            .ForNativeAd()
//            .Build();
//        adLoader.OnNativeAdLoaded += HandleNativeAdLoadedForLevelSelector;
//        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
//        adLoader.LoadAd(new AdRequest());
//    }

//    public void RequestNativeAdForSpecialLevelPopup()
//    {
//        if (GlobalController.Instance.ForTesting) return;
//        if (nativeAdForSpecialLevelPopup != null)
//        {
//            return;
//        }
//        Debug.Log("Request native ad for special level");
//        AdLoader adLoader = new AdLoader.Builder(native_unit_id)
//            .ForNativeAd()
//            .Build();
//        adLoader.OnNativeAdLoaded += HandleNativeAdLoadedForSpecialLevel;
//        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
//        adLoader.LoadAd(new AdRequest());
//    }

//    public void RequestNativeAdForSpecialLevelSelector()
//    {
//        if (GlobalController.Instance.ForTesting) return;
//        if (nativeAdForSpecialLevelSelector != null)
//        {
//            return;
//        }
//        Debug.Log("Request native ad for special level selector");
//        AdLoader adLoader = new AdLoader.Builder(native_unit_id)
//            .ForNativeAd()
//            .Build();
//        adLoader.OnNativeAdLoaded += HandleNativeAdLoadedForSpecialLevelSelector;
//        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
//        adLoader.LoadAd(new AdRequest());
//    }

//    private NativeAdsObject nao;

//    public void ParseNativeAdsObject(NativeAdsObject nao)
//    {
//        if (DataController.Instance.Stats.NoAds)
//        {
//            return;
//        }
//        this.nao = nao;
//    }

//    void Update()
//    {
//        if (nativeAdLoaded && nao != null)
//        {
//            nativeAdLoaded = false;
//            nao.Parse(nativeAd);
//        }
//    }

//    public void ParseNativeAdToLevelSelector(NativeAdsObject nativeAdsObject)
//    {
//        if (DataController.Instance.Stats.NoAds)
//        {
//            return;
//        }
//        nativeAdsObject.Parse(nativeAdForLevelSelector);
//        if (nativeAdForLevelSelector != null)
//        {
//            nativeAdForLevelSelector = null;
//            RequestNativeAdForLevelSelector();
//        }
//    }

//    public void ParseNativeAdToSpecialLevelPopup(NativeAdsObject nativeAdsObject)
//    {
//        if (DataController.Instance.Stats.NoAds)
//        {
//            return;
//        }
//        nativeAdsObject.Parse(nativeAdForSpecialLevelPopup);
//        if (nativeAdForSpecialLevelPopup != null)
//        {
//            nativeAdForSpecialLevelPopup = null;
//            RequestNativeAdForSpecialLevelPopup();
//        }
//    }

//    public void ParseNativeAdToSpecialLevelSelector(NativeAdsObject nativeAdsObject)
//    {
//        if (DataController.Instance.Stats.NoAds)
//        {
//            return;
//        }
//        nativeAdsObject.Parse(nativeAdForSpecialLevelSelector);
//        if (nativeAdForSpecialLevelSelector != null)
//        {
//            nativeAdForSpecialLevelSelector = null;
//            RequestNativeAdForSpecialLevelSelector();
//        }
//    }

//    public void ParseNativeAdTo(NativeAdsObject nativeAdsObject)
//    {
//        if (nativeAd != null)
//        {
//            nativeAdsObject.Parse(nativeAd);
//        }
//    }

//    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
//    {
//        Debug.Log("Native ad loaded.");
//        nativeAd = args.nativeAd;
//        nativeAdLoaded = true;
//    }

//    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//    {
//        Debug.Log("Native ad failed to load: " + args.LoadAdError.GetMessage());
//        //RequestNativeAd();
//    }

//    private void HandleNativeAdLoadedForLevelSelector(object sender, NativeAdEventArgs args)
//    {
//        Debug.Log("Native ad loaded for level selector.");
//        nativeAdForLevelSelector = args.nativeAd;
//    }

//    private void HandleNativeAdLoadedForSpecialLevel(object sender, NativeAdEventArgs args)
//    {
//        Debug.Log("Native ad loaded for special level.");
//        nativeAdForSpecialLevelPopup = args.nativeAd;
//    }

//    private void HandleNativeAdLoadedForSpecialLevelSelector(object sender, NativeAdEventArgs args)
//    {
//        Debug.Log("Native ad loaded for special level selector.");
//        nativeAdForSpecialLevelSelector = args.nativeAd;
//    }

//    void OnConsentInfoUpdated(FormError consentError)
//    {
//        if (consentError != null)
//        {
//            // Handle the error.
//            UnityEngine.Debug.LogError(consentError);
//            return;
//        }
//        if (ShowConsentForm)
//        {
//            // If the error is null, the consent information state was updated.
//            // You are now ready to check if a form is available.
//            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
//            {
//                if (formError != null)
//                {
//                    // Consent gathering failed.
//                    UnityEngine.Debug.LogError(consentError);
//                    return;
//                }

//                // Consent has been gathered.
//            });
//        }
//    }

//    private void OnInit(InitializationStatus obj)
//    {
//        Debug.Log("Google Ads init. AOA is " + (AOAEnable ? "enabled" : "disabled") + " on RemoteConfig.");
//        IsInitialized = true;
//        if (AOAEnable)
//        {
//            LoadAOA();
//        }
//        LoadInterstitialAd();
//        RequestNativeAd();
//        RequestNativeAdForLevelSelector();
//        RequestNativeAdForSpecialLevelPopup();
//        RequestNativeAdForSpecialLevelSelector();
//    }

//    public void LoadBannerAd(bool isCollapsible)
//    {
//        var bannerView = new BannerView(collap_ad_unit_id, AdSize.Banner, AdPosition.Bottom);

//        var adRequest = new AdRequest();

//        // Create an extra parameter that aligns the bottom of the expanded ad to the
//        // bottom of the bannerView.
//        if (isCollapsible)
//        {
//            adRequest.Extras.Add("collapsible", "bottom");
//        }

//        bannerView.LoadAd(adRequest);
//    }

//    private InterstitialAd _interstitialAd;

//    /// <summary>
//    /// Loads the interstitial ad.
//    /// </summary>
//    public void LoadInterstitialAd()
//    {
//        // Clean up the old ad before loading a new one.
//        if (_interstitialAd != null)
//        {
//            _interstitialAd.Destroy();
//            _interstitialAd = null;
//        }

//        Debug.Log("Loading the interstitial ad.");

//        // create our request used to load the ad.
//        var adRequest = new AdRequest();

//        // send the request to load the ad.
//        InterstitialAd.Load(inter_unit_id, adRequest,
//            (InterstitialAd ad, LoadAdError error) =>
//            {
//                // if error is not null, the load request failed.
//                if (error != null || ad == null)
//                {
//                    Debug.LogError("interstitial ad failed to load an ad " +
//                                   "with error : " + error);
//                    return;
//                }

//                Debug.Log("Interstitial ad loaded with response : "
//                          + ad.GetResponseInfo());

//                _interstitialAd = ad;
//                RegisterEventHandlers(_interstitialAd);
//            });
//    }

//    /// <summary>
//    /// Shows the interstitial ad.
//    /// </summary>
//    public void ShowInterstitialAd()
//    {
//        if (_interstitialAd != null && _interstitialAd.CanShowAd())
//        {
//            Debug.Log("Showing interstitial ad.");
//            _interstitialAd.Show();
//        }
//        else
//        {
//            Debug.LogError("Interstitial ad is not ready yet.");
//        }
//    }

//    private void RegisterEventHandlers(InterstitialAd interstitialAd)
//    {
//        // Raised when the ad is estimated to have earned money.
//        interstitialAd.OnAdPaid += (AdValue adValue) =>
//        {
//            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
//                adValue.Value,
//                adValue.CurrencyCode));
//        };
//        // Raised when an impression is recorded for an ad.
//        interstitialAd.OnAdImpressionRecorded += () =>
//        {
//            Debug.Log("Interstitial ad recorded an impression.");
//        };
//        // Raised when a click is recorded for an ad.
//        interstitialAd.OnAdClicked += () =>
//        {
//            Debug.Log("Interstitial ad was clicked.");
//        };
//        // Raised when an ad opened full screen content.
//        interstitialAd.OnAdFullScreenContentOpened += () =>
//        {
//            Debug.Log("Interstitial ad full screen content opened.");
//        };
//        // Raised when the ad closed full screen content.
//        interstitialAd.OnAdFullScreenContentClosed += () =>
//        {
//            Debug.Log("Interstitial ad full screen content closed.");
//            // Reload the ad so that we can show another as soon as possible.
//            LoadInterstitialAd();
//        };
//        // Raised when the ad failed to open full screen content.
//        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
//        {
//            Debug.LogError("Interstitial ad failed to open full screen content " +
//                           "with error : " + error);
//            // Reload the ad so that we can show another as soon as possible.
//            LoadInterstitialAd();
//        };
//    }

//    public void LoadAOA()
//    {
//        AdRequest request = new AdRequest();
//        Debug.Log("Google Ads: Loading AOA...");
//        // Load an app open ad for portrait orientation
//        AppOpenAd.Load(aoa_unit_id, request, ((appOpenAd, error) =>
//        {
//            if (error != null)
//            {
//                // Handle the error.
//                Debug.LogErrorFormat("Failed to load the AOA. (reason: {0})", error.GetMessage());
//                return;
//            }

//            // App open ad is loaded.
//            aoaAd = appOpenAd;
//            loadTime = DateTime.UtcNow;
//        }));
//    }

//    private Action onAOADismissed;

//    public bool ShowAOAAdIfAvailable(Action onClose)
//    {
//        if (DataController.Instance.Stats.NoAds || GlobalController.Instance.ForTesting || !IsAOAAdAvailable || isShowingAOAAd || !AOAEnable || (MAXAdsController.Instance != null && MAXAdsController.Instance.IsShowingFullscreenAds()))
//        {
//            return false;
//        }
//        onAOADismissed = onClose;
//        aoaAd.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
//        aoaAd.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
//        aoaAd.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
//        aoaAd.OnAdImpressionRecorded += HandleAdDidRecordImpression;
//        aoaAd.OnAdPaid += HandlePaidEvent;
//        isShowingAOAAd = true;
//        aoaAd.Show();
//        return true;
//    }

//    private void HandleAdDidDismissFullScreenContent()
//    {
//        Debug.Log("Closed app open ad");
//        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
//        aoaAd = null;
//        isShowingAOAAd = false;
//        LoadAOA();
//        onAOADismissed?.Invoke();
//    }

//    private void HandleAdFailedToPresentFullScreenContent(AdError args)
//    {
//        Debug.LogFormat("Failed to present the ad (reason: {0})", args.GetMessage());
//        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
//        aoaAd = null;
//        isShowingAOAAd = false;
//        LoadAOA();
//    }

//    private void HandleAdDidPresentFullScreenContent()
//    {
//        Debug.Log("Displayed app open ad");
//        isShowingAOAAd = true;
//    }

//    private void HandleAdDidRecordImpression()
//    {
//        Debug.Log("Recorded ad impression");
//    }

//    private void HandlePaidEvent(AdValue args)
//    {
//        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
//                args.CurrencyCode, args.Value);
//    }
//}