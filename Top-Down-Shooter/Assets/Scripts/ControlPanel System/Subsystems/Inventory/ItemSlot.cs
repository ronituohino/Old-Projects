using System;
using System.Linq;
using UnityEngine;

//Attached to every slot that an item can be placed in, talks with inventoryData
public class ItemSlot : MonoBehaviour, IHoverable, ILeftClickable
{
    [HideInInspector]
    public RectTransform rc;

    //Reference to the inventory this itemSlot is connected to, could totally be an entirely new inventory also!
    public InventoryData inventoryData;
    public int slotIndex;

    public ItemComponent storedItem;

    void Awake()
    {
        rc = GetComponent<RectTransform>();
        rc.sizeDelta = new Vector2(ControlPanel.Instance.slotSize, ControlPanel.Instance.slotSize);
    }

    void ILeftClickable.OnClickPress()
    {
        //We are setting item here
        if (ControlPanel.Instance.carriedItem != null && storedItem == null)
        {
            SetItem(ControlPanel.Instance.carriedItem);
        }
        //We are placing item to a taken slot, combine or switch
        else if (ControlPanel.Instance.carriedItem != null && storedItem != null)
        {

        }
        //We are picking item up from here
        else if (ControlPanel.Instance.carriedItem == null && storedItem != null)
        {
            ControlPanel.Instance.carriedItem = storedItem;
            ControlPanel.Instance.previousItemSlot = this;

            storedItem.rect.SetParent(ControlPanel.Instance.windowsParent);

            if (inventoryData != null)
            {
                //inventoryData.RemoveItem(slotIndex);
            }

            storedItem = null;
        }
    }

    public void SetItem(ItemComponent itemComponent)
    {
        //Set item here
        //InventoryData<int>.ItemInfo info = inventoryData.AddItem(itemComponent.info.item, itemComponent.info.itemData, slotIndex, itemComponent.info.amount);

        storedItem = itemComponent;

        ControlPanel.Instance.carriedItem = null;
        ControlPanel.Instance.previousItemSlot = null;

        itemComponent.rect.SetParent(rc);
        itemComponent.transform.localPosition = new Vector3(0, 0, 0);
    }

    void IHoverable.OnHoverStay()
    {

    }

    void IHoverable.OnHoverEnter()
    {

    }

    void IHoverable.OnHoverExit()
    {

    }

    void ILeftClickable.OnClickHold()
    {

    }

    void ILeftClickable.OnClickRelease()
    {

    }
}