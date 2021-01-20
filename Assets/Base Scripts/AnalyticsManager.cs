#define AUTO_FACEBOOK

using EasyMobile;
using Facebook.Unity;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using UnityEngine;
using UnityEngine.Analytics;



public enum DesignEventType {
    Clicked, Spun, Closed, Opened, Choice
}

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);

        }
        else {
            Instance = this;
        }

        // Initialize GameAnalytics
        //if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        //{
            GameAnalyticsSDK.GameAnalytics.Initialize();
            InitFB();

        //}
        //else {
        //    FindObjectOfType<GameAnalytics>().enabled = false;
        //    FindObjectOfType<GA_SpecialEvents>().enabled = false;
        //}

    }
    static bool _doingFBInit = false;
    void InitFB() {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            if (_doingFBInit) return;
            _doingFBInit = true;
            //Handle FB.Init
            FB.Init(() =>
            {
                FB.ActivateApp();
                _doingFBInit = false;
            });
        }
    }

    #region analytics-logs
    public static void LogAd(GAAdAction action,GAAdType type, string sdk, string placement, GAAdError failureReason = GAAdError.Undefined) {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted){
            GameAnalytics.NewAdEvent(GAAdAction.Show, type, sdk, placement, failureReason);
        }
    }

    public static void LogProgression(string name, string level, int score, string additionalData = "") {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            // Game Analytics (Downbeat)
            GAProgressionStatus status = GAProgressionStatus.Undefined;
            switch (name)
            {
                case "Start":
                    status = GAProgressionStatus.Start;
                    break;
                case "Fail":
                    status = GAProgressionStatus.Fail;
                    break;
                case "Complete":
                    status = GAProgressionStatus.Complete;
                    break;
                default:
                    break;
            }

            GameAnalytics.NewProgressionEvent(status, level, additionalData, score);

        }
    }

    public static void LogProgression(GAProgressionStatus status, string level, int score, string additionalData = "")
    {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            // Game Analytics (Downbeat)
            GameAnalytics.NewProgressionEvent(status, level, additionalData, score);
        }
    }

    public static void LogResourceFlow(GAResourceFlowType type, string currency, float amount, string itemType="", string itemId="")
    {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            // Game Analytics (Downbeat)
            GameAnalytics.NewResourceEvent(type, currency, amount, itemType, itemId);
        }
    }


    public static void LogUI(string name, DesignEventType type){
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            GameAnalytics.NewDesignEvent(name, (float)type);
        }
    }
    public static void LogDesign(string name, DesignEventType type) {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            GameAnalytics.NewDesignEvent(name, (float)type);
        }
    }
    
    public static void LogDesign(string name,float value) {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {
            GameAnalytics.NewDesignEvent(name, value);
        }
    }

//    public static void LogBusinessEvent(UnityEngine.GameFoundation.IAPTransaction transaction) {
//        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
//        {
//            try
//            {
//                var info = TransactionManager.GetLocalizedIAPProductInfo(transaction.productId);
//                int price = (int)((float)transaction.product.metadata.localizedPrice * 100.0f);


//#if UNITY_ANDROID
//                GameAnalytics.NewBusinessEventGooglePlay(transaction.product.metadata.isoCurrencyCode, price, transaction.displayName, transaction.productId, "", transaction.product.receipt, "");
//#elif UNITY_IOS
//        GameAnalytics.NewBusinessEvent(currency, amount, itemType, itemId, cartType);
//#endif

//#if AUTO_FACEBOOK
//                // Facebook (Done automatically)
//#else
//            var iapParameters = new Dictionary<string, object>();
//            iapParameters["dandydrift_packagename"] = transaction.productId;
//            FB.LogPurchase(
//              (float)transaction.product.metadata.localizedPrice,
//              transaction.product.metadata.isoCurrencyCode,
//              iapParameters
//            );
//#endif
//                var unityEvent = AnalyticsEvent.IAPTransaction("IAP Store", (float)transaction.product.metadata.localizedPrice, transaction.productId);


//            }
//            catch (System.Exception e)
//            {
//                LogErrorEvent(e);
//            }
//        }

//    }

    public static void LogErrorEvent(System.Exception e) {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        {

            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, e.StackTrace);
        }
    }
    #endregion


    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void OnApplicationQuit()
    {
        if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted) { }
    }
    private void OnApplicationPause(bool pause)
    {
        //if (AppConsent.current != null && AppConsent.current.analyticsConsent == ConsentStatus.Granted)
        //{
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pause)
            {
                //app resume
                GameAnalytics.NewDesignEvent("appResume", (float)DesignEventType.Opened);

                InitFB();
            }
            else
            {
                //Pause
                GameAnalytics.NewDesignEvent("appPause", (float)DesignEventType.Closed);

            }
        //}
    }
}
