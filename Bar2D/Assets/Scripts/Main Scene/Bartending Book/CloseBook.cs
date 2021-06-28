using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBook : MonoBehaviour, ILeftClickable
{
    [SerializeField] BartendingBook bb;

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        bb.CloseBook();
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}
