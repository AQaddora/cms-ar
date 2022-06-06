using UnityEngine;

public class Arrow : MonoBehaviour {
    private Transform target;
    public UnityEngine.UI.Text text;
    public void LookArTarget (Transform target) {
        this.target = target;
        DebugText.Log ("Looking at target: " + target.name, GetType ().ToString ());
    }
    private void Update () {
        if (target != null) {
            transform.LookAt (target);
            text.text = Vector3.Distance (Camera.main.transform.position, target.position) + "m";
        }
    }
}
