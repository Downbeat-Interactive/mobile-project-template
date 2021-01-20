using UnityEngine;
using EasyMobile; // include the Easy Mobile namespace to use its scripting API
using System.Collections.Generic;
using System;
using GameAnalyticsSDK;

[Serializable]
public class AppConsent
{
    public static AppConsent current= null;
    public const string StorageKey = "AppConsent";

    #region 3rd-party Services Consent

    // The consent for the whole Advertising module.
    // (we could have had different consents for individual ad networks, but for
    // the sake of simplicity in this demo, we'll ask the user a single consent
    // for the whole module and use it for all ad networks).
    public ConsentStatus advertisingConsent = ConsentStatus.Unknown;

    // The consent for the whole Notifications module.
    // Note that data consent is only applicable to push notifications,
    // local notifications don't require any consent.
    public ConsentStatus analyticsConsent = ConsentStatus.Unknown;

    public ConsentStatus notificationConsent = ConsentStatus.Unknown;


    // Since this demo app also has In-App Purchase, which forces the use of
    // Unity Analytics, we could have had to ask a consent for that too. However,
    // according to Unity it's sufficient to provide the user with an URL
    // so they can opt-out on Unity website. So we will include that URL in our
    // consent dialog and not need to ask and store any explicit consent locally.

    // Here you can add consent variables for other 3rd party services if needed,
    // including those not managed by Easy Mobile...

    #endregion

    /// <summary>
    /// To JSON string.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="EasyMobile.Demo.AppConsent"/>.</returns>
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// Converts this object to JSON and stores in PlayerPrefs with the provided key.
    /// </summary>
    /// <param name="key">Key.</param>
    public void Save(string key)
    {
        PlayerPrefs.SetString(key, ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Forwards the consent to relevant modules of EM.
    /// </summary>
    /// <param name="consent">Consent.</param>
    /// <remarks>
    /// In a real-world app, you'd want to write similar method
    /// to forward the obtained consent not only to relevant EM modules
    /// and services, but also to other relevant 3rd-party SDKs in your app.
    public static void ApplyConsent(AppConsent consent)
    {
        // Forward the consent to the Advertising module.
        if (consent.advertisingConsent == ConsentStatus.Granted)
            Advertising.GrantDataPrivacyConsent();
        else if (consent.advertisingConsent == ConsentStatus.Revoked) {
            Advertising.RevokeDataPrivacyConsent();
            AnalyticsManager.LogDesign("revokedAnalyticsConsent",DesignEventType.Choice);
        }
                
        
        if (consent.notificationConsent == ConsentStatus.Granted)
            Notifications.GrantDataPrivacyConsent();
        else if (consent.notificationConsent == ConsentStatus.Revoked) {
            Notifications.RevokeDataPrivacyConsent();
            AnalyticsManager.LogDesign("revokedNotificationConsent",DesignEventType.Choice);
        }

        // Forward the consent to the Analytics module.
        if (consent.analyticsConsent == ConsentStatus.Granted) {
            GameAnalytics.Initialize();
        }
        else if (consent.analyticsConsent == ConsentStatus.Revoked){
            AnalyticsManager.LogDesign("revokedAnalyticsConsent", DesignEventType.Choice);
        }

        current = consent;
        Save(current);
    }

    public static ConsentStatus AnalyticsConsentStatus() {
        return current.analyticsConsent;
    }  
    public static ConsentStatus AdConsentStatus() {
        return current.advertisingConsent;
    }    
    
    public static ConsentStatus NotificationConsentStatus() {
        return current.notificationConsent;
    }

    /// <summary>
    /// Saves the give app consent to PlayerPrefs as JSON using the demo storage key.
    /// </summary>
    /// <param name="consent">Consent.</param>
    public static void Save(AppConsent consent)
    {
        if (consent != null)
            consent.Save(StorageKey);
    }

    /// <summary>
    /// Loads the demo app consent from PlayerPrefs, returns null if nothing stored previously.
    /// </summary>
    /// <returns>The demo app consent.</returns>
    public static AppConsent LoadAppConsent()
    {
        string json = PlayerPrefs.GetString(StorageKey, null);

        if (!string.IsNullOrEmpty(json))
            return JsonUtility.FromJson<AppConsent>(json);
        else
            return null;
    }
}


public class EasyMobileManager : MonoBehaviour
{
    private static bool mIsInEEARegion = false;

    private static AppConsent defaultAppConsentNonEEA = null;

    public static string UnityAnalyticsOptOutURL { get; private set; }

    public delegate void FinishedInitAction();
    public static event FinishedInitAction OnFinishedInit;

    // Checks if EM has been initialized and initialize it if not.
    // This must be done once before other EM APIs can be used.
    void Awake()
    {
        // Checks if we're in EEA region.
        Privacy.IsInEEARegion(result =>
        {
            mIsInEEARegion = result == EEARegionStatus.InEEA;
        });

        // Fetch Unity Analytics URL for use in case the consent dialog
        // is shown from the demo buttons.
        if (string.IsNullOrEmpty(UnityAnalyticsOptOutURL))
            FetchUnityAnalyticsOptOutURL(null, null);

        shouldRequestConsent = shouldRequestConsent || mIsInEEARegion;

        AppConsent loadedConsent = AppConsent.LoadAppConsent();
        shouldRequestConsent = shouldRequestConsent && loadedConsent == null;
        // If we think consent is not needed for our app (or the current device
        // is not in EEA region), we can just
        // go ahead and initialize EM runtime as normal.
        if (!shouldRequestConsent)
        {
            defaultAppConsentNonEEA = AppConsent.LoadAppConsent();
            if (defaultAppConsentNonEEA == null) {
                // Was empty, load default values
                defaultAppConsentNonEEA = new AppConsent();
                defaultAppConsentNonEEA.advertisingConsent = ConsentStatus.Granted;
                defaultAppConsentNonEEA.analyticsConsent = ConsentStatus.Granted;
                defaultAppConsentNonEEA.notificationConsent = ConsentStatus.Granted;
            }

            AppConsent.ApplyConsent(defaultAppConsentNonEEA);
            Init();


            return;
        }

        AskForConsent();
        // If there's a stored consent:
        // the implementation of this demo app guarantees
        // that this consent was forwarded to relevant modules before it was stored.
        // These modules would have automatically stored their own consent persistently
        // and use that consent during initialization.
        // In short we'll just go ahead with initializing the EM runtime.
        if (AppConsent.current != null)
        {
            Init();
            return;
        }
       
    }

    private static void Init() {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
        if (!Notifications.IsInitialized())
            Notifications.Init();

        OnFinishedInit?.Invoke();

    }

    public static void FetchUnityAnalyticsOptOutURL(Action<string> success, Action<string> failure)
    {
        // If the URL was loaded before, just invoke the success callback immediately.
        if (!string.IsNullOrEmpty(UnityAnalyticsOptOutURL))
        {
            if (success != null)
                success(UnityAnalyticsOptOutURL);
        }

        // Since Unity 2018.3.0, the Unity Data Privacy plugin is embedded in the Analytics library,
        // so just call the method directly.
        UnityEngine.Analytics.DataPrivacy.FetchPrivacyUrl(url =>
        {
            OnFetchUnityAnalyticsURLSuccess(url, success);
        },
            error =>
            {
                OnFetchUnityAnalyticsURLFailure(error, failure);
            });
    }


    private static void OnFetchUnityAnalyticsURLSuccess(string url, Action<string> callback)
    {
        UnityAnalyticsOptOutURL = url;
        if (callback != null)
            callback(url);

        Debug.Log("Unity Analytics opt-out URL is fetched successfully.");
    }

    private static void OnFetchUnityAnalyticsURLFailure(string error, Action<string> callback)
    {
        UnityAnalyticsOptOutURL = string.Empty;
        if (callback != null)
            callback(error);

        Debug.LogWarning("Fetching Unity Analytics opt-out URL failed with error: " + error);
    }

    private static void PrivacyComplete(ConsentDialog dialog, ConsentDialog.CompletedResults results)
    {
        AppConsent newConsent = new AppConsent();
        if (results.toggleValues != null)
        {
            Debug.Log("Consent toggles:");
            foreach (KeyValuePair<string, bool> t in results.toggleValues)
            {
                string toggleId = t.Key;
                bool toggleValue = t.Value;
                Debug.Log("Toggle ID: " + toggleId + "; Value: " + toggleValue);

                if (toggleId == "advertising-toggle")
                {
                    // Whether the Advertising module is given consent.
                    newConsent.advertisingConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                }
                else if (toggleId == "tp-analytics-toggle")
                {
                    // Whether the Notifications module is given consent.
                    newConsent.analyticsConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                }
                else if (toggleId == "unity-analytics-toggle")
                {
                    // We don't store the UnityAnalytics consent ourselves as it is managed
                    // by the Unity Data Privacy plugin.
                }                
                else if (toggleId == "notification-toggle")
                {
                    newConsent.notificationConsent = toggleValue ? ConsentStatus.Granted : ConsentStatus.Revoked;
                }
                else
                {
                    // Unrecognized toggle ID.
                }
            }
        }

        if (!RuntimeManager.IsInitialized())
        {
            Init();
        }
        else
        {
            // The initialization has already been done. Inform the user
            // that the changes will take effect during next initialization (next app launch).
            PopupBuilder popup = new PopupBuilder();
            popup.blockBackgroundRaycasts = true;
            popup.showDeclineButton = false;
            popup.title = "Consent Updated";
            popup.text = "You've updated your data privacy consent.\n\n" +
                "Since the initialization process has already completed, all changes will take effect in the next app launch.";
            popup.Show();
        }

        _askingConsent = false;
        AppConsent.ApplyConsent(newConsent);
        dialog.Completed -= PrivacyComplete;


    }

    private static bool _askingConsent = false;
    public static void AskForConsent() {
        if (_askingConsent) return;
        _askingConsent = true;
        AnalyticsManager.LogUI("optionsPrivacy", DesignEventType.Clicked);
        //EasyMobileInitializer.AskForConsent();

        ConsentDialog dialog = Privacy.ShowDefaultConsentDialog();
        UnityEngine.Analytics.DataPrivacy.FetchPrivacyUrl(url =>
        {
            string newDesc = dialog.FindToggleWithId("unity-analytics-toggle").OnDescription.Replace("UNITY_ANALYTICS_URL", url);
            ConsentDialog.Toggle oldT = dialog.FindToggleWithId("unity-analytics-toggle");
            oldT.OnDescription = newDesc;
        },
          error =>
          {
              string str = dialog.FindToggleWithId("unity-analytics-toggle").Description.Replace("UNITY_ANALYTICS_URL", "\"mailto:developer@jacksonsdean.com\"");
              ConsentDialog.Toggle oldT = dialog.FindToggleWithId("unity-analytics-toggle");
              string newDesc = oldT.OnDescription.Insert(str.Length - 1, " Unfortunately, we couldn't find the opt-out link now. please email <a href=\"mailto:developer@jacksonsdean.com\">developer@jacksonsdean.com</a> for more info.");
              oldT.OnDescription = newDesc;

          });

        dialog.Completed += PrivacyComplete;
        dialog.Dismissed += (ConsentDialog d) => { _askingConsent = false; };
        
    }
   


#region Public Settings

    [Header("GDPR Settings")]
    [Tooltip("Whether we should request user consent for this app")]
    public bool shouldRequestConsent = true;

    [Header("Object References")]
    public GameObject isInEeaRegionDisplayer;
    //public DemoUtils demoUtils;

#endregion


}