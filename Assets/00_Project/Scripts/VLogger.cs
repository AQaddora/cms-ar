using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VLogger : MonoBehaviour {
    public static VLogger Instance;
    public Color errorColor, infoColor, warningColor, successColor;
    private UITransition uITransition;
    public TMP_Text text;
    private string previousMessage;
    private Image img;
    private Image bottomImage;
    private float duration;
    private void Awake () {
        Instance = this;
        uITransition = GetComponent<UITransition> ();
        img = GetComponent<Image> ();
        bottomImage = transform.GetChild (0).GetComponent<Image> ();
    }
    float time = 0;
    private void Update () {
        if (bottomImage) {
            time += Time.deltaTime / duration;
            bottomImage.transform.localScale = new Vector3(Mathf.Lerp (1, 0, time),1 ,1);
        }
    }
    public static void ShowMessage (string msg, LoggerType type = LoggerType.error, float duration = 5f) {
        if (msg.Equals (Instance.previousMessage)) {
            return;
        }
        Instance.duration = duration;
        Instance.time = 0;
        if (type == LoggerType.info) {
            Instance.img.color = Instance.infoColor;
        } else if (type == LoggerType.warning) {
            Instance.img.color = Instance.warningColor;
        } else if (type == LoggerType.success) {
            Instance.img.color = Instance.successColor;
        } else {
            Instance.img.color = Instance.errorColor;
        }
        Instance.uITransition.CancelInvoke ();
        Instance.previousMessage = msg;
        Instance.Invoke ("ForgetPreviousMessage", duration + Instance.uITransition.duration);
        Instance.text.text = msg;
        Instance.uITransition.Show ();
        Instance.uITransition.Invoke ("Hide", duration);
    }
    private void ForgetPreviousMessage () {
        previousMessage = string.Empty;
    }

    public void ForceHide () {
        uITransition.Hide ();
        previousMessage = string.Empty;
    }
}
public enum LoggerType {
    error,
    info,
    warning,
    success
}