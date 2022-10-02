using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen Instance;
    public UITransition fullLoading;
    public UITransition smallLoading;
    public TextMeshProUGUI fullText;
    public TextMeshProUGUI smallText;
    public GameObject progressBar;

    void Awake () {
        Instance = this;
        if (!fullText)
            fullText = fullLoading?.GetComponentInChildren<TextMeshProUGUI> ();
        if (!smallText)
            smallText = smallLoading?.GetComponentInChildren<TextMeshProUGUI> ();
    }
    public void ShowFull (string loadingText = "") {
        if (fullText && progressBar) {
            fullText.text = loadingText;
            fullLoading.Show ();
        }
    }
    public void ShowSmall (string loadingText = "") {
        if(smallText && smallLoading) {
            smallText.text = loadingText;
            smallLoading.Show ();
        }
    }
    public void HideFull () {
        if (fullLoading && fullLoading.shown) {
            fullLoading.Hide ();
        }
    }
    public void HideSmall () {
        if (smallLoading) {
            smallLoading.Hide ();
        }
    }
}
