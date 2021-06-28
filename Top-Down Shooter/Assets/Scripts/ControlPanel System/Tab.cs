using System.Collections.Generic;
using UnityEngine;

//Side tab, when clicked, opens a window
public class Tab : MonoBehaviour, IHoverable, ILeftClickable
{
    public Window windowToOpen;

    void ILeftClickable.OnClickPress()
    {
        windowToOpen.ToggleWindow();
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

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}