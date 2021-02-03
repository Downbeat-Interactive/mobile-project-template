using DG.Tweening;
using EasyMobile;
using System;
using UnityEngine;
//using UnityEngine.GameFoundation;
using UnityEngine.UI;

public class RewardedAd : MonoBehaviour
{
    public static RewardedAd Instance;

    public static bool showingAd = false;
    [SerializeField]
    string title = "";
    [SerializeField]
    string text = "";
    [SerializeField]
    Sprite graphic = null;

    [SerializeField]
    int coinRewardAmount = 0;
    [SerializeField]
    int gemRewardAmount = 0;

    [SerializeField]
    Color graphicTint = Color.white;

    static PopupCallback acceptRewardCallback;
    static PopupCallback declineRewardCallback;


    static AdPlacement lastPlace = null;

    static Action completeCallback = null;
    static Action failedToCompleteCallback = null;
    static bool callbacksCurrent = false;

    public static int GetRewardedCoinsAmount() {
        return Instance.coinRewardAmount;
    } 
    public static int GetRewardedGemsAmount() {
        return Instance.gemRewardAmount;
    }

    private void Awake()
    {
        if (Instance)
            Destroy(this.gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }


    public static void LoadContinue(){
        Advertising.AdMobClient.LoadRewardedAd(AdManager.continueRewarded);
    }

    void OnEnable(){
        if (!AdManager.AreRemoved())
        {
            //GameManager.OnStartGame += LoadContinue;
        }
    }

    // Unsubscribe events
    void OnDisable(){
        //GameManager.OnStartGame -= LoadContinue;
    }

#if EM_ADMOB
    static void RewardedAdFailedToShowHandler(object sender, GoogleMobileAds.Api.AdErrorEventArgs argse) {
        if(callbacksCurrent)
            failedToCompleteCallback?.Invoke();
        
        Unsubscribe();
        Debug.LogWarning(argse.Message.ToString());
        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", lastPlace.Name, GameAnalyticsSDK.GAAdError.InternalError);
    }

    static void RewardedAdFailedToLoadHandler(object sender, GoogleMobileAds.Api.AdErrorEventArgs args) {
        Debug.LogWarning("Failed to load rewarded ad: " + args.Message);
        if (lastPlace == null) {
            Debug.LogWarning("Last placement was null");
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", "null", GameAnalyticsSDK.GAAdError.InternalError);
        }
        else{
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", lastPlace.Name, GameAnalyticsSDK.GAAdError.InternalError);
        }
        
        if(callbacksCurrent)
            failedToCompleteCallback?.Invoke();
        
        Unsubscribe();
    }
#endif

    // Event handler called when a rewarded ad has completed
    static void RewardedAdCompletedHandler(IAdClient client, AdPlacement location)
    {
        Debug.Log("Rewarded ad has completed. The user should be rewarded now.");
        ScreenFadeController.Instance.FadeInTween();

        AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.RewardReceived, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", location.Name);

        if (callbacksCurrent)
            completeCallback?.Invoke();

        Unsubscribe();
    }

    // Event handler called when a rewarded ad has been skipped
    static void RewardedAdSkippedHandler(IAdClient client, AdPlacement location)
    {
        Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");
        showingAd = false;
        PopupBuilder b = new PopupBuilder();

        b.title = "Skipped";
        b.text = "You must complete the entire ad or survey to receive the reward.";
        b.Show();

        ScreenFadeController.Instance.FadeInTween();
      

        if(callbacksCurrent)
            failedToCompleteCallback?.Invoke();

        Unsubscribe();
    }

    private static void Unsubscribe()
    {
#if EM_ADMOB

        callbacksCurrent = false;
        completeCallback            = null;
        failedToCompleteCallback    = null;

        Advertising.AdMobClient.RewardedAdCompleted -= RewardedAdCompletedHandler;
        Advertising.AdMobClient.RewardedAdSkipped -= RewardedAdSkippedHandler;
        Advertising.AdMobClient.OnRewardedAdFailedToLoad -= RewardedAdFailedToLoadHandler;
        Advertising.AdMobClient.OnRewardedAdFailedToShow -= RewardedAdFailedToShowHandler;
#endif
    }    
    
    private static void Subscribe(Action completeAdCallback, Action failedToCompleteAdCallback)
    {
#if EM_ADMOB

        completeCallback = completeAdCallback;
        failedToCompleteCallback    = failedToCompleteAdCallback;
        callbacksCurrent            = true;

        Advertising.AdMobClient.RewardedAdCompleted += RewardedAdCompletedHandler;
        Advertising.AdMobClient.RewardedAdSkipped += RewardedAdSkippedHandler;
        Advertising.AdMobClient.OnRewardedAdFailedToLoad += RewardedAdFailedToLoadHandler;
        Advertising.AdMobClient.OnRewardedAdFailedToShow += RewardedAdFailedToShowHandler;
#endif
    }

    public static void ShowRewardedAd(string placementName, Action completedAdCallback, Action failedToCompleteAdCallback) {
        Instance.ShowAd(AdPlacement.PlacementWithName(placementName), completedAdCallback, failedToCompleteAdCallback);
    }
    private void ShowAd(AdPlacement place, Action completedAdCallback, Action failedToCompleteAdCallback) {
#if EM_ADMOB

        // Internet Check
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            PopupBuilder p = new PopupBuilder();
            p.title = "No connection";
            p.text = "You must be connected to the internet to do that";
            p.Show();

            failedToCompleteCallback?.Invoke();
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", place.Name, GameAnalyticsSDK.GAAdError.Offline);

            return;
        }

        ScreenFadeController.Instance.FadeOutTween();

        Subscribe(completedAdCallback, failedToCompleteAdCallback);
  
        lastPlace = place;
        // Show it if it's ready
        if (Advertising.AdMobClient.IsRewardedAdReady(place)){
            Advertising.AdMobClient.ShowRewardedAd(place);
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.RewardedVideo, "admob", place.Name);
        }
        else{
            Instance.StartCoroutine(Instance.LoadAndShow(place));
        }
#endif
    }

    public void OnDeclineContinue(GameObject popup) {
        declineRewardCallback?.Invoke(popup);
        showingAd = false;
    }


    public void ShowContinuePopup(PopupCallback acceptReward, PopupCallback declineRewardContinue, float delay = 0.0f) {
        if (showingAd) return;
        acceptRewardCallback = acceptReward;
        declineRewardCallback = declineRewardContinue;
        
        PopupBuilder b = new PopupBuilder();
        b.title = title;
        b.text = text;
        b.showConfirmButton = true;
        b.showDeclineButton = true;

        b.confirmCallback = OnConfirmContinue;
        b.declineCallback = OnDeclineContinue;
        b.delay = delay;
        b.graphic = graphic;
        b.graphicTint = graphicTint;
        b.blockBackgroundRaycasts = false;
        if (!AdManager.AreRemoved())
        {
            b.Show();
        }
        else {
            // give them the reward for free
            RewardedAdCompletedHandler(null, AdManager.continueRewarded);
        }

    }

    System.Collections.IEnumerator LoadAndShow(AdPlacement placement) {
#if EM_ADMOB

        int count = 0;

        Advertising.AdMobClient.LoadRewardedAd(placement);
        while (!Advertising.AdMobClient.IsRewardedAdReady(placement)){
            yield return new WaitForSeconds(.5f);
            count++;
            if (count > 30) {
                ScreenFadeController.Instance.FadeInTween();
                yield return null;
            }
        }


        lastPlace = placement;
        if (Advertising.AdMobClient.IsRewardedAdReady(placement))
            Advertising.AdMobClient.ShowRewardedAd(placement);
        else { 
            Debug.LogWarning("Ad failed to load " + placement.Name);
            ScreenFadeController.Instance.FadeInTween();
            if (callbacksCurrent)
                failedToCompleteCallback?.Invoke();
        }
#else
        yield return null;
#endif
    }
    public void OnConfirmContinue(GameObject popup)
    {
        showingAd = false;
        AnalyticsManager.LogUI("acceptContinueReward", DesignEventType.Clicked);
        ShowAd(AdManager.continueRewarded, 
            ()=> {
                acceptRewardCallback?.Invoke();
                showingAd = false;
            }, 
        ()=> { OnDeclineContinue(popup); });
    }
   

   

}
