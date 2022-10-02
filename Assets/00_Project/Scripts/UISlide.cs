using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISlide : UITransition
{
    [HideInInspector] public RectTransform rectTransform;
    [Header ("Slide Custom Properties")]
    [Space (20)]
    public Vector2 showedPosition;
    public Vector2 hiddenPosition;
    public Vector2 showedPortraitPosition;
    public Vector2 hiddenPortraitPosition;
    private Vector2 showedLandscapePosition, hiddenLandscapePosition;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private bool moving = false;
    private float timer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        showedLandscapePosition = showedPosition;
        hiddenLandscapePosition = hiddenPosition;
        shown = rectTransform.anchoredPosition == showedPosition;
        SetActiveSelf (shown);
    }

    void Update()
    {
        if (moving)
        {
            timer += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(
                shown ? hiddenPosition : showedPosition,
                shown ? showedPosition : hiddenPosition,
                animationCurve.Evaluate(timer / duration)
                );
            if (timer >= duration)
            {
                moving = false;
                rectTransform.anchoredPosition = shown ? showedPosition : hiddenPosition;
                if (!shown)
                    SetActiveSelf(shown);
            }
        }
    }
    public override void Show()
    {
        if (shown) return;
        SetActiveSelf(true);
        rectTransform.anchoredPosition = hiddenPosition;
        // shown += gameObject.tag != "HudScreen" ? 1 : 0;
        shown = true;
        timer = 0;
        moving = true;
        onShown.Invoke();
    }

    public override void Hide()
    {
        if (!shown) return;
        foreach (UISlide item in GetComponentsInChildren<UISlide>())
        {
            if (item != this) item.Hide();
        }
        // shown -= gameObject.tag != "HudScreen" ? 1 : 0;
        shown = false;
        timer = 0;
        moving = true;
        onHidden.Invoke();
    }

    public void UpdateOrientation () {
        if (Screen.orientation == ScreenOrientation.Portrait && Application.isMobilePlatform) {
            rectTransform.anchoredPosition = shown ? showedPortraitPosition : hiddenPortraitPosition;
            showedPosition = showedPortraitPosition;
            hiddenPosition = hiddenPortraitPosition;
        } else {
            rectTransform.anchoredPosition = shown ? showedLandscapePosition : hiddenLandscapePosition;
            showedPosition = showedLandscapePosition;
            hiddenPosition = hiddenLandscapePosition;
        }
    }
}
