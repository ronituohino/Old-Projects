using UnityEngine;

class EquipmentWindow : Window
{
    public ItemSlot[] itemSlots;

    int len;

    void Awake()
    {
        len = itemSlots.Length;
        InputManager.Instance.controlledPlayer.equippedItems = new InventoryData.ItemInfo[len];
    }

    void Update()
    {
        if(ControlPanel.Instance.controlPanelOpen)
        {
            InventoryData.ItemInfo[] items = new InventoryData.ItemInfo[len];
            for (int i = 0; i < len; i++)
            {
                ItemSlot slot = itemSlots[i];
                if(slot.storedItem != null)
                {
                    //items[i] = slot.storedItem.info;
                } else
                {
                    //items[i] = null;
                }
            }

            InputManager.Instance.controlledPlayer.equippedItems = items;
        }
    }
}