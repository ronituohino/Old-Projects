using UnityEngine;

class EquipmentWindow : Window
{
    public PlayerController playerController;
    public ItemSlot[] itemSlots;

    int len;

    void Awake()
    {
        associatedTab = ControlPanelManager.Instance.AddTabSingle(this);

        len = itemSlots.Length;
        playerController.equippedItems = new ItemInfo[len];
    }

    void Update()
    {
        if(ControlPanel.Instance.controlPanelOpen)
        {
            ItemInfo[] items = new ItemInfo[len];
            for (int i = 0; i < len; i++)
            {
                ItemSlot slot = itemSlots[i];
                if(slot.storedItem != null)
                {
                    items[i] = slot.storedItem.info;
                } else
                {
                    items[i] = null;
                }
            }

            playerController.equippedItems = items;
        }
    }
}