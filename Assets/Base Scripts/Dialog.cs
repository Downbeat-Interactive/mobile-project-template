using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [SerializeField]
    public float animTime = .45f;
    [SerializeField]
    public float displayTime = 4.0f;

    [SerializeField]
    RectTransform rt = null;
    [SerializeField]
    TMPro.TextMeshProUGUI tmp = null;
    [SerializeField]
    CanvasGroup group;

    public RectTransform anchor = null;
    // Start is called before the first frame update
    void Start()
    {
        // *** IN ***
        rt.DOScaleY(1.0f,animTime*.9f)
            .SetEase(Ease.OutBounce)
            .ChangeStartValue(new Vector3(0,0,1.0f));
        
        rt.DOScaleX(1.0f, animTime)
            .SetEase(Ease.OutBounce);
        
        group.DOFade(1.0f, animTime).ChangeStartValue(new Color(0,0,0,0));


        // *** OUT ***
        rt.DOScaleY(0.0f, animTime * 1.0f)
           .SetDelay(animTime+displayTime);

        rt.DOScaleX(0.0f, animTime*.9f)
            .SetDelay(animTime+displayTime);

        group.DOFade(0.0f, animTime)
            .SetDelay(animTime + displayTime);

    }

    private void OnGUI()
    {
        if (anchor)
            rt.position = anchor.position;
    }
    internal void SetText(string v)
    {
        tmp.text = v;
    }

    internal void SetAnchor(RectTransform _anchor)
    {
        anchor = _anchor;
    }
}
