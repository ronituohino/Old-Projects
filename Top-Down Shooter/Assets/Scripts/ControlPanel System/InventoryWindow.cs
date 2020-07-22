using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//UI inventory
[System.Serializable]
public class InventoryWindow : Window, IHoverable
{
    public InventoryData inventoryData;
    int previousItemCount;

    //UI
    [Space]
    List<ItemSlot> itemSlots = new List<ItemSlot>();

    public RectTransform inventoryScroll;

    [Space]

    [SerializeField] RectTransform slotParent;

    [Space]

    public RectTransform scrollKnob;
    [SerializeField] TextMeshProUGUI capacityText;

    [Space]

    [SerializeField] RectTransform capacityFill;
    [SerializeField] RectTransform capacityFillSlider;

    [HideInInspector]
    public bool folded = false;



    public int maxCapacity = 0;
    int capacityTaken = 0;

    [HideInInspector]
    public int ySlots = 5;
    [HideInInspector]
    public int xSlots = 3;

    [SerializeField] float scrollSpeedMultiplier;
    [SerializeField] float scrollDampening;

    Vector2 previousSize = Vector2.zero;


    private void Awake()
    {
        previousSize = windowTransform.sizeDelta;
        inventoryData = new InventoryData();

        SetScrollKnob(-1);

        CreateSlots();
        UpdateCapacity();

        associatedTab = ControlPanelManager.Instance.AddTabSingle(this);
    }

    //Detects window scaling
    private void Update()
    {
        //Create new slots if the window is resized
        if (ControlPanel.Instance.controlPanelOpen)
        {
            //The window has been scaled
            if (windowTransform.sizeDelta != previousSize)
            {
                //Make new sizes
                int previousX = xSlots;
                int previousY = ySlots;

                xSlots = Mathf.RoundToInt(windowTransform.sizeDelta.x / ControlPanel.Instance.slotSize);
                ySlots = Mathf.RoundToInt((windowTransform.sizeDelta.y - 35) / ControlPanel.Instance.slotSize) + 2;

                //Set scroll knob position
                float maxY = (ySlots - Mathf.FloorToInt(windowTransform.sizeDelta.y / ControlPanel.Instance.slotSize)) * ControlPanel.Instance.slotSize;
                scrollKnob.anchoredPosition = new Vector2(0, -Extensions.Remap(inventoryScroll.anchoredPosition.y, 0, maxY, -1, 1) * ((windowTransform.sizeDelta.y - 35 - scrollKnob.sizeDelta.y) / 2f));

                List<ItemComponent> itemComponents = new List<ItemComponent>();

                //Reposition items
                foreach (ItemSlot slot in itemSlots)
                {
                    ItemComponent ic = slot.storedItem;

                    if (ic != null)
                    {
                        itemComponents.Add(ic);
                        ic.transform.SetParent(slotParent);

                        Vector2Int coords = Extensions.GetCoordsFromIndex(slot.slotIndex, previousX, previousY);

                        Vector2Int fixedCoords = coords;
                        if (fixedCoords.x >= xSlots)
                        {
                            fixedCoords.x = xSlots - 1;
                        }
                        if (fixedCoords.y >= ySlots - 2)
                        {
                            fixedCoords.y = ySlots - 3;
                        }

                        int index = Extensions.GetIndexFromCoords(fixedCoords.x, fixedCoords.y, xSlots);

                        if (fixedCoords == coords)
                        {
                            //ic.rect.anchoredPosition = GetObjectPositionInInventory(fixedCoords.x, fixedCoords.y);
                            ic.info.index = index;
                        }
                        else
                        {
                            if (!inventoryData.Occupied(index))
                            {
                                //ic.rect.anchoredPosition = GetObjectPositionInInventory(fixedCoords.x, fixedCoords.y);
                                ic.info.index = index;
                            }
                            else
                            {
                                int newIndex = inventoryData.GetFreePositionInContainer();
                                //ic.rect.anchoredPosition = GetObjectPositionInInventoryIndex(newIndex);
                                ic.info.index = newIndex;
                            }
                        }
                    }
                }

                //Destroy old slots
                int count = itemSlots.Count;
                for (int i = 0; i < count; i++)
                {
                    Destroy(itemSlots[i].gameObject);
                }
                itemSlots.Clear();

                //Make new ones
                CreateSlots(itemComponents);
                UpdateCapacity();
            }

            int itemCount = inventoryData.storedItems.Count;
            //Some item was taken out or put in
            if (itemCount != previousItemCount)
            {
                UpdateCapacity();
            }

            previousSize = windowTransform.sizeDelta;
            previousItemCount = inventoryData.storedItems.Count;
        }
    }

    //Creates slot objects in the window, give itemComponents if there were items stored
    void CreateSlots(List<ItemComponent> itemComponents = null)
    {
        //Slots (make automatic sizing)
        for (int y = 0; y < ySlots; y++)
        {
            for (int x = 0; x < xSlots; x++)
            {
                GameObject g = GameObject.Instantiate(ControlPanel.Instance.itemSlot, slotParent);

                ItemSlot itemSlot = g.AddComponent<ItemSlot>();
                itemSlot.inventoryData = inventoryData;
                itemSlot.slotIndex = y * xSlots + x;

                g.name = "Slot";
                itemSlots.Add(itemSlot);

                if (itemComponents != null)
                {
                    foreach (ItemComponent ic in itemComponents)
                    {
                        if (ic.info.index == itemSlot.slotIndex)
                        {
                            itemSlot.storedItem = ic;
                            ic.transform.SetParent(g.transform);
                            ic.transform.localPosition = new Vector3(0, 0, 0);

                            break;
                        }
                    }
                }

                itemSlot.rc.anchoredPosition = GetObjectPositionInInventory(x, y);
            }
        }
    }

    Vector2 GetObjectPositionInInventory(int x, int y)
    {
        return new Vector2(-(windowTransform.sizeDelta.x / 2f - ControlPanel.Instance.slotSize / 2f) + x * ControlPanel.Instance.slotSize,
                          ((windowTransform.sizeDelta.y - 35) / 2f - ControlPanel.Instance.slotSize / 2f) - y * ControlPanel.Instance.slotSize);
    }

    //Calculates the rect middle coordinates given the 0,0 position and size
    public Vector2 GetObjectPositionInInventoryIndex(int index)
    {
        Vector2Int coords = Extensions.GetCoordsFromIndex(index, xSlots, ySlots);
        return GetObjectPositionInInventory(coords.x, coords.y);
    }

    //Updates the capacity stuff
    public void UpdateCapacity()
    {
        int newCapacity = 0;
        foreach (ItemInfo info in inventoryData.storedItems)
        {
            newCapacity += info.item.inventorySpace;
        }
        capacityTaken = newCapacity;

        capacityText.text = capacityTaken.ToString() + "/" + maxCapacity.ToString();
        capacityFillSlider.offsetMax = new Vector2(-capacityFill.rect.width * Extensions.Remap(capacityTaken, 0, maxCapacity, 1, 0), 0); //new Vector2(-right, -top)
    }





    //Creates a new UI item object and adds it to inventory
    public bool AddItem(Item item, ItemData itemData, int index, bool createGameObject) //Position is the 0,0 square position in container grid
    {
        ItemInfo info = inventoryData.AddItem(item, itemData, index);
        if (info != null)
        {
            if (createGameObject)
            {
                ItemSlot slot = itemSlots[index];

                GameObject g = Instantiate(ControlPanel.Instance.item, slot.rc);
                g.transform.localPosition = new Vector3(0, 0, 0);

                ItemComponent ic = g.GetComponent<ItemComponent>();
                RectTransform rect = g.GetComponent<RectTransform>();

                ic.info = info;
                slot.storedItem = ic;

                rect.sizeDelta = new Vector2(ControlPanel.Instance.slotSize, ControlPanel.Instance.slotSize);
                rect.anchoredPosition = new Vector2(0, 0);

                Image image = g.GetComponent<Image>();
                image.sprite = item.inventoryImage;
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(int index)
    {
        inventoryData.RemoveItem(index);
    }

    public void ClearItems()
    {
        int count = itemSlots.Count;
        for (int i = 0; i < count; i++)
        {
            ItemComponent ic = itemSlots[i].storedItem;
            if (ic != null)
            {
                GameObject g = ic.gameObject;
                Destroy(g);
            }
        }

        inventoryData.ClearItems();
    }





    void IHoverable.OnHoverEnter() { }

    void IHoverable.OnHover()
    {
        if (ControlPanel.Instance.mouseScroll != 0f)
        {
            //Set slots position
            Vector2 newPos = new Vector2(inventoryScroll.anchoredPosition.x, inventoryScroll.anchoredPosition.y - ControlPanel.Instance.mouseScroll * scrollSpeedMultiplier);
            Vector2 vel = Vector2.zero;
            inventoryScroll.anchoredPosition = Vector2.SmoothDamp(inventoryScroll.anchoredPosition, newPos, ref vel, scrollDampening);

            float maxY = (ySlots - Mathf.FloorToInt(windowTransform.sizeDelta.y / ControlPanel.Instance.slotSize)) * ControlPanel.Instance.slotSize;

            if (inventoryScroll.anchoredPosition.y < 0)
            {
                inventoryScroll.anchoredPosition = new Vector2(inventoryScroll.anchoredPosition.x, 0);
            }
            else if (inventoryScroll.anchoredPosition.y > maxY)
            {
                inventoryScroll.anchoredPosition = new Vector2(inventoryScroll.anchoredPosition.x, maxY);
            }

            //Set knob position
            float scrollPercent = Extensions.Remap(inventoryScroll.anchoredPosition.y, 0, maxY, -1, 1);
            //35 is capacity width
            SetScrollKnob(scrollPercent);
        }
    }

    void SetScrollKnob(float minusOneToOne)
    {
        scrollKnob.anchoredPosition = new Vector2(0, -minusOneToOne * ((windowTransform.sizeDelta.y - 35 - scrollKnob.sizeDelta.y) / 2f));
    }

    void IHoverable.OnHoverExit() { }
}