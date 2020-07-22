using UnityEngine;

//Panel close button
public class WindowCloseButton : MonoBehaviour, IHoverable, IClickable
{
    [SerializeField] Window window;

    bool IClickable.disabledClick { get => disabledClick; set => disabledClick = value; }
    bool disabledClick = false;

    void IClickable.Click()
    {
        ControlPanelManager.Instance.ToggleWindow(window.associatedTab);
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
    
