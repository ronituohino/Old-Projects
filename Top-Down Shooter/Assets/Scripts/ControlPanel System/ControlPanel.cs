using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

//Handle player interaction with the controlpanel
public class ControlPanel : Singleton<ControlPanel>
{
    public Camera cam;

    public KeyCode controlPanelKey;

    [HideInInspector]
    public bool controlPanelOpen = false;

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

    List<IHoverable> hoverableElements = new List<IHoverable>();
    List<IHoverable> previousElements = new List<IHoverable>();
    IDraggable draggableElement;
    IClickable clickableElement;
    MonoBehaviour clickableAndDraggable;

    //Clicking
    [SerializeField] float pressMouseMovementThreshold;
    Vector2 origianlMousePosition;
    bool dragging = false;

    public float mouseScroll;

    //Item movement
    public ItemComponent carriedItem = null;
    public ItemSlot previousItemSlot;

    private void Update()
    {
        if (Input.GetKeyDown(controlPanelKey))
        {
            ToggleControlPanel();
        }

        //Interacting with the control panel
        if (controlPanelOpen)
        {
            List<RaycastResult> results = RaycastPosition(Input.mousePosition);

            //Hover effects
            //Copy previous elements
            previousElements.Clear();
            foreach (IHoverable h in hoverableElements)
            {
                previousElements.Add(h);
            }

            //Read new elements
            hoverableElements.Clear();
            foreach (RaycastResult rr in results)
            {
                MonoBehaviour[] behaviours = rr.gameObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour mb in behaviours)
                {
                    if (mb is IHoverable)
                    {
                        IHoverable hoverable = (IHoverable)mb;
                        hoverableElements.Add(hoverable);
                    }
                }
            }

            //Compare if there are any that were removed
            foreach (IHoverable h in previousElements)
            {
                if (!hoverableElements.Contains(h))
                {
                    h.OnHoverExit();
                }
            }

            //Compare if there are any that were added
            foreach (IHoverable h in hoverableElements)
            {
                h.OnHover();
                if (!previousElements.Contains(h))
                {
                    h.OnHoverEnter();
                }
            }



            //Initial mouse press, raycast and check if there is UI elements under
            if (Input.GetMouseButtonDown(0))
            {
                origianlMousePosition = Input.mousePosition.ToVector2();

                bool foundElement = false;
                foreach (RaycastResult rr in results)
                {
                    //Check for any UI elements under mouse
                    MonoBehaviour[] behaviours = rr.gameObject.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour mb in behaviours)
                    {
                        //Handle elements that are both clickable and draggable
                        if (mb is IClickable && mb is IDraggable)
                        {
                            clickableAndDraggable = mb;
                        } 

                        if(mb is IClickable)
                        {
                            IClickable clickable = (IClickable)mb;
                            if (!clickable.disabledClick)
                            {
                                clickableElement = clickable;
                                foundElement = true;
                            }
                        } 
                        if (mb is IDraggable)
                        {
                            draggableElement = (IDraggable)mb;
                            draggableElement.OnPress();

                            foundElement = true;
                        }

                        if (foundElement)
                        {
                            break;
                        }
                    }

                    if(foundElement)
                    {
                        break;
                    }
                }
            }

            //Holding mouse down, dragging code
            if (Input.GetMouseButton(0))
            {
                Vector2 mouseMovement = Input.mousePosition.ToVector2() - origianlMousePosition;

                if (Mathf.Abs(mouseMovement.x) > pressMouseMovementThreshold || Mathf.Abs(mouseMovement.y) > pressMouseMovementThreshold)
                {
                    dragging = true;
                }

                if (dragging && draggableElement != null)
                {
                    draggableElement.OnDrag();

                    //If an element that is both clickable and draggable is dragged, cancel click
                    if(clickableAndDraggable != null)
                    {
                        clickableAndDraggable = null;
                        clickableElement = null;
                    }
                }
            }

            //Mouse release, check for clicks, complete drag thingies, cleanup variables
            if (Input.GetMouseButtonUp(0))
            {
                if (draggableElement != null) //Drag
                {
                    draggableElement.OnRelease();
                }

                if (clickableElement != null) //Click
                {
                    foreach(IHoverable h in hoverableElements)
                    {
                        if(h is IClickable)
                        {
                            if(h == clickableElement)
                            {
                                clickableElement.Click();
                            }
                        }
                    }
                }

                ResetVariables();
            }

            //We are carrying an item
            if (carriedItem != null)
            {
                Vector2 point = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlPanel.Instance.controlPanelRect, Input.mousePosition.ToVector2(), ControlPanel.Instance.cam, out point);
                carriedItem.rect.anchoredPosition = point;
            }

            mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        }

        //Search for containers around the player
        else
        {

        }
    }

    //Turns inventory window on/off
    void ToggleControlPanel()
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

    public List<RaycastResult> RaycastPosition(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    void ResetVariables()
    {
        draggableElement = null;
        dragging = false;
    }

    public void SetItemBack()
    {
        if(carriedItem != null)
        {
            previousItemSlot.SetItem(carriedItem);
        }
    }
}