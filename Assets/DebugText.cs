using UnityEngine;

public class DebugText : MonoBehaviour
{
    private static UnityEngine.UI.Text text;
    private static int index = 0;
    private void Awake () {
        text = GetComponent<UnityEngine.UI.Text> ();
    }
    public static void Log (string log, string type = "unknown") {
        log = "<Color=green>[" + type + "] </Color>" + log;
        Debug.Log (log);
        log = text.text + index + ". " + log + "\n";
        text.text = log;
    }
}
