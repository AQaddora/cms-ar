using UnityEngine;

public class TempArrow : MonoBehaviour
{
    public static Transform target;
    public TMPro.TMP_Text distanceText;
    private void Awake () {
        distanceText = GameObject.Find ("DistanceText").GetComponent<TMPro.TMP_Text> ();
    }
    void Update()
    {
        if(target != null) {
            transform.LookAt (target);
            distanceText.text = Vector3.Distance (transform.position, target.position).ToString("0.00") + "m";
        }
    }
}
