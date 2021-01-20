using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance = null;
    [SerializeField]
    RectTransform exitButton = null;
    [SerializeField]
    RectTransform metersPanel = null;

    [SerializeField]
    GameObject popupText = null;
    [SerializeField]
    float popupAnimTime = 1.0f;
    [SerializeField]
    float defaultPopupStayTime = 1.0f;

    static bool _showing = false;

    private void Awake(){
        Instance = this;
        Instance.exitButton.localScale = Vector2.zero;
        Instance.metersPanel.DOAnchorPosX(-600f, 0.0f).SetRelative(true);
        _showing = false;
    }

    public static void ShowPopupText(string text, float stayTime = -1f, Vector2 anchoredPosition = new Vector2()) {
        if (stayTime < 0)
            stayTime = Instance.defaultPopupStayTime;

        var t = Instantiate(Instance.popupText, Instance.transform).GetComponent<TextMeshProUGUI>();
        t.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        t.text = text;
        float delay = Instance.popupAnimTime + stayTime;

        #region in
        t.DOMaxVisibleCharacters(text.Length, Instance.popupAnimTime)
            .ChangeStartValue(0.0f);

        t.DOScale(1.0f, Instance.popupAnimTime)
            .ChangeStartValue(Vector3.zero)
            .SetEase(Ease.OutBounce);
        #endregion


        #region out
        t.DOMaxVisibleCharacters(0, Instance.popupAnimTime / 2.0f)
            .SetDelay(delay);

        t.DOScale(0.0f, Instance.popupAnimTime / 2.0f)
            .SetDelay(delay)
            .OnComplete(() => { Destroy(t.gameObject); });
        #endregion
    }


    public void OnClickExit() {
        GameManager.isPlaying = false;
        Loader.FadeToScene(DefaultScene.MainMenu, true);
    }

    public static void ShowInGameUI() {
        if (!Instance || !Instance.exitButton || _showing) return;
        _showing = true;
        Instance.exitButton.DOKill();

        Instance.metersPanel.DOAnchorPosX(600f, .95f).SetRelative(true).SetEase(Ease.OutBounce);
        Instance.exitButton.DOScale(1.0f, .85f).SetEase(Ease.OutBounce);
    }
    public static void HideInGameUI() {
        if (!Instance || !Instance.exitButton || !_showing) return;
        _showing = false;
        Instance.exitButton.DOKill();
        Instance.metersPanel.DOAnchorPosX(-600f, .45f).SetRelative(true).SetEase(Ease.InBack);
        Instance.exitButton.DOScale(0.0f, .45f).SetEase(Ease.OutBounce);
    
    }
}
