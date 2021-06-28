using System.Collections;
using UnityEngine;

public class SwitchPage : MonoBehaviour, ILeftClickable
{
    [SerializeField] BartendingBook bb;
    [SerializeField] bool switchPageLeft;

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        if(switchPageLeft)
        {
            bb.SwitchPageLeft();
        }
        else
        {
            bb.SwitchPageRight();
        }
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}