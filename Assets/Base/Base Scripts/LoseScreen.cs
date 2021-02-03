using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    [SerializeField]
    Image panel = null;

    [Range(0, 2.0f)]
    [SerializeField]
    float inAnimTime = 1.0f;

    [SerializeField]
    Button skipButton = null;

    [SerializeField]
    TextMeshProUGUI titleText = null;

    [SerializeField]
    string overrideTitleText = "";
    private void Awake(){
        panel.rectTransform.DOScale(1.0f, inAnimTime)
            .ChangeStartValue(Vector3.zero)
            .SetEase(Ease.OutBounce);

        if(overrideTitleText.Length>0)
            titleText.text = overrideTitleText;

        if(AdManager.AreRemoved())
            Destroy(skipButton.gameObject);
    }

    private Tween PlayCloseAnim(){
        return panel.rectTransform.DOScale(0.0f, inAnimTime/2.5f);
    }

    public void OnClickRetry()
    {
        GameManager.ContinueFromLoseScreen();
        PlayCloseAnim().OnComplete(() => {
            Loader.UnloadScene(DefaultScene.Lose);
        });
    }

    public void Skip() {
        RewardedAd.ShowRewardedAd(
            AdManager.SkipRewarded,
            completedAdCallback:        ()=> { GameManager.ContinueFromSkip(); },
            failedToCompleteAdCallback: () => {}
            );
    }
}
