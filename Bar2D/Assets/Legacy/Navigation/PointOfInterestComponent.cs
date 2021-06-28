using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterestComponent : MonoBehaviour, ILeftClickable, IRightClickable
{
    public PointOfInterest poi;

    public RectTransform rect;
    public Image image;

    [HideInInspector]
    public bool seen = false;

    // Determines whether this point has triggered the event already 
    // (avoids multiple calls to StartEvent in viscinity)
    [HideInInspector]
    public bool eventTriggered = false;

    [HideInInspector]
    public NavigationEvent evnt;

    [HideInInspector]
    public Vector2 position;

    [HideInInspector]
    public Navigation navigation;

    bool hasMarker = false;

    void ILeftClickable.OnClickPress()
    {

    }

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickRelease()
    {
        navigation.CreateMarker(rect.anchoredPosition);
        hasMarker = true;
    }



    void IRightClickable.OnClickPress()
    {

    }

    void IRightClickable.OnClickHold()
    {

    }

    void IRightClickable.OnClickRelease()
    {
        if(hasMarker)
        {
            int index = navigation.markers.FindIndex(x => x.rect.anchoredPosition == rect.anchoredPosition);
            navigation.RemoveMarker(index);

            hasMarker = false;
        }
    }
}