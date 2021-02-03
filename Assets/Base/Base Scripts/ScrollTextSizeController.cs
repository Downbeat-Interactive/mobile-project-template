using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollTextSizeController : MonoBehaviour
{
    [SerializeField]
    float maxHeight = 500;

    private RectTransform textTransform;

    private LayoutElement thisLayout;
    // Start is called before the first frame update
    void OnEnable()
    {
        thisLayout = GetComponent<LayoutElement>();
        textTransform = GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>();
    }

    void OnGUI()
    {
        if (textTransform.rect.height < maxHeight)
        {

            thisLayout.preferredHeight = textTransform.rect.height;
        }
        else {
            thisLayout.preferredHeight = maxHeight;
        }
    }
}
