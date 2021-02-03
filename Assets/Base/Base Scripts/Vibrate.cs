using UnityEngine;
public class Vibrate
{

    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject sysService;

    private static Vibrate _instance = null;
    public Vibrate()
    {
#if UNITY_ANDROID
#if !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        sysService = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        Debug.Log("Making vibrator");
#endif
#endif
    }

    //Functions from https://developer.android.com/reference/android/os/Vibrator.html
    public void vibrate()
    {
#if UNITY_ANDROID
#if !UNITY_EDITOR
        sysService.Call("vibrate");
#else
        Debug.Log("vibrate");
#endif
#endif
    }


    public void vibrate(long milliseconds)
    {

#if UNITY_ANDROID
#if !UNITY_EDITOR
        sysService.Call("vibrate", milliseconds);
#else
        Debug.Log("vibrate" + milliseconds.ToString());
#endif
#endif
    }

    public void vibrate(long[] pattern, int repeat)
    {
#if UNITY_ANDROID
#if !UNITY_EDITOR
        sysService.Call("vibrate", pattern, repeat);
#else
        Debug.Log("vibrate");
#endif
#endif

    }


    public void cancel()
    {
#if UNITY_ANDROID
#if !UNITY_EDITOR
        sysService.Call("cancel");
#else
        Debug.Log("cancel vibrate");
#endif
#endif

    }

    public bool hasVibrator()
    {
#if UNITY_ANDROID
#if !UNITY_EDITOR
        return sysService.Call<bool>("hasVibrator");
#else
        return false;  
#endif
#endif
    }

    public static void DoVibrate(long ms) {
        if(_instance == null)
            _instance = new Vibrate();
        if (_instance.hasVibrator()){
            _instance.vibrate(ms);
        }
    }
}