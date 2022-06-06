using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class LocationServices : MonoBehaviour {
    public static LocationServices Instance;
    public double[] lats;
    public double[] lons;
    public GameObject dummyObject;
    public UnityEngine.UI.Text debug;
    public Transform parent;
    public static Vector3 Center { get; set; }
    private void Awake () {
        if(Instance != null)
            Instance = this;
    }
    void Start () {
        Input.location.Start ();
        Input.compass.enabled = true;
#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission (Permission.FineLocation)) {
            StartCoroutine (FindLocation ());
        } else {

            var callbacks = new PermissionCallbacks ();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission (Permission.FineLocation, callbacks);
        }
#else
        StartCoroutine (FindLocation ());
#endif
    }

    void SpawnObjects () {
        GameObject[] spawnedObjects = new GameObject[lats.Length];
        for (int i = 0; i < lats.Length; i++) {
            Vector3 pos = LatLonToVector3 (lats[i], lons[i]) - Center;
            spawnedObjects[i] = Instantiate (dummyObject, pos, Quaternion.identity, parent);
        }
        DebugText.Log ("Spawned " + lats.Length + " BLUE BALLS", GetType ().ToString ());
        DebugText.Log ("The arrow indicates the nearest one", GetType ().ToString ());
        CompassRotator.spawn = true;
        FindObjectOfType<Arrow> ().LookArTarget (FindNearestObject (spawnedObjects));
    }

    private Transform FindNearestObject (GameObject[] spawnedObjects) {
        float min = Mathf.Infinity;
        float dist;
        int index = 0;
        for (int i = 0; i < spawnedObjects.Length; i++) {
            dist = Vector3.Distance (spawnedObjects[i].transform.position, Camera.main.transform.position);
            if (dist < min) {
                index = i;
                min = dist;
            }
        }
        return spawnedObjects[index].transform;
    }

    IEnumerator FindLocation () {
        if (!Input.location.isEnabledByUser)
            yield break;
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds (1);
            maxWait--;
        }
        if (maxWait < 1) {
            DebugText.Log("Timed out", GetType().ToString());
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed) {
            DebugText.Log ("Unable to determine device location", GetType ().ToString ());
            yield break;
        } else {
            DebugText.Log ("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp, GetType ().ToString ());
            Center = LatLonToVector3 (Input.location.lastData.latitude, Input.location.lastData.longitude);
#if UNITY_WEBGL && !UNITY_EDITOR
            yield return new WaitUntil (() => WebContact.initialized);
#endif
            SpawnObjects ();
        }
    }
    public static Vector3 LatLonToVector3 (double lat, double lon) {
        lon = lon * 111320;
        lat = 110574 * lat;
        return new Vector3 ((float) lon, 0, (float) lat);
    }
#if UNITY_ANDROID
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
    }

    internal void PermissionCallbacks_PermissionGranted (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
        StartCoroutine (FindLocation ());
    }

    internal void PermissionCallbacks_PermissionDenied (string permissionName) {
        DebugText.Log ("PermissionCallbacks_PermissionDeniedAndDontAskAgain", GetType ().ToString ());
    }
#endif
}