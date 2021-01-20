using System.Collections;
using UnityEngine;

using EasyMobile;
using System.Collections.Generic;

public class BannerAd : MonoBehaviour
{
    public static BannerAd Instance;

    [SerializeField]
    BannerAdPosition bannerPosition = BannerAdPosition.Bottom;

    public static bool isShowing = false;

    static List<AdPlacement> showingAds = new List<AdPlacement>();

    void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        else Instance = this;

    }

    public void ShowBannerAdInternal(AdPlacement place) {
        StartCoroutine(ShowBannerWhenInitialized(place));
    }
    public static void ShowBannerAd(AdPlacement place, BannerAdPosition position) {
        if (!AdManager.AreRemoved())
        {
            Instance.bannerPosition = position;
            Instance.ShowBannerAdInternal(place);
        }
    } 
    
    

    IEnumerator ShowBannerWhenInitialized(AdPlacement place)
    {
        if (!Advertising.AdMobClient.IsInitialized) {
            Advertising.AdMobClient.Init();
            yield return new WaitForSeconds(1.0f);
        }
        if (showingAds.Contains(place))
            yield return null;
        else {
            showingAds.Add(place);
            isShowing = true;
            Advertising.AdMobClient.ShowBannerAd(place, bannerPosition, BannerAdSize.Banner);
            AnalyticsManager.LogAd(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.Banner, "admob", place.Name);
        }
    }


    public static void HideBannerAd(AdPlacement place) {
        //Advertising.AdMobClient.HideBannerAd(place);
        Advertising.AdMobClient.DestroyBannerAd(place);
        isShowing = false;
        showingAds.Remove(place);
    }

    public static void HideAllShowing() {
        AdPlacement[] places = showingAds.ToArray();
        for (int i = 0; i < places.Length; i++){
            HideBannerAd(places[i]);
        }
    }

    private void Update()
    {
        //if (isShowing && GameManager.Instance.isPlaying) {
        //    HideAllShowing();
        //}
    }
}
