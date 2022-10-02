using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopup : UITransition
{
    [Header ("PopUp Custom Properties")]
    [Space (20)]
    public Color backgroundColor;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut (0, 0, 1, 1);

    private RectTransform popup;
    private Image overlay;
    private bool moving = false;
    private float timer = 0;
    private Vector2 hiddenPosition;
    private void Awake () {
        Init ();
    }
    private void Init () {
        overlay = GetComponent<Image> ();
        overlay.color = Color.clear;

        popup = transform.GetChild (0).transform as RectTransform;
        hiddenPosition = popup.sizeDelta.y * Vector2.down;
        popup.anchoredPosition = Vector2.zero;

        SetActiveSelf (shown);
    }
    private void Update () {
        if (moving) {
            timer += Time.deltaTime;
            popup.anchoredPosition = Vector2.Lerp (
                shown ? hiddenPosition : Vector2.zero,
                shown ? Vector2.zero : hiddenPosition,
                animationCurve.Evaluate (timer / duration)
                );
            overlay.color = Color.Lerp (
                shown ? Color.clear : backgroundColor,
                shown ? backgroundColor : Color.clear,
                animationCurve.Evaluate (timer / duration)
                );
            if (timer >= duration) {
                moving = false;
                popup.anchoredPosition = shown ? Vector2.zero : hiddenPosition;
                overlay.color = shown ? backgroundColor : Color.clear;
                if (!shown)
                    SetActiveSelf (shown);
            }
        }
    }
    public override void Hide () {
        if (!shown) return;
        shown = false;
        timer = 0;
        moving = true;
        onHidden.Invoke ();
    }

    public override void Show () {
        if (shown) return;
        shown = true;
        if (popup == null) {
            Init ();
        }
        popup.anchoredPosition = hiddenPosition;
        overlay.color = Color.clear;
        timer = 0;
        moving = true;
        onShown.Invoke ();
        SetActiveSelf (true);
    }
    public void OnTopBtnDrag (BaseEventData e) {
        PointerEventData pe = e as PointerEventData;
        Vector2 newPosition = popup.anchoredPosition;
        newPosition.y = Mathf.Min(0, newPosition.y + pe.delta.y);
        popup.anchoredPosition = newPosition;
        overlay.color = backgroundColor * (1 - popup.anchoredPosition.y / hiddenPosition.y);
        shown = pe.delta.y > 0;
    }
    public void OnTopBtnUp (BaseEventData e) {
        float hiddenPrec = popup.anchoredPosition.y / hiddenPosition.y;
        //shown = hiddenPrec <= 0.5f;
        if (shown) {
            timer = (1 - hiddenPrec) * duration;
        } else {
            timer = hiddenPrec * duration;
        }
        moving = true;
    }
}
