using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpscounter : MonoBehaviour
{

    [SerializeField]
    float refresh = 1;
    [SerializeField]
    TMPro.TextMeshProUGUI text = null;

    float fpsTimer = 0;
    float avgFramerate = 0;

    void Awake()
    {
        if (!Debug.isDebugBuild)
            Destroy(this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        // FPS
        float timelapse = Time.deltaTime;
        fpsTimer = fpsTimer <= 0 ? refresh : fpsTimer -= timelapse;

        if (fpsTimer <= 0)
        {
            avgFramerate = (int)(1f / timelapse);
            text.text = string.Format(avgFramerate.ToString("000"));
        }
    }
}
