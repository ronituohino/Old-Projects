using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public GameObject inventoryWindow;
    public Transform windowParent;

    List<InventoryWindow> inventories = new List<InventoryWindow>();

    public Weapon testItem;
    public WeaponData testData;
    public WeaponData testData2;
    public Item testItem2;
        
    private void Awake()
    {
        InventoryWindow inventory = AddInventoryWindow("Pockets", 30);
        inventory.AddItem(new InventoryData.ItemInfo(testItem, testData, new Vector2(0.5f, 0.5f), 1), null);
        inventory.AddItem(new InventoryData.ItemInfo(testItem, testData2, new Vector2(0.2f, 0.5f), 1), null);

        InventoryWindow inventory2 = AddInventoryWindow("Backpack", 120);
        inventory2.AddItem(new InventoryData.ItemInfo(testItem2, null, new Vector2(0.5f, 0.5f), 1), null);
    }

    public InventoryWindow AddInventoryWindow(string name, int maxCapacity)
    {
        GameObject g = Instantiate(inventoryWindow, windowParent);
        g.name = name;

        InventoryWindow i = g.GetComponent<InventoryWindow>();
        i.topText.text = name;
        i.maxCapacity = maxCapacity;

        inventories.Add(i);
        g.SetActive(true);
        return i;
    }
}