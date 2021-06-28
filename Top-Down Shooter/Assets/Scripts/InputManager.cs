using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    public PlayerController controlledPlayer;

    List<IHoverable> hoverableElements = new List<IHoverable>();
    List<IHoverable> previousElements = new List<IHoverable>();

    bool l_clickable = false;
    bool r_clickable = false;
    bool draggable = false;

    ILeftClickable l_clickableElement;
    IRightClickable r_clickableElement;
    IDraggable draggableElement;

    Vector2 originalMousePosition;
    private bool dragging;
    
    [SerializeField] float pressMouseMovementThreshold;

    public Vector2 mouseInScreen { get; private set; }
    public Vector2 mouseInWorld { get; private set; }

    public float mouseScroll { get; private set; } = 0f;

    bool ignoringInput = false;
    public bool IgnoreAllInput
    { 
        get => ignoringInput;
        set
        {
            ignoringInput = value;
            print($"Ignoring input: {value}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ignoringInput)
        {
            return;
        }

        mouseInScreen = Input.mousePosition;
        mouseInWorld = CameraScript.Instance.cam.ScreenToWorldPoint(mouseInScreen);


        //Movement
        Vector2 movement = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            movement += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += new Vector2(0, -1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += new Vector2(1, 0);
        }
        movement = movement.normalized;
        controlledPlayer.MovePlayer(movement);


        //Control panel
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ControlPanel.Instance.ToggleControlPanel();
        }


        //Equipment
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlledPlayer.EquipItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlledPlayer.EquipItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlledPlayer.EquipItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controlledPlayer.EquipItem(3);
        }


        //Debug
        if(Input.GetKeyDown(KeyCode.H))
        {
            //WorldMapGenerator.Instance.GenerateMap();
        }

        #region Mouse

        mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (ControlPanel.Instance.controlPanelOpen)
        {
            //Collider objects
            RaycastHit2D[] hoveredObjects = Physics2D.RaycastAll(mouseInWorld, Vector2.zero);
            int hoveredObjectsCount = hoveredObjects.Length;

            //UI
            List<RaycastResult> hoveredUiObjects = RaycastPosition(mouseInScreen);
            int hoveredUiObjectsCount = hoveredUiObjects.Count;

            //All behaviours collectively
            List<MonoBehaviour> behaviours = new List<MonoBehaviour>();

            //UI objects are handled first
            for (int i = 0; i < hoveredUiObjectsCount; i++)
            {
                MonoBehaviour[] attachedBehaviours = hoveredUiObjects[i].gameObject.GetComponents<MonoBehaviour>();
                behaviours.AddRange(attachedBehaviours);
            }

            for (int i = 0; i < hoveredObjectsCount; i++)
            {
                MonoBehaviour[] attachedBehaviours = hoveredObjects[i].collider.gameObject.GetComponents<MonoBehaviour>();
                behaviours.AddRange(attachedBehaviours);
            }

            //Handle IHoverables
            //Copy previous elements
            previousElements.Clear();
            previousElements.AddRange(hoverableElements);

            //Read new elements
            hoverableElements.Clear();
            foreach (MonoBehaviour mb in behaviours)
            {
                if (mb is IHoverable)
                {
                    IHoverable hoverable = (IHoverable)mb;
                    hoverableElements.Add(hoverable);
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
                h.OnHoverStay();
                if (!previousElements.Contains(h))
                {
                    h.OnHoverEnter();
                }
            }

            //Initial click
            bool left = Input.GetMouseButtonDown(0);
            bool right = Input.GetMouseButtonDown(1);

            if (left || right)
            {
                originalMousePosition = mouseInScreen;

                bool foundObject = false;
                foreach (MonoBehaviour mb in behaviours)
                {
                    l_clickable = mb is ILeftClickable;
                    r_clickable = mb is IRightClickable;

                    draggable = mb is IDraggable;

                    if (l_clickable && left)
                    {
                        ILeftClickable c = (ILeftClickable)mb;
                        l_clickableElement = c;
                        c.OnClickPress();

                        foundObject = true;
                    }
                    if (r_clickable && right)
                    {
                        IRightClickable c = (IRightClickable)mb;
                        r_clickableElement = c;
                        c.OnClickPress();

                        foundObject = true;
                    }
                    if (draggable && left && !right)
                    {
                        IDraggable d = (IDraggable)mb;
                        draggableElement = d;
                        d.OnDragPress();

                        foundObject = true;
                    }

                    if (foundObject)
                    {
                        break;
                    }
                }
            }

            left = Input.GetMouseButton(0);
            right = Input.GetMouseButton(1);

            //Hold
            if (left || right)
            {
                if(right && r_clickable)
                {
                    r_clickableElement.OnClickHold();
                } 
                else
                {
                    Vector2 mouseMovement = mouseInScreen - originalMousePosition;

                    if (left && Mathf.Abs(mouseMovement.x) > pressMouseMovementThreshold || Mathf.Abs(mouseMovement.y) > pressMouseMovementThreshold)
                    {
                        dragging = true;
                    }

                    if (dragging)
                    {
                        if (draggable)
                        {
                            draggableElement.OnDrag(mouseMovement);
                        }
                    }
                    else
                    {
                        if (l_clickable)
                        {
                            l_clickableElement.OnClickHold();
                        }
                    }
                }
            }

            left = Input.GetMouseButtonUp(0);
            right = Input.GetMouseButtonUp(1);

            //Release
            if (left || right)
            {
                if(r_clickable && right)
                {
                    r_clickableElement.OnClickRelease();
                }
                if (draggable && left)
                {
                    draggableElement.OnDragRelease();
                }
                if (l_clickable && left && !dragging)
                {
                    l_clickableElement.OnClickRelease();
                }

                ResetVariables();
            }
        } 
        else
        {
            if(Input.GetMouseButton(0))
            {
                controlledPlayer.FireWeapon();
            }
            
            controlledPlayer.focusing = Input.GetMouseButton(1); 
        }
        
        #endregion
    }

    void ResetVariables()
    {
        l_clickable = false;
        r_clickable = false;

        draggable = false;
        l_clickableElement = null;
        r_clickableElement = null;
        draggableElement = null;
        originalMousePosition = Vector2.zero;
        dragging = false;
    }

    //UI
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


    public void InterceptDrag()
    {
        draggableElement.OnDragRelease();
        ResetVariables();
    }
}

interface ILeftClickable
{
    void OnClickPress();
    void OnClickHold();
    void OnClickRelease();
}

interface IRightClickable
{
    void OnClickPress();
    void OnClickHold();
    void OnClickRelease();
}

interface IDraggable
{
    void OnDragPress();
    void OnDrag(Vector2 totalMovement);
    void OnDragRelease();
}

interface IHoverable
{
    void OnHoverEnter();
    void OnHoverStay();
    void OnHoverExit();
}