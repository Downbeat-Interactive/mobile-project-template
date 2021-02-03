using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
public class ReviewManager : MonoBehaviour
{
    static readonly string NOT_FIRST_OPEN = "Not First Open";
    static readonly string HAS_REVIEWED = "Reviewed";

    static bool hasReviewed = false;
    // Start is called before the first frame update
    void Awake()
    {
        // TODO check if has opened
        if (PlayerPrefs.GetInt(NOT_FIRST_OPEN, 0) == 0)
        {
            PlayerPrefs.SetInt(NOT_FIRST_OPEN, 1);
            PlayerPrefs.Save();
        }
        


    }

    public static bool HasReviewed() {
        hasReviewed = PlayerPrefs.GetInt(HAS_REVIEWED, 0) >= 1;
        return hasReviewed;
    }

    private static void RatingCallback(StoreReview.UserAction action) {
        switch (action)
        {
            case StoreReview.UserAction.Refuse:
                break;
            case StoreReview.UserAction.Postpone:
                break;
            case StoreReview.UserAction.Feedback:
                PopupBuilder b = new PopupBuilder();
                b.blockBackgroundRaycasts = true;
                b.text = "Please email developer@jacksonsdean.com with feedback";
                b.title = "Give feedback";
                b.showDeclineButton = false;
                b.showConfirmButton = true;
                b.Show();
                break;
            case StoreReview.UserAction.Rate:
                PlayerPrefs.SetInt(HAS_REVIEWED, 1);
                PlayerPrefs.Save();
                AnalyticsManager.LogDesign("reviewGiven", DesignEventType.Choice);

                break;
            default:
                break;
        }
    }

    public static void RequestReview() {
        if (PlayerPrefs.GetInt(NOT_FIRST_OPEN, 0) != 0 && !HasReviewed()) {
            AnalyticsManager.LogDesign("reviewRequested",DesignEventType.Opened);
            StoreReview.RequestRating(RatingDialogContent.Default, RatingCallback);
        }
    }

   
}
