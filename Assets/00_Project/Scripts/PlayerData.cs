using Newtonsoft.Json.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    public static bool IsAuth { get { return !string.IsNullOrEmpty (accessToken); } }
    public static int id;
    public static string accessToken;
    public new static string name;
    public static string mobile;
    public static bool isVerified;
    public static string city;
    public static void FillFromJson (JObject token) {
        id = token["data"].Value<int> ("id");
        accessToken = token["cGAccessToken"].ToString ();
        name = token["data"]["name"].ToString ();
        mobile = token["data"]["mobile"].ToString ();
        isVerified = token["data"].Value<bool> ("isVerified");
        city = token["data"]["city"].ToString ();
    }
}
