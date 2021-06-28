using System.Collections;
using UnityEngine;

public class ReturnToCatalogue : MonoBehaviour, ILeftClickable
{
    [SerializeField] BartendingBook bb;

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        bb.ReturnToCatalogue();
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}
