using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    public CameraMovement controlledCamera;

    //All behaviours collectively
    List<MonoBehaviour> behaviours = new List<MonoBehaviour>();

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

    // How many pixels mouse has to move in order to register as a drag
    [SerializeField] float pressMouseMovementThreshold;

    public Vector2 mouseInScreen { get; private set; }
    public Vector3 mouseInWorld { get; private set; }

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

    // Mouse raycast hit on first physics object
    [HideInInspector]
    public RaycastHit closestMouseRayHit;

    //Changed from PhysicsObject.cs
    public bool interactingWithPhysicsObject { get; set; }

    public bool holdingShift { private set; get; }

    // Update is called once per frame
    void Update()
    {
        if (ignoringInput)
        {
            return;
        }

        mouseInScreen = Input.mousePosition;
        mouseInWorld = controlledCamera.cam.ScreenToWorldPoint(mouseInScreen, Camera.MonoOrStereoscopicEye.Mono);

        //Debug
        if (Input.GetKeyDown(KeyCode.H))
        {
            //WorldMapGenerator.Instance.GenerateMap();
        }

        holdingShift = Input.GetKey(KeyCode.LeftShift);

        mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        //Collider objects
        RaycastHit[] hoveredObjects = Physics.RaycastAll(mouseInWorld, controlledCamera.transform.forward);
        int hoveredObjectsCount = hoveredObjects.Length;

        //UI
        List<RaycastResult> hoveredUiObjects = RaycastPosition(mouseInScreen);
        int hoveredUiObjectsCount = hoveredUiObjects.Count;

        behaviours.Clear();

        //UI objects are handled first
        for (int i = 0; i < hoveredUiObjectsCount; i++)
        {
            MonoBehaviour[] attachedBehaviours = hoveredUiObjects[i].gameObject.GetComponents<MonoBehaviour>();
            behaviours.AddRange(attachedBehaviours);
        }

        //Find the nearest physics object
        int closestIndex = 0;
        float closest = float.MaxValue;

        for (int i = 0; i < hoveredObjectsCount; i++)
        {
            float dist = Vector3.Distance(mouseInWorld, hoveredObjects[i].point);
            if(dist < closest)
            {
                closest = dist;
                closestIndex = i;
            }

            MonoBehaviour[] attachedBehaviours = hoveredObjects[i].collider.gameObject.GetComponents<MonoBehaviour>();
            MonoBehaviour[] attachedParentBehaviours = hoveredObjects[i].collider.transform.parent.GetComponents<MonoBehaviour>();

            behaviours.AddRange(attachedBehaviours);
            behaviours.AddRange(attachedParentBehaviours);
        }

        if(hoveredObjectsCount > 0)
        {
            closestMouseRayHit = hoveredObjects[closestIndex];

            //Switch places to make closesIndex = 0
            if(hoveredObjectsCount > 1)
            {
                RaycastHit locRch = hoveredObjects[0];
                hoveredObjects[0] = hoveredObjects[closestIndex];
                hoveredObjects[closestIndex] = locRch;

                closestIndex = 0;
            }
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
            if (right)
            {
                if (r_clickable)
                {
                    r_clickableElement.OnClickHold();
                }
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

                    // Camera movement
                    // We are holding mouse and dragging, but not on any interactable object
                    else
                    {
                        controlledCamera.MoveCamera(mouseMovement - mouseMovement.normalized * pressMouseMovementThreshold);
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
            if (r_clickable && right)
            {
                r_clickableElement.OnClickRelease();
            }
            if (draggable && left)
            {
                draggableElement.OnDragRelease();
            }
            if (dragging && left && !draggable)
            {
                controlledCamera.ReleaseDrag();
            }
            if (l_clickable && left && !dragging)
            {
                l_clickableElement.OnClickRelease();
            }

            ResetVariables();
        }
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