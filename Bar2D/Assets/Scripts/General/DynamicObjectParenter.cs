using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectParenter : MonoBehaviour
{
    [System.Serializable]
    public class ParentType
    {
        public Transform parent;
        public LayerMask layers;
    }

    public List<ParentType> parentTypes = new List<ParentType>();

    // Parent objects again when changing ships
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (ParentType pt in parentTypes)
        {
            // This is sooo cool!
            if((pt.layers.value & 1 << collision.gameObject.layer) != 0)
            {
                if(!collision.transform.IsChildOf(pt.parent))
                {
                    collision.transform.parent = pt.parent;
                }
            }
        }
    }
}