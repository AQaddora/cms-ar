using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
#endif

public class WebContact : MonoBehaviour
{
    public static WebContact Instance;
#if UNITY_WEBGL && !UNITY_EDITOR
    public static bool initialized = false;
    [DllImport ("__Internal")]
    private static extern string GetInitData ();
    [DllImport ("__Internal")]
    public static extern void ShowLoading ();
    [DllImport ("__Internal")]
    public static extern void HideLoading ();
#endif
    private void Awake () {
        if(Instance == null)
            Instance = this;
    }
    private void Start () {
#if UNITY_WEBGL && !UNITY_EDITOR
        //JObject initData = JObject.Parse (GetInitData ());
        //Debug.Log ("Fetching Data...");
        //Debug.Log (LocationServices.Instance.lats.Length);
        //Debug.Log (initData.ToString ());
        //if (initData["lats"] != null && initData["lats"].Type != JTokenType.Null && initData["lons"] != null && initData["lons"].Type != JTokenType.Null) {
        //    JArray latsJArr = initData.Value<JArray> ("lats");
        //    JArray lonsJArr = initData.Value<JArray> ("lons");
        //    double[] lats = new double[latsJArr.Count];
        //    double[] lons = new double[lonsJArr.Count];
        //    for (int i = 0; i < latsJArr.Count; i++) {
        //        lats[i] = latsJArr[i].Value<double>();
        //    }
        //    for (int i = 0; i < lonsJArr.Count; i++) {
        //        lons[i] = lonsJArr[i].Value<double>();
        //    }
        //    LocationServices.Instance.lats = LocationServices.Instance.lats.Concat (lats).ToArray ();
        //    LocationServices.Instance.lons = LocationServices.Instance.lons.Concat (lons).ToArray ();
        //    Debug.Log (LocationServices.Instance.lats);
        //}
        initialized = true;
#endif
    }
    public void ShowHTMLLoading () {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowLoading ();
#endif
    }
    public void HideHTMLLoading () {
#if UNITY_WEBGL && !UNITY_EDITOR
        HideLoading ();
#endif
    }
}
