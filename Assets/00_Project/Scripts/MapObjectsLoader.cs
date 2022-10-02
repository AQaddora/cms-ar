using UnityEngine;
using ARLocation;
using Newtonsoft.Json.Linq;

public class MapObjectsLoader : MonoBehaviour {
    public static MapObjectsLoader Instance;
    public GameObject[] prefabs;
    private void Awake () {
        Instance = this;
        WebRequests.Instance.RefreshLocations ();
        //Invoke ("ReloadScene", 10);
    }
    public void SpawnObjects (JObject res) {
        foreach (Transform child in ARLocationManager.Instance.gameObject.transform) {
            Destroy (child.gameObject);
        }
        JArray jArray = res.Value<JArray> ("data");
        foreach (JToken jToken in jArray) {
            GameObject prefab = prefabs[jToken.Value<int> ("prefabIndex")];
            if (!prefab) continue;

            PlaceAtLocation.PlaceAtOptions placementOptions = new PlaceAtLocation.PlaceAtOptions () {
                HideObjectUntilItIsPlaced = false
            };
            Location location = new Location () {
                Latitude = jToken.Value<double> ("lat"),
                Longitude = jToken.Value<double> ("lon"),
                Altitude = jToken.Value<double> ("alt"),
                AltitudeMode = AltitudeMode.GroundRelative,
                Label = jToken["label"].ToString ()
            };
            PlaceAtLocation.CreatePlacedInstance (prefab, location, placementOptions);
        }
        if (ARLocationManager.Instance.gameObject.transform.childCount > 0)
            TempArrow.target = ARLocationManager.Instance.gameObject.transform.GetChild (0);
    }
    private void ReloadScene () {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (1).completed += (_) => UnityEngine.SceneManagement.SceneManager.LoadScene (1, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}