using UnityEngine;

[RequireComponent (typeof (CanvasGroup))]
public class UIFade : UITransition {
    [HideInInspector] public CanvasGroup canvasGroup;
    [Header ("Fade Custom Properties")]
    [Space (20)]
    [Range (0, 1)] public float alpha = 1;
    void Awake () {
        canvasGroup = GetComponent<CanvasGroup> ();
        if (alpha <= 0)
            Hide ();
        else
            Show ();
        shown = alpha > 0;
    }
    private void Start () {
        SetActiveSelf (shown);
    }
    void Update () {
        if (canvasGroup.alpha == 1 && alpha == 1)
            return;
        canvasGroup.alpha = Mathf.Clamp01 (canvasGroup.alpha + ((alpha == 1) ? 1 : -1) * Time.deltaTime / duration);
        canvasGroup.interactable = alpha > 0.5f;
        canvasGroup.blocksRaycasts = alpha > 0.5f;
        if (canvasGroup.alpha == 0) {
            SetActiveSelf (false);
        }
    }
    public override void Show () {
        shown = true;
        alpha = 1;
        SetActiveSelf (true);
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup> ();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        onShown.Invoke ();
    }
    public override void Hide () {
        shown = false;
        alpha = 0;
        if (canvasGroup != null && canvasGroup.alpha != 0)
            onHidden.Invoke ();
    }
}
