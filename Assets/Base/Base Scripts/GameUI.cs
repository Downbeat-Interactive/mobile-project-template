using DG.Tweening;
using TMPro;
using UnityEngine;


public class GameUI : MonoBehaviour
{
    public static GameUI Instance = null;
    [SerializeField]
    RectTransform exitButton = null;
    [SerializeField]
    RectTransform panel = null;
    [SerializeField]
    TextMeshProUGUI levelText = null;
    [SerializeField]
    GameObject popupText = null;
    [SerializeField]
    float popupAnimTime = 1.0f;
    [SerializeField]
    float popupStayTime = 1.0f;
    [SerializeField]
    TextMeshProUGUI scoreText = null;
    static bool _showing = false;

    [SerializeField]
    bool recordingMode = false;

    Vector2 originalPanelPos = Vector2.zero;


    private void Awake()
    {
        Instance = this;
        _showing = true;
        originalPanelPos = panel.anchoredPosition;
        if (Loader.IsSceneOpen(DefaultScene.MainMenu))
        {
            Instance.exitButton.localScale = Vector2.zero;

            _showing = false;
        }
        levelText.text = GameManager.GetCurrentLevelForDisplay();

#if UNITY_EDITOR
        if (recordingMode)
        {
            Instance.exitButton.gameObject.SetActive(false);
            Instance.levelText.gameObject.SetActive(false);
        }

#endif
        var rt = scoreText?.GetComponent<RectTransform>();
        rt?.DOScale(1.09f * rt.localScale, .85f).SetLoops(-1, LoopType.Yoyo);
    }

    public static GameObject ShowPopup(string text)
    {
        var t = Instantiate(Instance.popupText, Instance.transform).GetComponent<TextMeshProUGUI>();
        t.text = text;

        //IN
        t.DOMaxVisibleCharacters(text.Length, Instance.popupAnimTime).ChangeStartValue(0);
        t.DOScale(1.0f, Instance.popupAnimTime).ChangeStartValue(Vector3.zero).SetEase(Ease.OutBounce);

        float delay = Instance.popupAnimTime + Instance.popupStayTime;
        //OUT
        t.DOMaxVisibleCharacters(0, Instance.popupAnimTime / 2.0f).SetDelay(delay).SetEase(Ease.Linear);
        t.DOScale(0.0f, Instance.popupAnimTime / 2.0f).SetDelay(delay).OnComplete(() => { Destroy(t.gameObject); });

        return t.gameObject;
    }


    public void OnClickExit()
    {
        GameManager.isPlaying = false;
        Loader.FadeToScene(DefaultScene.MainMenu, true);
    }

    public static void ShowInGameUI()
    {
        if (!Instance || !Instance.exitButton || _showing) return;
        _showing = true;
        Instance.exitButton?.DOKill();

        if (Vector2.Distance(Instance.originalPanelPos, Instance.panel.anchoredPosition) > 10f)
        {
            Instance.panel.DOAnchorPosX(600f, .95f).SetRelative(true).SetEase(Ease.OutBounce);
        }
        Instance.exitButton?.DOScale(1.0f, .85f).SetEase(Ease.OutBounce);
        Instance.ShowGameScore();
    }

    void ShowGameScore()
    {
        scoreText?.DOFade(1f, .45f);
        levelText?.DOFade(1f, .45f);
    }

    void HideGameScore()
    {
        scoreText?.DOFade(0f, .45f);
        levelText?.DOFade(0f, .45f);
    }
    public static void HideInGameUI()
    {
        if (!Instance || !Instance.exitButton || !_showing) return;
        _showing = false;
        Instance.exitButton?.DOKill();
        Instance.panel.DOAnchorPosX(-600f, .45f).SetRelative(true).SetEase(Ease.InBack);
        Instance.exitButton?.DOScale(0.0f, .45f).SetEase(Ease.OutBounce);

        Instance.HideGameScore();
    }

    public static void ChangeLevelText(string txt)
    {
        if (Instance.levelText) Instance.levelText.text = txt;
    }

    public static void UpdateScore(int score, bool animate = false)
    {
        Instance.scoreText.text = score.ToString();
        Instance.scoreText.gameObject.SetActive(score > 0);
        if (animate)
        {
            var rt = Instance.scoreText.GetComponent<RectTransform>();
            rt.DOPunchScale(rt.localScale * .5f, .45f);
        }

    }

    private void OnEnable()
    {
        GameManager.OnWin += HideGameScore;
    }

    private void OnDisable()
    {
        GameManager.OnWin -= HideGameScore;
    }


}
