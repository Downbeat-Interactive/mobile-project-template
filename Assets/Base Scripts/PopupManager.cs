using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public delegate void PopupCallback(GameObject popup = null);

public class PopupBuilder {
    public string title = "Title";
    public string text = "Content";
    public Sprite graphic = null;
    public bool showConfirmButton = true;
    public bool showDeclineButton = false;
    public TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;

    public float textSize = 0;

    public PopupCallback confirmCallback = null;
    public PopupCallback declineCallback = null;
    public float delay = 0;
    public Color? graphicTint = null;
    public bool blockBackgroundRaycasts = true;
    public GameObject Show() {
        return PopupManager.ShowPopup(this);
    }

    public PopupBuilder() {
        this.title = "Title";
        this.text = "Content";
        this.graphic = null;
        this.showConfirmButton = true;
        this.showDeclineButton = false;
        this.confirmCallback = null;
        this.declineCallback = null;
        this.textSize = 0;
        this.textAlignment = TextAlignmentOptions.Center;
        this.delay = 0;
        this.blockBackgroundRaycasts = true;
        Color? gfxTint = null;
    }
    public PopupBuilder(
        string title, string text, Sprite graphic = null, bool showConfirmButton = true,
        bool showDeclineButton = false, PopupCallback confirmCallback = null,
        PopupCallback declineCallback = null, float textSize = 0,
        TextAlignmentOptions textAlign = TextAlignmentOptions.Center, float delay = 0,
        Color? gfxTint = null, bool blockBackgroundRaycasts = true
        )
    {
        this.title = title;
        this.text = text;
        this.graphic = graphic;
        this.showConfirmButton = showConfirmButton;
        this.showDeclineButton = showDeclineButton;
        this.confirmCallback = confirmCallback;
        this.declineCallback = declineCallback;
        this.textSize = textSize;
        this.textAlignment = textAlign;
        this.delay = delay;
        this.graphicTint = gfxTint;
        this.blockBackgroundRaycasts = blockBackgroundRaycasts;

    }
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField]
    GameObject popupPrefab;

    List<GameObject> openPopups = new List<GameObject>();

    [SerializeField]
    float animationDuration;




    private void Awake(){
        if (PopupManager.Instance)
            Destroy(this.gameObject);
        else {
            Instance = this;
        }
    }

    public void OnClickConfirm(GameObject thisPopup)
    {
        
        UIAudioManager.PlayClickSound();
        if (thisPopup){
            var components = thisPopup.GetComponentInChildren<PopupComponents>();
            components.confirm.interactable = false;
            components.confirmCallback?.Invoke(thisPopup);
            Close(thisPopup);
        }
    }
    public void OnClickDecline(GameObject thisPopup)
    {
        UIAudioManager.PlayClickSound();

        if (thisPopup) {
        var components = thisPopup.GetComponentInChildren<PopupComponents>();
        components.decline.interactable = false;
        components.declineCallback?.Invoke(thisPopup);
        Close(thisPopup);
        }

    }


    public static GameObject ShowPopup(PopupBuilder builder) {
        if (builder != null &&Instance!=null)
        {
            return Instance.Show(builder);
        }
        else {
            Debug.LogError("PopupBuilder or PopupManager was null, on stack or heap?");
            return null;
        }
    }

    public static GameObject ShowPopup(string title, string text,
        Sprite graphic = null,
        bool showConfirmButton = true,
        bool showDeclineButton = false,
         PopupCallback confirmCallback = null,
         PopupCallback declineCallback = null,
         float textSize = 0,
         TextAlignmentOptions textAlign = TextAlignmentOptions.Center,
         float delay = 0,
         Color? gfxTint = null,
         bool blockBackgroundRaycasts = true
         )
    {
        return Instance.Show(title, text, graphic, showConfirmButton, showDeclineButton,confirmCallback, declineCallback, textSize, textAlign, delay, gfxTint, blockBackgroundRaycasts);
    }


    public GameObject Show(PopupBuilder builder) {
        return Show(builder.title, builder.text, builder.graphic, builder.showConfirmButton, builder.showDeclineButton, builder.confirmCallback, builder.declineCallback, builder.textSize, builder.textAlignment, builder.delay, builder.graphicTint, builder.blockBackgroundRaycasts);
    }

    public GameObject Show(
        string title, string text,
        Sprite graphic = null,
        bool showConfirmButton = true,
        bool showDeclineButton = false,
         PopupCallback confirmCallback = null,
         PopupCallback declineCallback = null,
         float textSize = 0,
         TextAlignmentOptions textAlign = TextAlignmentOptions.Center,
         float delay = 0,
         Color? gfxTint = null,
         bool blockBackgroundRaycasts = true
        )
    {
        GameObject thisPopup = Instantiate(popupPrefab, this.transform);


        if (!blockBackgroundRaycasts)
            thisPopup.GetComponent<Image>().raycastTarget = false;
        
        PopupComponents components = thisPopup.GetComponentInChildren<PopupComponents>();
        components.confirmCallback = confirmCallback;
        components.declineCallback = declineCallback;
        openPopups.Add(thisPopup);

        TextMeshProUGUI titleTMP = components.title;
        TextMeshProUGUI textTMP = components.text;
        Image image = components.image;
        Button confirmButton = components.confirm;
        Button declineButton = components.decline;


        titleTMP.text = title;
        textTMP.text = text;

        textTMP.alignment = textAlign;
        
        if (textSize > 0) {
            textTMP.fontSize = textSize;
        }


        if (graphic == null){
            Destroy(image.gameObject);
        }
        else{
            image.sprite = graphic;
            image.color = gfxTint ?? Color.white;
        }


        if (!showConfirmButton){
            Destroy(confirmButton.gameObject);
        }
        else{
            confirmButton.onClick.AddListener(()=>OnClickConfirm(thisPopup));
        }


        if (!showDeclineButton){
            Destroy(declineButton.gameObject);
        }
        else{
            declineButton.onClick.AddListener(()=>OnClickDecline(thisPopup));
        }

        Open(thisPopup, delay);
        return thisPopup;
    }



    public void Close(GameObject popup) {
        if (popup == null) return;
        popup.GetComponent<RectTransform>()
            .DOScale(0, animationDuration)
            .ChangeStartValue(new Vector3(1,1,1))
            .SetEase(Ease.InBounce)
            .OnComplete(()=> {Destroy(popup); });
    }

    private void Open(GameObject popup, float delay = 0) {
        if (popup == null) return;

        PopupComponents components = popup.GetComponentInChildren<PopupComponents>();

        popup.GetComponent<RectTransform>()
            .DOScale(1, animationDuration)
            .ChangeStartValue(new Vector3(0, 0, 0))
             .SetEase(Ease.OutBounce)
             .SetDelay(delay);

        components.title.transform.parent.GetComponent<RectTransform>()
              .DOScale(1, animationDuration + .3f)
              .ChangeStartValue(new Vector3(0, 0, 0))
              .SetEase(Ease.OutBounce);

        components.text.transform.parent.GetComponent<RectTransform>()
             .DOScale(1, animationDuration + .3f)
             .ChangeStartValue(new Vector3(0, 0, 0))
             .SetEase(Ease.OutBounce)
             .SetDelay(delay);


        if (components.image != null) {
            components.image.GetComponent<RectTransform>()
                .DOScale(1, animationDuration+.7f)
                .ChangeStartValue(new Vector3(0, 0, 0))
                .SetEase(Ease.OutBounce)
                .SetDelay(delay);
        }
    }

}
