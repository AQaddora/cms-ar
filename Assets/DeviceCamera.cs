using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class DeviceCamera : MonoBehaviour {
    public RawImage rw;
    public AspectRatioFitter arf;
    private WebCamTexture webCam;
    bool fixedRatio = false;
    private void Awake () {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission (Permission.Camera)) {
            var callbacks = new PermissionCallbacks ();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission (Permission.Camera, callbacks);
        }
#endif
    }
    void Start () {
        WebContact.Instance.HideHTMLLoading ();
        webCam = new WebCamTexture (Screen.width, Screen.height);
        rw.texture = webCam;
        webCam.Play ();
    }

    void Update () {
        if (fixedRatio || webCam.width < 100)
            return;
        fixedRatio = true;
        float rot = -webCam.videoRotationAngle;
        if (webCam.videoVerticallyMirrored)
            rot += 180;

        rw.rectTransform.localEulerAngles = Vector3.forward * rot;
        arf.aspectRatio = (float) webCam.width / webCam.height;

        if (webCam.videoVerticallyMirrored) {
            rw.uvRect = new Rect (1, 0, -1, 1);
        } else {
            rw.uvRect = new Rect (0, 0, 1, 1);
        }
    }
#if UNITY_ANDROID
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
    }

    internal void PermissionCallbacks_PermissionGranted (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
    }

    internal void PermissionCallbacks_PermissionDenied (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
    }
#endif
}
