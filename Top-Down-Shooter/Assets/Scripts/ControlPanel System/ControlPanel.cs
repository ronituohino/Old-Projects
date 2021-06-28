using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Handles the control panel, tabs, pages
public class ControlPanel : Singleton<ControlPanel>
{
    public bool controlPanelOpen { get; set; } = false;

    [Space]

    public RectTransform controlPanelRect;
    [SerializeField] CanvasGroup controlPanelGroup;

    public RectTransform windowsParent;

    [Space]

    public GameObject itemSlot;
    public GameObject item;

    public float slotSize;

    public Sprite defaultSlotSprite;
    public Sprite litSlotSprite;

    //Item movement
    public ItemComponent carriedItem { get; set; } = null;
    public ItemSlot previousItemSlot { get; set; }


    private void Awake()
    {
        controlPanelGroup.alpha = 0f;
    }

    private void Update()
    {
        //Interacting with the control panel
        if (controlPanelOpen)
        {
            //We are carrying an item
            if (carriedItem != null)
            {
                Vector2 point = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlPanel.Instance.controlPanelRect, Input.mousePosition.ToVector2(), CameraScript.Instance.cam, out point);
                carriedItem.rect.anchoredPosition = point;
            }
        }

        //Search for containers around the player
        else
        {

        }
    }

    //Turns inventory window on/off
    public void ToggleControlPanel()
    {
        controlPanelOpen = !controlPanelOpen;
        if (controlPanelOpen)
        {
            controlPanelGroup.alpha = 1;
        }
        else
        {
            controlPanelGroup.alpha = 0;
            SetItemBack();
        }

        Cursor.visible = controlPanelOpen;
    }

    public void SetItemBack()
    {
        if(carriedItem != null)
        {
            previousItemSlot.SetItem(carriedItem);
        }
    }
}