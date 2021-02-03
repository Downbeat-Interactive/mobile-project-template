using EasyMobile;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SecretDebugMenu : MonoBehaviour, IPointerClickHandler {
    public static SecretDebugMenu Instance = null;
    private static int numClicks = 0;

    [SerializeField]
    GameObject levelPop = null;

    private static int  popupIndex = 0;

    private static string TITLE = "Debug menu";
    private static string DESC = "For developer use only";
    private static string[,] buttons = new string[,]
    {
        { "Set level", "Next", "Close" },
        { "No Ads", "Next", "Back" },
        { "Clear prefs", "Next", "Back" },
    };

    private static Action[,] buttonCallbacks = new Action[,]
    {
        { SetLevelCallback, NextCallback, CloseCallback },
        { NoAdsCallback, NextCallback, BackCallback },
        { ResetPrefsCallback, NextCallback, BackCallback },

    };


    void Awake() {
        Instance = this;
    }
    static GameObject input=null;

    private static void SetLevelCallback() {
        input = Instantiate(Instance.levelPop, Instance.GetComponentsInParent<Canvas>()[0].transform);
        
        UnityEngine.Events.UnityAction cb = () =>
        {
            GameManager.SetCurrentLevel(int.Parse(input.GetComponentInChildren<TMP_InputField>().text));
            Destroy(input.gameObject);
        };
        input.GetComponentInChildren<Button>().onClick.AddListener(cb);
    }
    private static void NoAdsCallback() {
        NativeUI.ShowToast("No ads");
        AdManager.RemoveForTime(10);
    }
    private static void NextCallback() {
        popupIndex++;
        ShowPopup();
    }
    private static void BackCallback() {
        popupIndex--;
        ShowPopup();
    }
    private static void CloseCallback() {

    }

    private static void ResetPrefsCallback(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    private static void ShowPopup()
    {
        NativeUI.AlertPopup alert = NativeUI.ShowThreeButtonAlert(
             TITLE,
             DESC,
             buttons[popupIndex,0],
             buttons[popupIndex,1],
             buttons[popupIndex,2]
        );

        // Subscribe to the event
        if (alert != null)
        {
            alert.OnComplete += OnAlertCompleteHandler;
        }

    }
    // The event handler
    static void  OnAlertCompleteHandler(int buttonIndex)
    {
        buttonCallbacks[popupIndex, buttonIndex]?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        numClicks++;
        if (numClicks >= 13) {
            popupIndex = 0;
            ShowPopup();
        }

#if UNITY_EDITOR
        SetLevelCallback();
#endif
    }



    private void OnEnable()
    {
        numClicks = 0;
    }
    private void OnDisable()
    {
        numClicks = 0;
    }
}
