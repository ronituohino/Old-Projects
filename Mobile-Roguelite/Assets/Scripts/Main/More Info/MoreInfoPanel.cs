using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreInfoPanel : Interactable
{
    [SerializeField] MoreInfo moreInfo;

    [Space]

    public RectTransform rect;
    public RectTransform contentAnchor;

    public bool opened = false;

    public override void Enter(int fingerId)
    {
        
    }

    public override void Stay(Vector2 delta)
    {
        
    }

    public override void Interrupt()
    {

    }

    public override void Complete()
    {
        if(opened)
        {
            moreInfo.HideMoreInfo(this);
        }
    }
}
