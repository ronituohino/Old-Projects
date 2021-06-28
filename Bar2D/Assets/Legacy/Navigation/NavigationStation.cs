using System.Collections;
using UnityEngine;

// Navigation panel on deck that opens NavigationPanel.cs
public class NavigationStation : MonoBehaviour, ILeftClickable
{
    [SerializeField] Navigation panel;

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        
    }

    void ILeftClickable.OnClickRelease()
    {
        if(!panel.opened)
        {
            panel.Open();
        }
    }
}