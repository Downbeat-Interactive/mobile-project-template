using DG.Tweening;
using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField]
    Image fader = null;

    [SerializeField]
    RectTransform menuParent = null;

    [SerializeField]
    RectTransform optionsMenu = null;

    bool _doneLoading = false;

    private void Awake()
    {
        Instance = this;
        optionsMenu.DOScale(0.0f, 0.0f).OnComplete(() => {
            optionsMenu.gameObject.SetActive(false);
        });


        BannerAd.ShowBannerAd(AdPlacement.PlacementWithName("MainMenuBanner"), BannerAdPosition.Bottom);
    }
    private void Start()
    {
        _doneLoading = false;
        menuParent.DOScale(1.0f, .65f).ChangeStartValue(Vector3.zero).SetEase(Ease.OutElastic);
#if UNITY_EDITOR
        if (GameManager.Instance.useTestLevel)
        {
            Loader.LoadSceneBackground("level-test", fader, () => { _doneLoading = true; });
            return;
        }
#endif
        Loader.LoadSceneBackground("level-" + GameManager.GetCurrentLevelWithRepeats().ToString(), fader, () => { _doneLoading = true; });
    }

    public void StartButtonClicked(Button button)
    {
        StartCoroutine(StartEnum());
        //BannerAd.HideAllShowing();
        GameUI.ShowInGameUI();
        button.enabled = false;

    }
    public void OptionsButtonClicked()
    {
        optionsMenu.gameObject.SetActive(true);
        optionsMenu.DOScale(1.0f, .85f).SetEase(Ease.OutBounce).ChangeStartValue(Vector3.zero);
        optionsMenu.GetComponent<OptionsMenu>().OnOpen();
    }
    public static void CloseOptionsMenu()
    {
        Instance.optionsMenu.DOScale(0.0f, .35f).OnComplete(() => {
            Instance.optionsMenu.gameObject.SetActive(true);
        });
    }


    private IEnumerator StartEnum()
    {
        while (!_doneLoading)
        {
            yield return new WaitForSeconds(.5f);
        }
        GameManager.StartGame();

        menuParent.DOKill();
        menuParent.DOScale(0.0f, .45f).SetEase(Ease.InBounce).OnComplete(() => {
            Loader.UnloadScene(DefaultScene.MainMenu);
        });
    }
}
