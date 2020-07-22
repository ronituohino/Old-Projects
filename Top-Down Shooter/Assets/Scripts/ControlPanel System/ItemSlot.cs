using System;
using System.Linq;
using UnityEngine;

//Attached to every slot that an item can be placed in, talks with inventoryData
public class ItemSlot : MonoBehaviour, IHoverable, IClickable
{
    bool IClickable.disabledClick { get => disabledClick; set => disabledClick = value; }
    bool disabledClick = false;

    [HideInInspector]
    public RectTransform rc;

    //Reference to the inventory this itemSlot is connected to, could totally be an entirely new inventory also!
    public InventoryData inventoryData;
    public int slotIndex;

    public ItemComponent storedItem;

    //Leave empty to allow all
    public Item.ItemType[] itemsAllowed;

    void Awake()
    {
        rc = GetComponent<RectTransform>();
        rc.sizeDelta = new Vector2(ControlPanel.Instance.slotSize, ControlPanel.Instance.slotSize);
    }

    void IClickable.Click()
    {
        //We are setting item here
        ItemComponent component = ControlPanel.Instance.carriedItem;
        if (component != null)
        {
            SetItem(component);
        } 

        //We are picking item up from here
        else if(storedItem != null) 
        {
            ControlPanel.Instance.carriedItem = storedItem;
            ControlPanel.Instance.previousItemSlot = this;

            storedItem.rect.SetParent(ControlPanel.Instance.windowsParent);

            if(inventoryData != null)
            {
                inventoryData.RemoveItem(slotIndex);
            }
            
            storedItem = null;
        }
    }

    public void SetItem(ItemComponent itemComponent)
    {
        //Set item here
        bool canStore = false;
        if (storedItem == null)
        {
            if (itemsAllowed == null || itemsAllowed.Length == 0 || itemsAllowed.Contains(itemComponent.info.item.itemType))
            {
                if (inventoryData != null)
                {
                    if (inventoryData.AddItem(itemComponent.info, slotIndex))
                    {
                        canStore = true;
                    }
                }
                else
                {
                    canStore = true;
                }
            }
        }

        if (canStore)
        {
            storedItem = itemComponent;

            ControlPanel.Instance.carriedItem = null;
            ControlPanel.Instance.previousItemSlot = null;

            itemComponent.rect.SetParent(rc);
            itemComponent.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    void IHoverable.OnHover()
    {
        
    }

    void IHoverable.OnHoverEnter()
    {
        
    }

    void IHoverable.OnHoverExit()
    {
        
    }
}