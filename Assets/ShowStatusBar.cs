using UnityEngine;

public class ShowStatusBar : MonoBehaviour {
    public static ShowStatusBar Instance;
    public static bool isLightBar = false;
    public static bool lastIsLightBar = false;
    public static AndroidJavaObject androidActivity;
    void Awake () {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void Start () {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
            androidActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        }
        ShowBar(true);
#endif
    }
    public static void ShowBar (bool lightStatusBar) {
#if UNITY_ANDROID && !UNITY_EDITOR
        androidActivity.Call ("runOnUiThread", new AndroidJavaRunnable (ShowStatusBarUIThread));
#endif
        isLightBar = lightStatusBar;
    }
    private static void ShowStatusBarUIThread () {
        using (var window = androidActivity.Call<AndroidJavaObject> ("getWindow")) {
            window.Call ("clearFlags", 1024);
            window.Call ("setStatusBarColor", 0);
            window.Call ("setNavigationBarColor", -1);
            using (var view = window.Call<AndroidJavaObject> ("getDecorView")) {
                int flags = view.Call<int> ("getSystemUiVisibility");
                //if (isLightBar)
                //    flags |= 0x00002000;
                //else
                //    flags &= ~0x00002000;
                view.Call ("setSystemUiVisibility", flags);
            }
        }
    }
}
