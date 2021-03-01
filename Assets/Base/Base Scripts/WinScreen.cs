using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField]
    Image panel = null;

    [Range(0, 2.0f)]
    [SerializeField]
    float inAnimTime = 1.0f;

    [SerializeField]
    TextMeshProUGUI titleText = null;

    [SerializeField]
    TextMeshProUGUI scoreText = null;

    [SerializeField]
    RectTransform[] stars = null;
    private void Awake()
    {
        panel.rectTransform.DOScale(1.0f, inAnimTime)
            .ChangeStartValue(Vector3.zero)
            .SetEase(Ease.OutBounce);

        titleText.text = titleText.text.Replace("{N}", GameManager.GetCurrentLevelIndex().ToString());

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].DOScale(0f, 0f);
        }


        var rt = scoreText.GetComponent<RectTransform>();
        rt.DOScale(rt.localScale * 1.1f, .65f).SetLoops(-1, LoopType.Yoyo);

        int current = 0;
        DOTween.To(() => current, (s) => { current = s; }, LevelManager.GetScore(), 2f).OnUpdate(() => {
            scoreText.text = "+" + current;
        }).SetEase(Ease.OutBounce);


        GameUtil.WaitThenDo(1f, InAnimation);
    }


    private void InAnimation()
    {
        if (stars == null || stars.Length <= 0)
            return;
        float delay = 0;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].DOScale(1f, .55f).SetDelay(delay).SetEase(Ease.OutBounce).OnComplete(() => {
            });
            stars[i].DOPunchRotation(Vector3.forward * Random.Range(-30f, 30f), 1f)
                .SetDelay(delay + .55f)
                .OnComplete(
                    () =>
                    {
                        if (i < stars.Length)
                            stars[i].GetComponent<DOTweenVisualManager>().enabled = true;
                    });

            delay += .65f;
        }
    }

    private float PlayCloseAnim()
    {
        float output = inAnimTime / 2.5f;
        panel.rectTransform.DOScale(0.01f, output);
        return output;
    }
    bool _clickedNext = false;
    public void OnClickNext()
    {
        if (_clickedNext)
            return;
        _clickedNext = true;
        GameUtil.WaitThenDo(PlayCloseAnim(), GameManager.ContinueFromWinScreen);
    }
}
