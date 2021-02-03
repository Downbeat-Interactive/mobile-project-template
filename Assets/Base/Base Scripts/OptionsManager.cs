using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OptionsManager : MonoBehaviour
{
    public static readonly string PREF_QUALITY = "Quality Level";

    public static readonly string PREF_SFX = "SFX Volume";
    public static readonly string PREF_MUSIC = "Music Volume";
    public static readonly string PREF_ZEN = "Zen Mode Enabled";

    public static float music = 0.5f;
    public static float sfx   = 0.5f;
    public static bool zenMode = false;
    public static bool hasOverrideGFX = false;

    private static OptionsManager Exists;
    internal static int prevGFXQuality = -1;
    
    private void Awake()
    {
        if (Exists)
            Destroy(this.gameObject);
        else Exists = this;

        DontDestroyOnLoad(this.gameObject);
        LoadFromSave();
        
    }

    public static void LoadFromSave()
    {
        int qualityIndex = PlayerPrefs.GetInt(OptionsManager.PREF_QUALITY, -1);
        OptionsManager.hasOverrideGFX = qualityIndex != -1;
        if (!OptionsManager.hasOverrideGFX)
        {
            qualityIndex = GetDefaultQuality();
            Debug.LogFormat("User has not overrode quality, using default: {0}", qualityIndex);
            AnalyticsManager.LogDesign("setDefaultQuality", (float)qualityIndex);
        }
        else
        {
            AnalyticsManager.LogDesign("setSavedOverrideQuality", (float)qualityIndex);
        }

        ConfirmQuality(qualityIndex);

        music = PlayerPrefs.GetFloat(PREF_MUSIC, AudioManager.GetMaxMusicVolume());
        sfx = PlayerPrefs.GetFloat(PREF_SFX, AudioManager.GetSFXVolume());
        AudioManager.SetMaxMusicVolume(music);
        AudioManager.SetSFXVolume(sfx);
    }

    internal static int GetGFXQuality(){
        return prevGFXQuality;
    }


    internal static void OverrideToLowGFX()
    {
        if (hasOverrideGFX) return;
        hasOverrideGFX = true;

        int qualityIndex = (int)GraphicsQualityLevel.Low;

        var menu = FindObjectOfType<OptionsMenu>();
        if (menu) {
            menu.SetOverrideGFX(qualityIndex);
        }
        else
        {
            ConfirmQuality(qualityIndex);
        }

    }
        public static void ConfirmQuality(int index, bool fromMenu = false)
        {
            PlayerPrefs.SetInt(PREF_QUALITY, index);
            QualitySettings.SetQualityLevel(index > 0 ? index : 0);

            if (index < 2){
                EnforceLowGFX(fromMenu);
            }
            else
            {
                LowToHighGFX(fromMenu);

            }

            if (fromMenu){
                hasOverrideGFX = true;
            }

            prevGFXQuality = index;

        }

        private static void EnforceLowGFX(bool fromMenu)
        {
        //if (fromMenu && prevGFXQuality == 0) return;
        Camera.main.GetComponent<PostProcessLayer>().enabled = false;
    }
        private static void LowToHighGFX(bool fromMenu)
        {
        //if (fromMenu && prevGFXQuality > 1) return;
        Camera.main.GetComponent<PostProcessLayer>().enabled = true;


    }


    private static int GetDefaultQuality()
    {
        return 1;
        //TODO
        GraphicsQualityLevel result = GraphicsQualityLevel.Medium;

        if (SystemInfo.systemMemorySize < 3500)
        {
            result = GraphicsQualityLevel.Low; // LOW
        }

        return (int)result;
    }


}
