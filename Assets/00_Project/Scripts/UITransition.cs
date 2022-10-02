using UnityEngine;
using UnityEngine.Events;

public abstract class UITransition : MonoBehaviour
{
    [Header ("Transition Properties")]
    public UnityEvent onShown;
    public UnityEvent onHidden;
    public float duration = 0.3f;
    public bool controlActiveState = true;


    public abstract void Show();
    public abstract void Hide();
    [HideInInspector] 
    public bool shown = false;
    public void Toggle()
    {
        SetVisible(!shown);
    }
    public void SetVisible(bool visible)
    {
        if (visible)
            Show();
        else
            Hide();
    }
    public void SetInvisible (bool invisible) {
        if (invisible)
            Hide ();
        else
            Show ();
    }
    public void SetActiveSelf (bool active)
    {
        if(controlActiveState) gameObject.SetActive(active);
    }
}