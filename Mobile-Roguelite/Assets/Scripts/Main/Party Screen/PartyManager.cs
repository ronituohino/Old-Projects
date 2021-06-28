using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : Singleton<PartyManager>
{
    public bool opened = false;

    [Space]

    [SerializeField] RectTransform partyRect;

    [Space]

    [SerializeField] List<ItemContainer> inventorySlots;

    [Space]

    public List<Hero> heroes;

    public void Open()
    {
        opened = true;
        inventorySlots.ForEach(x => x.Visible());
        heroes.ForEach(x => x.itemContainer.Visible());

        heroes.ForEach(x => x.UpdateHero());
    }

    public void Close()
    {
        opened = false;
        inventorySlots.ForEach(x => x.Hidden());
        heroes.ForEach(x => x.itemContainer.Hidden());
    }
}
