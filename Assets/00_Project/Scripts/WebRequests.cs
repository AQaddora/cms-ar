using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests : MonoBehaviour {
    public static WebRequests Instance;
    public static string API_BASE = "https://api-alwosta-tv.herokuapp.com/api/v1/mobile/";
    //public static string SOCKET_BASE = "https://portal.chickchack.net:7005/";

    private void Awake () {
        if (Instance == null) {
            DontDestroyOnLoad (gameObject);
            Instance = this;
        } else {
            Destroy (gameObject);
        }
    }
    #region AuthAPIs
    public void Register (string name, string mobileNumber, string email, string password, string city, string area, Action<JObject> callback) {
        string url = API_BASE + "auth/signup";
        JObject form = new JObject ();
        form.Add ("name", name);
        form.Add ("mobile", mobileNumber);
        form.Add ("email", email);
        form.Add ("password", password);
        form.Add ("city", city);
        form.Add ("area", area);
        StartCoroutine (PostRequest (url, form.ToString (), callback, true, true));
    }

    public void Login (string mobileNumber, string password, Action<JObject> callback) {
        string url = API_BASE + "auth/login";
        JObject form = new JObject ();
        form.Add ("mobile", mobileNumber);
        form.Add ("password", password);
        StartCoroutine (PostRequest (url, form.ToString (), callback, true, true));
    }
    public void VerifyMobile (string code, string mobile, Action<JObject> callback) {
        string url = API_BASE + "auth/verify-mobile";
        JObject form = new JObject ();
        form.Add ("code", code);
        form.Add ("mobile", mobile);
        StartCoroutine (PutRequest (url, form.ToString (), callback, true, true));
    }
    #endregion
    #region SpawnObjects
    public void RefreshLocations () {
        if (PlayerData.IsAuth) {
            string url = API_BASE + "location/get-list";
            StartCoroutine (GetRequest (url, MapObjectsLoader.Instance.SpawnObjects));
        }
    }
    #endregion
    #region Bare requests
    IEnumerator GetRequest (string uri, Action<JObject> callback, bool showLoading = true, bool callbackOnError = false) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get (uri)) {
            webRequest.SetRequestHeader ("Authorization", "Bearer " + PlayerData.accessToken);
            yield return webRequest.SendWebRequest ();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                ShowErrorMessage ("Error: " + webRequest.error);
            } else {
                try {
                    JObject res = JObject.Parse (webRequest.downloadHandler.text);
                    if (res.Value<int> ("statusCode") == 200)
                        callback?.Invoke (res);
                    else {
                        ShowErrorMessage (res.Value<string> ("message"));
                        if (callbackOnError)
                            callback?.Invoke (res);
                    }
                } catch (Exception) {
                    ShowErrorMessage ("Server Under Maintenance");
                    if (callbackOnError)
                        callback?.Invoke (null);
                }
            }
        }
    }
    IEnumerator DeleteRequest (string uri, Action<JObject> callback) {
        using (UnityWebRequest webRequest = UnityWebRequest.Delete (uri)) {
            webRequest.SetRequestHeader ("Content-Type", "application/json; charset=utf-8");
            if (PlayerData.IsAuth)
                webRequest.SetRequestHeader ("Authorization", "Bearer " + PlayerData.accessToken);
            yield return webRequest.SendWebRequest ();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                ShowErrorMessage ("Error: " + webRequest.error);
            } else {
                callback?.Invoke (null);
            }
        }
    }
    IEnumerator PutRequest (string uri, string bodyData, Action<JObject> callback, bool showLoading = true, bool callbackOnError = false) {
        using (UnityWebRequest webRequest = UnityWebRequest.Put (uri, bodyData)) {
            webRequest.SetRequestHeader ("Content-Type", "application/json; charset=utf-8");
            if (PlayerData.IsAuth)
                webRequest.SetRequestHeader ("Authorization", "Bearer " + PlayerData.accessToken);
            yield return webRequest.SendWebRequest ();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                ShowErrorMessage ("Error: " + webRequest.error);
                if (callbackOnError)
                    callback?.Invoke (new JObject ());
            } else {
                try {
                    JObject res = JObject.Parse (webRequest.downloadHandler.text);
                    if (res.Value<int> ("statusCode") == 200)
                        callback?.Invoke (res);
                    else {
                        ShowErrorMessage (res.Value<string> ("message"));
                        if (callbackOnError)
                            callback?.Invoke (res);
                    }
                } catch (Exception) {
                    ShowErrorMessage ("Server Under Maintenance");
                    if (callbackOnError)
                        callback?.Invoke (null);
                }
            }
        }
    }
    IEnumerator PostRequest (string uri, string bodyData, Action<JObject> callback, bool showLoading = true, bool callbackOnError = false) {
        using (UnityWebRequest webRequest = new UnityWebRequest (uri, "POST",
            new DownloadHandlerBuffer (),
            new UploadHandlerRaw (Encoding.UTF8.GetBytes (bodyData)))) {
            webRequest.SetRequestHeader ("Content-Type", "application/json; charset=utf-8");
            if (PlayerData.IsAuth)
                webRequest.SetRequestHeader ("Authorization", "Bearer " + PlayerData.accessToken);
            yield return webRequest.SendWebRequest ();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
                ShowErrorMessage ("Error: " + webRequest.error);
                if (callbackOnError) {
                    JObject res = new JObject ();
                    res.Add ("statusCode", 500);
                    callback?.Invoke (res);
                }
            } else {
                try {
                    JObject res = JObject.Parse  (webRequest.downloadHandler.text);
                    if (res.Value<int> ("statusCode") == 200)
                        callback?.Invoke (res);
                    else {
                        ShowErrorMessage (res.Value<string> ("message"));
                        if (callbackOnError)
                            callback?.Invoke (res);
                    }
                } catch (Exception) {
                    ShowErrorMessage ("Server Under Maintenance!");
                    if (callbackOnError)
                        callback?.Invoke (null);
                }
            }
        }
    }
    #endregion
    public void ShowErrorMessage (string message) {
        VLogger.ShowMessage ("ERROR: " + message);
    }
}
