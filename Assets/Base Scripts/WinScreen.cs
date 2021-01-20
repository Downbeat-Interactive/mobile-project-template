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

    [Range(0,2.0f)][SerializeField]
    float inAnimTime=1.0f;

    [SerializeField]
    TextMeshProUGUI titleText = null;
    private void Awake(){
        panel.rectTransform.DOScale(1.0f, inAnimTime)
            .ChangeStartValue(Vector3.zero)
            .SetEase(Ease.OutBounce);

        titleText.text = "Level " + (GameManager.GetCurrentLevel() - 1).ToString() + " complete!";
    }

    private Tween PlayCloseAnim() {
        return panel.rectTransform.DOScale(0.0f, inAnimTime/2.5f);
    }

    public void OnClickNext() {
        PlayCloseAnim().OnComplete(()=>{
            GameManager.ContinueFromWinScreen();
        });
    }
}
