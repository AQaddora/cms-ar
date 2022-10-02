using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[AddComponentMenu ("UI/URL_RawImage")]
public class URLRawImage : RawImage
{
    [SerializeField] string m_Url = null;
    public bool dontDestroyTexture = false;
    public bool setWhiteColorOnLoad = true;
    public string url {
        get {
            return m_Url;
        }
        set {
            if (m_Url == value)
                return;
            //color = Color.clear;
            if (!string.IsNullOrEmpty (m_Url) && !dontDestroyTexture)
                CacheManager.Instance.DestroyTexture (m_Url);
            m_Url = value;
            if (!string.IsNullOrEmpty(m_Url) && CacheManager.Instance)
                CacheManager.Instance.DownloadTexture(m_Url, FileType.rawImage, DownloadTextureCallback);
        }
    }
    protected override void Start () {
        base.Start ();
        if (!string.IsNullOrEmpty (m_Url) && CacheManager.Instance)
            CacheManager.Instance.DownloadTexture (m_Url, FileType.rawImage, DownloadTextureCallback);
    }
    void DownloadTextureCallback (Texture dtexture) {
        if(setWhiteColorOnLoad)
            color = Color.white;
        texture = dtexture;
        AspectRatioFitter fitter = GetComponent<AspectRatioFitter> ();
        if (fitter != null) {
            fitter.aspectRatio = texture.width / (float)texture.height;
        }
    }
    protected override void OnDestroy () {
        if (!string.IsNullOrEmpty (m_Url) && !dontDestroyTexture && CacheManager.Instance != null)
            CacheManager.Instance.DestroyTexture (m_Url);
    }
}
#if UNITY_EDITOR
[CustomEditor (typeof (URLRawImage))]
public class URLRawImageEditor : UnityEditor.UI.RawImageEditor {
    SerializedProperty m_Url;
    SerializedProperty m_SetWhiteColorOnLoad;
    SerializedProperty m_DontDestroyTexture;
    protected override void OnEnable () {
        base.OnEnable ();
        m_Url = serializedObject.FindProperty ("m_Url");
        m_SetWhiteColorOnLoad = serializedObject.FindProperty ("setWhiteColorOnLoad");
        m_DontDestroyTexture = serializedObject.FindProperty ("dontDestroyTexture");
    }
    public override void OnInspectorGUI () {
        serializedObject.Update ();
        EditorGUILayout.PropertyField (m_Url);
        EditorGUILayout.PropertyField (m_DontDestroyTexture);
        EditorGUILayout.PropertyField (m_SetWhiteColorOnLoad);
        serializedObject.ApplyModifiedProperties ();
        base.OnInspectorGUI ();
    }

    [MenuItem ("GameObject/UI/URL_RawImage")]
    private static void CreateURLRawImageGameObject () {
        GameObject obj = new GameObject ("URL_RawImage");
        GameObject canvasObj;
        if (Selection.activeTransform) {
            if (Selection.activeTransform.GetComponentInParent<Canvas> ()) {
                obj.transform.parent = Selection.activeTransform;
            } else {
                canvasObj = new GameObject ("Canvas", new System.Type[] { typeof (Canvas), typeof (CanvasScaler), typeof (GraphicRaycaster) });
                canvasObj.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.transform.parent = Selection.activeTransform;
                obj.transform.parent = canvasObj.transform;
            }
        } else {
            if(FindObjectOfType<Canvas> ()) {
                canvasObj = FindObjectOfType<Canvas> ().gameObject;
                obj.transform.parent = canvasObj.transform;
            } else {
                canvasObj = new GameObject ("Canvas", new System.Type[] { typeof (Canvas), typeof (CanvasScaler), typeof (GraphicRaycaster) });
                canvasObj.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.transform.parent = Selection.activeTransform;
                obj.transform.parent = canvasObj.transform;
            }
        }
        obj.transform.localScale = Vector3.one;
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.AddComponent<URLRawImage> ();
        obj.AddComponent<AspectRatioFitter> ().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        obj.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
        Selection.activeObject = obj;
    }
}
#endif