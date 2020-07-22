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
        inventory.AddItem(testItem, testData, 1, true);
        inventory.AddItem(testItem, testData2, 2, true);

        InventoryWindow inventory2 = AddInventoryWindow("Backpack", 120);
        inventory2.AddItem(testItem2, null, 0, true);
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