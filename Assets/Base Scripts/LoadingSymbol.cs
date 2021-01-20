using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSymbol : MonoBehaviour
{
    [SerializeField]
    float delayTime = 1.0f;
    [SerializeField]
    float loopTime = 1.0f;
    [SerializeField]
    Image inner = null;

    Image mask   = null;
    Image circle = null;
    Image circle2 = null;
    CanvasGroup group = null;

    private void Awake()
    {
        circle = transform.GetChild(0).GetComponent<Image>();
        circle2 = transform.GetChild(1).GetComponent<Image>();
        mask = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        Hide();
    }
  

    public void ShowandAnimate() {


        circle.DOKill();
        circle2.DOKill();
        inner.DOKill();
        mask.DOKill();
        GetComponent<RectTransform>().DOKill();

        group.DOFade(1.0f, delayTime / 2.0f).SetDelay(delayTime / 2.0f).ChangeStartValue(0f);
        //circle.DOFade(1.0f,delayTime/2.0f).SetDelay(delayTime/2.0f).ChangeStartValue(new Color(circle.color.r,circle.color.g,circle.color.b,0.0f));
        mask.DOFillAmount(1.0f, loopTime).SetDelay(delayTime).SetLoops(-1, LoopType.Yoyo).ChangeStartValue(0.0f);

        DOTween.To(
            () => circle.pixelsPerUnitMultiplier,
            x => circle.pixelsPerUnitMultiplier = x,
            2.3f,
            loopTime + .2f).SetLoops(-1, LoopType.Yoyo).ChangeStartValue(.24f);

        GetComponent<RectTransform>().DORotate(Vector3.forward * 180.0f, loopTime).SetRelative(true).SetLoops(-1, LoopType.Incremental);
        inner.GetComponent<RectTransform>().DORotate(-Vector3.forward * 180f, loopTime).SetRelative(true).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        inner.DOFade(.5f, loopTime).SetLoops(-1, LoopType.Yoyo);

        DOTween.To(() => circle2.pixelsPerUnitMultiplier, (x) => { circle2.pixelsPerUnitMultiplier = x; circle2.SetVerticesDirty(); }, .77f,loopTime).ChangeStartValue(0.07f).SetLoops(-1, LoopType.Yoyo);
    }

    public void Hide() {
        circle.DOKill();
        circle2.DOKill();
        //circle.DOFade(0.0f,delayTime/2.0f);
        group.DOKill();
        group.DOFade(0.0f, delayTime / 2.0f).OnComplete(()=> {
            GetComponent<RectTransform>().DOKill();
            mask.DOKill();
        });
    }


}
