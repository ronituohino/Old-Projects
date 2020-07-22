using System.Collections.Generic;
using UnityEngine;

//Side tab, when clicked, opens a window
public class Tab : MonoBehaviour, IHoverable, IClickable
{
    bool IClickable.disabledClick { get => disabledClick; set => disabledClick = value; }
    bool disabledClick = false;

    //If length is 1, single tab
    //If length is >1, make them into sub-tabs
    public Window windowToOpen;
    public List<WindowSettings> windowSettings;

    //Each tab has a list of settings corresponding to the amount of pages
    [System.Serializable]
    public struct WindowSettings
    {
        public bool enabled;
        public Vector2 location;
        public Vector2 scale;

        public WindowSettings(bool enabled, Vector2 location, Vector2 scale)
        {
            this.enabled = enabled;
            this.location = location;
            this.scale = scale;
        }
    }

    public bool hasSubTabs = false;
    public List<Tab> subTabs;

    void IClickable.Click()
    {
        if(!hasSubTabs)
        {
            //No sub-tabs
            ControlPanelManager.Instance.ToggleWindow(this);
        }
        else 
        {
            //Open sub-tabs
        }
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