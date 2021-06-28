using System.Collections;
using UnityEngine;

public class InventoryArea : MonoBehaviour,  IHoverable, ILeftClickable
{
    [SerializeField] InventoryWindow window;

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        //We are picking up item
        if (ControlPanel.Instance.carriedItem == null)
        {
            Vector2 point = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                                                (
                                                    window.inventoryParent,
                                                    InputManager.Instance.mouseInScreen, 
                                                    CameraScript.Instance.cam, 
                                                    out point
                                                );

            ItemComponent carriedItem = window.GetClosestItemInPosition(point);

            if(carriedItem != null)
            {
                ControlPanel.Instance.carriedItem = carriedItem;

                window.RemoveItem(carriedItem.info.pos);
                carriedItem.rect.parent = ControlPanel.Instance.transform;
            }
        }
        //We are putting item down
        else
        {
            Vector2 point = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                                                (
                                                    window.inventoryParent,
                                                    InputManager.Instance.mouseInScreen,
                                                    CameraScript.Instance.cam,
                                                    out point
                                                );

            ControlPanel.Instance.carriedItem.info.pos = window.GetRelativePosition(point);

            window.AddItem(ControlPanel.Instance.carriedItem.info, ControlPanel.Instance.carriedItem);

            ControlPanel.Instance.carriedItem.rect.parent = window.inventoryParent;
            ControlPanel.Instance.carriedItem = null;
        }
    }

    void ILeftClickable.OnClickRelease()
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