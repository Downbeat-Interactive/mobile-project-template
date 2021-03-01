using DG.Tweening;
using EasyMobile;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OptionsState {
    Closed, Opened,
    Ready
}

public enum GraphicsQualityLevel {
    Low, Medium, High
}
public class OptionsMenu : MonoBehaviour
{


    [SerializeField]
    ToggleGroup gfxToggleGroup = null;

    AudioManager audioManager = null;

    [SerializeField]
    Slider musicSlider = null;
    [SerializeField]
    Slider sfxSlider = null;
    [SerializeField]
    AudioClip sfxTestClip = null;

    [SerializeField]
    GameObject rippleCamera;

    AudioSource audioSource;

    [SerializeField]
    Toggle zenToggle = null;

    int qualityIndex = 1;
    internal void SetOverrideGFX(int qualityIndex)
    {
        if (gameObject.activeInHierarchy)
        {
            OptionsManager.prevGFXQuality = qualityIndex;
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex].isOn = true;
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex].group.RegisterToggle(gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex]);
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex].group.NotifyToggleOn(gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex]);
        }
       
    }

    public static OptionsState state = OptionsState.Closed;

    bool _playTestSFXFlag = false;
    float lastTestTime = 0;

    float _openedTime = float.MaxValue;
    // Start is called before the first frame update
    void Awake(){
        audioManager = AudioManager.Instance;
        audioSource = GetComponent<AudioSource>();
        InitUI();
    }

    internal void OnOpen()
    {
        state = OptionsState.Opened;
        lastTestTime = Time.time;
        _openedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playTestSFXFlag && state == OptionsState.Ready)
        {
            if (Time.time > lastTestTime + 1.0f)
            {
                _playTestSFXFlag = false;
                lastTestTime = Time.time;
                audioSource.PlayOneShot(sfxTestClip);
            }
        }
        if (state == OptionsState.Opened) state = OptionsState.Ready;
    }

    void CloseComplete() {
        gameObject.SetActive(false);
    }
    internal void OnClose()
    {
        state = OptionsState.Closed;
        GetComponent<RectTransform>().DOScale(Vector3.zero, .35f).OnComplete(CloseComplete);
        PlayerPrefs.Save();

    }

  
    public void InitUI() {
        // GFX Quality:

        // GFX Quality:
        int qualityIndex = OptionsManager.prevGFXQuality;
        var toggles = gfxToggleGroup.GetComponentsInChildren<Toggle>();
        if (toggles.Length > 0 && qualityIndex < toggles.Length)
        {
            toggles[qualityIndex].isOn = true;
            toggles[qualityIndex].group.RegisterToggle(gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex]);
            toggles[qualityIndex].group.NotifyToggleOn(gfxToggleGroup.GetComponentsInChildren<Toggle>()[qualityIndex]);
        }

        //Audio
        musicSlider.minValue = 0;
        musicSlider.maxValue = 1;
        sfxSlider.minValue = 0;
        sfxSlider.maxValue = 1;
        musicSlider.value = OptionsManager.music;
        sfxSlider.value =   OptionsManager.sfx;

        ToggleShine();
            
    }

  

    

    void OnConfirmQualityWarning(GameObject gameObject) {
        int val = 2;
        OptionsManager.ConfirmQuality(val, true); // 2 is high graphics
        ToggleShine();
        AnalyticsManager.LogUI("optionsQualityHighConfirmed", DesignEventType.Clicked);

    }

    void OnDeclineQualityWarning(GameObject gameObject)
    {
        int val = OptionsManager.prevGFXQuality == 2 ? 1 : OptionsManager.prevGFXQuality;
        OptionsManager.ConfirmQuality(val, true); // 1 is med graphics, 2 is high
        UpdateQualityToggles(val);
        AnalyticsManager.LogUI("optionsQualityHighDeclined", DesignEventType.Clicked);

    }

    void UpdateQualityToggles(int val) {
        if (!gfxToggleGroup.GetComponentsInChildren<Toggle>()[val].isOn)
        {
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[val].isOn = true;
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[val].group.RegisterToggle(gfxToggleGroup.GetComponentsInChildren<Toggle>()[val]);
            gfxToggleGroup.GetComponentsInChildren<Toggle>()[val].group.NotifyToggleOn(gfxToggleGroup.GetComponentsInChildren<Toggle>()[val]);
        }
    }

    public void OnToggleQuality(int index) {
        if (state == OptionsState.Ready)
            UIAudioManager.PlayClickSound();
        if (gfxToggleGroup.GetComponentsInChildren<Toggle>()[index].isOn)
        {
            if (index == 2 && OptionsManager.prevGFXQuality !=2 && state == OptionsState.Ready)
            {
                // High quality, show warning
                PopupBuilder p = new PopupBuilder();
                p.title = "Are you sure?";
                p.text = "High graphics requires a very powerful device and may slow down the game";
                p.confirmCallback = OnConfirmQualityWarning;
                p.declineCallback = OnDeclineQualityWarning;
                p.showConfirmButton = true;
                p.showDeclineButton = true;
                p.Show();

                AnalyticsManager.LogUI("optionsQualityHigh", DesignEventType.Clicked);
            }
            else if (index == 0) {
                OptionsManager.ConfirmQuality(index, state == OptionsState.Ready);
                ToggleShine();
                if(state == OptionsState.Ready)
                    AnalyticsManager.LogUI("optionsQualityLow", DesignEventType.Clicked);

            }
            else if(index==1)
            {
                OptionsManager.ConfirmQuality(index, state == OptionsState.Ready);
                ToggleShine();
                if (state == OptionsState.Ready)
                    AnalyticsManager.LogUI("optionsQualityMed", DesignEventType.Clicked);

            }
        }

    }

    private void ToggleShine()
    {
        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            foreach (Image img in toggle.GetComponentsInChildren<Image>())
            {
                if(img.gameObject.CompareTag("Checkmark Shine"))
                    img.enabled = toggle.isOn;
            }
        }
    }

    public void OnSetMusicVolume(float v) {
        AudioManager.SetMaxMusicVolume(v);
        OptionsManager.music = v;
        PlayerPrefs.SetFloat(OptionsManager.PREF_MUSIC, v);
        PlayerPrefs.Save();
        if (state == OptionsState.Ready)
            AnalyticsManager.LogDesign("optionsSetMusicVolume", v);

    }

    public void OnSetSFXVolume(float v) {
        AudioManager.SetSFXVolume(v);
        OptionsManager.sfx = v;
        PlayerPrefs.SetFloat(OptionsManager.PREF_SFX, v);
        PlayerPrefs.Save();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        audioSource.volume = v;
        if (state == OptionsState.Ready){
            _playTestSFXFlag = true;
            AnalyticsManager.LogDesign("optionsSetSFXVolume", v);
        }

    }

    public void OnToggleZenMode(bool t) {
        OptionsManager.zenMode = t;
        PlayerPrefs.SetInt(OptionsManager.PREF_ZEN, t?1:0);
        PlayerPrefs.Save();
        ToggleShine();
        if (state == OptionsState.Ready)
            AnalyticsManager.LogUI("optionsZenModeToggle", DesignEventType.Clicked);


    }

    public void OnClickZenModeInfo() {
        PopupBuilder b = new PopupBuilder();
        b.title = "Zen Mode";
        b.text = "Enable zen mode to remove the user interface and slow down the game.\n\nMakes the experience easier and more relaxing, but you will get less coins.";
        AnalyticsManager.LogUI("optionsZenModeInfo", DesignEventType.Clicked);

        PopupManager.ShowPopup(b);
    }



    public void OnCredits()
    {
        PopupBuilder b = new PopupBuilder();
        b.title = "Credits";
        b.text = Resources.Load<TextAsset>("Credits").text;
        b.textSize = 60;
        b.textAlignment = TMPro.TextAlignmentOptions.Left;
        PopupManager.ShowPopup(b);
        AnalyticsManager.LogUI("optionsCredits", DesignEventType.Clicked);

    }

    public void OnPrivacy() {
        AnalyticsManager.LogUI("optionsPrivacy", DesignEventType.Clicked);
        EasyMobileManager.AskForConsent();
        
    }

    public void OnConfirm() {
        MainMenu.CloseOptionsMenu();
    }

}
