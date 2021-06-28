using UnityEngine;

//Panel close button
public class WindowCloseButton : MonoBehaviour, IHoverable, ILeftClickable
{
    Window window;

    void Awake()
    {
        window = transform.parent.parent.GetComponent<Window>();
    }

    void IHoverable.OnHoverExit()
    {
        
    }

    void IHoverable.OnHoverEnter()
    {
        
    }

    void IHoverable.OnHoverStay()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        window.ToggleWindow();
    }

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}
    
