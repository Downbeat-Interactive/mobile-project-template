using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupComponents : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;
    public Button confirm;
    public Button decline;
    public Image image;

    public PopupCallback confirmCallback;
    public PopupCallback declineCallback;

}
