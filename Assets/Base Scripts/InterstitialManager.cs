using EasyMobile;
#if EM_ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InterstitialPlacement {
    ReturnToMenu, BetweenLevels
}

public class InterstitialManager : MonoBehaviour
{
    public static InterstitialManager Instance;

    public static DateTime lastAdTime = DateTime.MinValue;

    static AdPlacement returnToMenu = null;
    static AdPlacement betweenLevels = null;

    static Action callback = null;
    static bool callbackCurrent = false;
    static string lastPlacementName = "";
    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        else Instance = this;
        DontDestroyOnLoad(this);
        returnToMenu = AdPlacement.PlacementWithName("ReturnToMenuInterstitial");
        betweenLevels = AdPlacement.PlacementWithName("BetweenLevelsInterstitial");
        
    }

    private void Start(){
        Invoke("LoadInterstitialDelayed", 6.0f);
    }

    private void LoadInterstitialDelayed() {
        if (!AdManager.AreRemoved())
        {
            if (EasyMobile.RuntimeManager.IsInitialized())
            {
                //Advertising.LoadInterstitialAd(returnToMenu);
                Advertising.LoadInterstitialAd(betweenLevels);
            }
        }
    }
#if EM_ADMOB
    private static void InterstitialFailed(object sender, AdFailedToLoadEventArgs args) {
        Debug.LogWarning("Failed to load interstitial: " + args.Message);
        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.Interstitial, "admob", lastPlacementName);
        InvokeCurrentCallback();
        Unsubscribe();
    }
#endif
    private static void InterstitialClosed(object sender, EventArgs args)
    {
        InvokeCurrentCallback();
    }

    private static void InterstitialComplete(InterstitialAdNetwork net, AdPlacement place)
    {
        ScreenFadeController.Instance.FadeInTween();
        lastAdTime = DateTime.Now;
        InvokeCurrentCallback();
        Unsubscribe();
    }

    private static void InvokeCurrentCallback()
    {
        if (callbackCurrent)
            callback?.Invoke();
        else {
            Debug.LogWarning("Interstitial manager tried to call old callback");
        }
    }

    internal static void Load(InterstitialPlacement place)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }


        if (!AdManager.AreRemoved())
        {
            switch (place)
            {
                case InterstitialPlacement.ReturnToMenu:
                    if (Advertising.IsInterstitialAdReady(returnToMenu))
                    {
                        Advertising.LoadInterstitialAd(returnToMenu);
                    }
                    AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Loaded, GameAnalyticsSDK.GAAdType.Interstitial, "admob", "ReturnToMenuInterstitial");
                    break;
                case InterstitialPlacement.BetweenLevels:
                    if (Advertising.IsInterstitialAdReady(betweenLevels))
                    {
                        Advertising.LoadInterstitialAd(betweenLevels);
                    }
                    AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Loaded, GameAnalyticsSDK.GAAdType.Interstitial, "admob", "BetweenLevelsInterstitial");
                    break;
                default:
                    break;
            }
        }

    }

    public static void Show(InterstitialPlacement place, Action callback)
    {
#if EM_ADMOB

        if (Application.internetReachability == NetworkReachability.NotReachable){
            callback?.Invoke();
            return;
        }

        if (!AdManager.AreRemoved())
        {
            ScreenFadeController.Instance.FadeOutTween();


            switch (place)
            {
                case InterstitialPlacement.ReturnToMenu:
                    Subscribe(returnToMenu.Name, callback);

                    if (Advertising.AdMobClient.IsInterstitialAdReady(returnToMenu))
                    {
                        //Advertising.ShowInterstitialAd(returnToMenu);
                        Advertising.AdMobClient.ShowInterstitialAd(returnToMenu);
                        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.Interstitial, "admob", "ReturnToMenuInterstitial");
                    }
                    else
                    {
                        Instance.StartCoroutine(Instance.LoadAndShow(returnToMenu));
                    }
                    break;
                case InterstitialPlacement.BetweenLevels:
                    Subscribe(betweenLevels.Name, callback);
                    if (Advertising.AdMobClient.IsInterstitialAdReady(betweenLevels))
                    {
                        Advertising.AdMobClient.ShowInterstitialAd(betweenLevels);
                        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.Interstitial, "admob", "BetweenLevelsInterstitial");
                    }
                    else
                    {
                        Instance.StartCoroutine(Instance.LoadAndShow(betweenLevels));
                    }
                    break;
                default:
                    //Unknown placement
                    callback?.Invoke();
                    return;
            }

        }
        else {
            callback.Invoke();
        }
#else
            callback.Invoke();
#endif
    }

    private static void Subscribe(string lastPlaceName, Action callbackAction)
    {
        lastPlacementName = lastPlaceName;
        callback = callbackAction;
        callbackCurrent = true;
        Advertising.InterstitialAdCompleted += InterstitialComplete;
#if EM_ADMOB

        Advertising.AdMobClient.OnInterstitialAdFailedToLoad += InterstitialFailed;
        Advertising.AdMobClient.OnInterstitialAdClosed += InterstitialClosed;
        Advertising.AdMobClient.OnInterstitialAdLeavingApplication += InterstitialClicked;
#endif
    }
    private static void InterstitialClicked(object sender, EventArgs args) {
        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Clicked, GameAnalyticsSDK.GAAdType.Interstitial, "admob", lastPlacementName);
    }

    private static void Unsubscribe()
    {

        Advertising.InterstitialAdCompleted -= InterstitialComplete;
#if EM_ADMOB

        Advertising.AdMobClient.OnInterstitialAdFailedToLoad -= InterstitialFailed;
        Advertising.AdMobClient.OnInterstitialAdClosed -= InterstitialClosed;
#endif
        callback = null;
        callbackCurrent = false;
    }

    bool loading = false;

    System.Collections.IEnumerator LoadAndShow(AdPlacement placement)
    {
#if EM_ADMOB
        if (loading) yield return null;
        loading = true;
        int count = 0;

        Advertising.AdMobClient.LoadInterstitialAd(placement);
        while (!Advertising.AdMobClient.IsInterstitialAdReady(placement))
        {
            yield return new WaitForSeconds(.5f);
            count++;
            if (count > 20)
            {
                break;
            }
        }

        if (Advertising.AdMobClient.IsInterstitialAdReady(placement)){
            Advertising.AdMobClient.ShowInterstitialAd(placement);
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Loaded, GameAnalyticsSDK.GAAdType.Interstitial, "admob", placement.Name);

        }
        else {
            // Failed to load, do callback
            Debug.LogWarning("Interstitial failed to load: " +placement.Name);
            InterstitialFailed(InterstitialAdNetwork.None, new AdFailedToLoadEventArgs());
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.Interstitial, "admob", placement.Name, GameAnalyticsSDK.GAAdError.InternalError);
        }

        loading = false;

#else
        yield return null;
#endif


    }
}
