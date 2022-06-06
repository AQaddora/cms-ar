using UnityEngine;

public class CompassRotator : MonoBehaviour
{
    public static bool spawn = false;
    int timer = 0;
    void Awake () {
        Input.location.Start ();
        Input.compass.enabled = true;
        InvokeRepeating ("LogRotation", 3f, 3f);
    }
    void Update () {
        if (spawn && Input.compass.magneticHeading != 0) {
            transform.rotation = Quaternion.Euler (0, -Input.compass.magneticHeading, 0);
        }
    }
    void LogRotation () {
        if (spawn && Input.compass.magneticHeading != 0) {
            if (++timer >= 3)
                CancelInvoke ();
            DebugText.Log (transform.eulerAngles.ToString(), GetType ().ToString ());
        }
    }
}
