using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, ILeftClickable, IHoverable
{
    public UnityEvent clickedEvent;
    public UnityEvent hoverEnterEvent;
    public UnityEvent hoverExitEvent;

    void ILeftClickable.OnClickHold() { }

    void ILeftClickable.OnClickPress() { }

    void ILeftClickable.OnClickRelease()
    {
        clickedEvent.Invoke();
    }



    void IHoverable.OnHoverEnter()
    {
        hoverEnterEvent.Invoke();
    }

    void IHoverable.OnHoverExit()
    {
        hoverExitEvent.Invoke();
    }

    void IHoverable.OnHoverStay() { }
}