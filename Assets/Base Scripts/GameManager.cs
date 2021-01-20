using DG.Tweening;
using GameAnalyticsSDK;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;

    private const string LEVEL_PREF = "level";
    private const string SHOWN_TUT_PREF = "shown-tutorial";
    private const string INTERSTITIAL_FREQ_KEY = "inter-freq";

    public delegate void GameStartAction();
    public static event GameStartAction OnStartGame;

    public delegate void GameLoseAction();
    public static event GameLoseAction OnLose;
    public delegate void GameWinAction();
    public static event GameWinAction OnWin;

    [SerializeField]
    bool useOverrideLevel = false;
    [SerializeField]
    int overrideStartingLevel = 0;
    [SerializeField]
    int attemptsAdFreqDefault = 4;
    int attemptsAdFreq = 4;
    [SerializeField]
    bool removeAds = false;

    private static int currentLevel = 1;
    public static int attempts = 0;
    internal static bool isPlaying = false;
    internal static bool showTut = false;

    internal static int currentLevelTier = 0;

    internal static int GetCurrentLevel() {
        return currentLevel;
    }


    void Awake()
    {

        Application.targetFrameRate = 60;

#if UNITY_EDITOR
        DOTween.logBehaviour = LogBehaviour.Verbose;

        if (removeAds)
            AdManager.RemoveForTime(100);
#endif
        if (Instance) Destroy(this.gameObject);
        else Instance = this;

        DontDestroyOnLoad(this.gameObject);

        if (useOverrideLevel)
            currentLevel = overrideStartingLevel;
        else 
            currentLevel = PlayerPrefs.GetInt(LEVEL_PREF, 1);

        showTut = PlayerPrefs.GetInt(SHOWN_TUT_PREF, 0) == 0;


        attemptsAdFreq= attemptsAdFreqDefault;
        try
        {
            attemptsAdFreq = int.Parse(GameAnalytics.GetRemoteConfigsValueAsString(INTERSTITIAL_FREQ_KEY, attemptsAdFreqDefault.ToString()));
        }
        catch (Exception e) {
        }

    }


    internal static void StartGame(){
        isPlaying = true;
        LevelManager.NewLevel(currentLevel);
        OnStartGame?.Invoke();
        attempts++;
    }

    internal static void ShownTutorial(){
        showTut = false;
        PlayerPrefs.SetInt(SHOWN_TUT_PREF, 1);
        PlayerPrefs.Save();
    }

    internal static void SetCurrentLevel(int v){
        NextLevel(v);
    }

    public static void NextLevel( int overrideLvl = -1, bool overrideNoAd = false) {
        if (overrideLvl >= 0)
            currentLevel = overrideLvl;

        bool showAd = !overrideNoAd && ( attempts>0 && attempts % Instance.attemptsAdFreq == 0);

        if (showAd) {
            InterstitialManager.Show(InterstitialPlacement.BetweenLevels,()=> { LoadAndAdvance(showAd); });
        }
        else {
            LoadAndAdvance(showAd);
        }

    }

    private static void LoadAndAdvance(bool didAd = false)
    {
        bool needsLoading = true;
        if (!needsLoading)
        {
            if(didAd)
                ScreenFadeController.Instance.FadeInTween();
            isPlaying = true;
            LevelManager.NewLevel(currentLevel);
        }

    }

    public static void ContinueFromSkip() {
        Loader.UnloadScene(DefaultScene.Lose);
        isPlaying = false;
        currentLevel++;
        PlayerPrefs.SetInt(LEVEL_PREF, currentLevel);
        PlayerPrefs.Save();

        LoadAndAdvance(true);
    }

    public static void ContinueFromWinScreen() {
        Loader.UnloadScene(DefaultScene.Win);
        NextLevel();
        attempts++;
    }

    public static void ContinueFromLoseScreen(){

        bool showAd = attempts > 0 && attempts % Instance.attemptsAdFreqDefault == 0;
        if (showAd){
            InterstitialManager.Show(InterstitialPlacement.BetweenLevels, ()=>{DoContinue(showAd); });
        }
        else{
            DoContinue(showAd);
        }
    }

    static void DoContinue(bool didAd = false)
    {
        if (didAd)
            ScreenFadeController.Instance.FadeInTween();

        StartGame();
    }

    public static void WonLevel(){
        isPlaying = false;
        currentLevel++;
        GameUI.ShowInGameUI();
        PlayerPrefs.SetInt(LEVEL_PREF, currentLevel);
        PlayerPrefs.Save();
        Loader.ImmediateAdditive(DefaultScene.Win);
        OnWin?.Invoke();

    }

    public static void LostLevel(){
        isPlaying = false;
        GameUI.ShowInGameUI();
        Loader.ImmediateAdditive(DefaultScene.Lose);
        OnLose?.Invoke();
    }


    private void OnApplicationQuit(){
        AnalyticsManager.LogDesign("num-attempts-this-session", attempts);
    }
}
