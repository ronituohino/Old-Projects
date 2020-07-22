using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour, IHoverable, IClickable
{
    bool IClickable.disabledClick { get => disabledClick; set => disabledClick = value; }
    bool disabledClick = false;

    void IClickable.Click()
    {
        ControlPanelManager.Instance.SwitchPage(this);
    }

    void IHoverable.OnHoverExit()
    {
        
    }

    void IHoverable.OnHoverEnter()
    {
        
    }

    void IHoverable.OnHover()
    {
        
    }
}

