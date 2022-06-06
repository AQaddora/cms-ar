using UnityEngine;

public class CameraGyroTest : MonoBehaviour
{
    private void Awake () {
        Input.gyro.enabled = true;
    }
    void LateUpdate()
    {
        Quaternion orientation = Input.gyro.attitude;
        Quaternion rot = Quaternion.Inverse (new Quaternion (orientation.x, orientation.y, -orientation.z, orientation.w));
        transform.rotation = Quaternion.Lerp(transform.rotation, (Quaternion.Euler (new Vector3 (90.0f, 0.0f, 0.0f)) * rot), Time.deltaTime * 10);
        if(Input.location.isEnabledByUser) {
            transform.position = LocationServices.Center - LocationServices.LatLonToVector3 (Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
    }
}
