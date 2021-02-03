using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ScreenFadeController : MonoBehaviour
{
    public static ScreenFadeController Instance;
    CanvasGroup group;

    [SerializeField]
    float fadeTime = .5f;

    [SerializeField]
    float timeBetweenSteps = 0.01f;

    LoadingSymbol loadingSymbol = null;


    void Awake()
    {
        if (Instance)
            Destroy(this.gameObject);
        else
            Instance = this;

        group = GetComponentInChildren<CanvasGroup>();
        loadingSymbol = GetComponentInChildren<LoadingSymbol>();

        DontDestroyOnLoad(this.gameObject);
    }

    public static void FadeIn(float overrideTime = -1) {
        Instance.FadeInTween(overrideTime);
    }
    public static void FadeOut(float overrideTime = -1)
    {
        Instance.FadeOutTween(overrideTime);
    }

    public Tween FadeInTween(float overrideTime = -1)
    {
        group.DOKill();
        loadingSymbol.Hide();
        return group.DOFade(0.0f, overrideTime >= 0 ? overrideTime : fadeTime).OnComplete(()=> { group.blocksRaycasts = false; });
    }

    public Tween FadeOutTween(float overrideTime = -1)
    {
        loadingSymbol.ShowandAnimate();
        group.DOKill();
        return group.DOFade(1.0f, overrideTime >= 0 ? overrideTime : fadeTime).OnComplete(() => { group.blocksRaycasts = true; });
    }
}
