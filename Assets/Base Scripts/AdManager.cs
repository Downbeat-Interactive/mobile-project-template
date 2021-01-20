#define NO_ADS

using EasyMobile;
#if EM_ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAdsMediationTestSuite.Api;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;


public class AdManager : MonoBehaviour
{
    public const string MainMenuBannerName      = "MainMenuBanner";
    public const string ContinueBannerName      = "ContinueBanner";
    public const string ContinueRewardedName    = "ContinueRewarded";
    public const string SkipRewarded            = "SkipRewarded";


    static bool _removed = false;
    static readonly string REMOVED_KEY = "remove_ads";
    private const string TEMP_TIME_PREF_KEY = "time_remove_ads_ends";

    static AdPlacement mainMenuBanner       = null;
    static AdPlacement continueBanner       = null;
    public static AdPlacement continueRewarded     = null;
    public static AdPlacement skipRewarded     = null;



    private void Awake(){
#if NO_ADS
        _removed = true;
#endif

        mainMenuBanner      =   AdPlacement.PlacementWithName(MainMenuBannerName);
        continueBanner      =   AdPlacement.PlacementWithName(ContinueBannerName);
        continueRewarded    =   AdPlacement.PlacementWithName(ContinueRewardedName);
        skipRewarded =   AdPlacement.PlacementWithName(SkipRewarded);
    }

    internal static void RemoveForTime(float lengthInMins)
    {
        RemoveAds();
        DateTime endTime = System.DateTime.Now.AddMinutes(lengthInMins);

        PlayerPrefs.SetString(TEMP_TIME_PREF_KEY, endTime.ToBinary().ToString());

    }
   
    public static bool AreRemoved() {
#if NO_ADS
        _removed = true;
#else
        //if (GameFoundationManager.isInit)
        //    _removed = TransactionManager.IsIapProductOwned(REMOVED_KEY);
        //else {
        //    GameFoundationManager.Initialize();
        //    _removed = true;        
        //}

        _removed = false;

        _removed = _removed || IsTempRemoved();
#endif
        return _removed;
    }


    private static bool IsTempRemoved() {
        //Store the current time when it starts
        DateTime currentDate = System.DateTime.Now;

        //Grab the old time from the player prefs as a long
        long tempStoredDate = Convert.ToInt64(PlayerPrefs.GetString(TEMP_TIME_PREF_KEY,"0"));

        //Convert the old time from binary to a DataTime variable
        DateTime storedDate = DateTime.FromBinary(tempStoredDate);

        //Use the Subtract method and store the result as a timespan variable
        TimeSpan difference = currentDate.Subtract(storedDate);
        return difference.Seconds < -1;
    }



    public static void Init(Action callback) {
        // Initialize the Mobile Ads SDK.
#if EM_ADMOB
        Debug.Log("AdManager initializing AdMob...");
        GoogleMobileAds.Api.MobileAds.Initialize((initStatus) =>
        {
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            Debug.LogFormat("AdMob initialized by AdManager, {0} adapters found", map.Count);
            if (Debug.isDebugBuild) {
                PopupBuilder b = new PopupBuilder();
                b.title = "debug";
                b.text= "initialized admob in admanager, "+map.Count.ToString() + " adapters found";
                b.Show();
            }
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        MonoBehaviour.print("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        MonoBehaviour.print("Adapter: " + className + " is initialized.");
                        break;
                }
            }
            callback?.Invoke();
        });

        if (EasyMobile.RuntimeManager.IsInitialized()){
            EasyMobile.Advertising.AdMobClient.Init();
        }
#endif
    }

    public static void ShowMediationTest() {
#if EM_ADMOB
        MediationTestSuite.AdRequest = new AdRequest.Builder()
          .AddTestDevice("C83F2B11BF765AB8BDAE9D886BCA6232") // Nexus
          .AddTestDevice("F59B396FDC0D739F95275CF676CF26B6") // S9
          .AddTestDevice("A0D0922D857EB2A859FBAAEF61B35D69") // S9 GP
          .Build();

        GoogleMobileAdsMediationTestSuite.Api.MediationTestSuite.Show();
#endif
    }

    //public void OnAdRemoved(BaseTransaction t){
    //    RemoveAds();
    //}

    private static void RemoveAds() {
        _removed = true;
        BannerAd.HideBannerAd(mainMenuBanner);
        BannerAd.HideBannerAd(continueBanner);
        //GameObject adButton = GameObject.FindGameObjectWithTag("RemoveAdsButton");
        //if (adButton)
        //{
        //    adButton.GetComponent<RectTransform>().DOAnchorPosX(-1000.0f, .65f).SetRelative(true).OnComplete(() => { adButton.SetActive(false); });
        //}
    }


}
