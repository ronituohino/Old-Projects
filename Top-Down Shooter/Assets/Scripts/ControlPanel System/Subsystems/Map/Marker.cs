using UnityEngine;

public class Marker : MonoBehaviour, IHoverable, ILeftClickable, IRightClickable
{
    public string markerName;

    public RectTransform Rect { get; set; }
    public int Index { get; set; }

    public MapWindow MapWindow { get; set; }

    void ILeftClickable.OnClickHold()
    {
        
    }

    void IRightClickable.OnClickHold()
    {
        
    }



    void ILeftClickable.OnClickPress()
    {
        print("l_p");
    }

    void IRightClickable.OnClickPress()
    {
        MapWindow.SetHeading(this);
    }



    void ILeftClickable.OnClickRelease()
    {
        
    }

    void IRightClickable.OnClickRelease()
    {
        
    }



    void IHoverable.OnHoverEnter()
    {
       
    }

    void IHoverable.OnHoverExit()
    {
        
    }

    void IHoverable.OnHoverStay()
    {
        
    }
}
