using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Handles all input from the player, keyboard, mouse, controllers, buttons...
public class InputManager : Singleton<InputManager>, GameInput.IPlayerActions
{
    GameInput input;

    List<IHoverable> hoverableElements = new List<IHoverable>();
    List<IHoverable> previousElements = new List<IHoverable>();

    bool l_clickable;
    bool r_clickable;
    bool l_draggable;
    bool r_draggable;

    ILeftClickable leftClickableElement;
    IRightClickable rightClickableElement;
    ILeftDraggable leftDraggableElement;
    IRightDraggable rightDraggableElement;

    Vector2 mousePositionWhenPressed;
    float pressMouseMovementThreshold = 5f;
    bool dragging;

    public Vector2 mousePositionInScreen = Vector2.zero;
    public Vector3 mousePositionInWorld = Vector2.zero;


    public UnityAction<char> KeyboardTextInput;
    public UnityAction<float> MouseScroll;


    public CanvasScript sceneCanvas;
    bool isGraphicRaycasterConnected = false;

    public Camera sceneCamera;
    bool isCameraConnected = false;

    public BartendingBook bartendingBook = null;

    public PlayerControls controlledPlayer = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (input == null)
        {
            input = new GameInput();
            input.Player.SetCallbacks(this);
            input.Player.Enable();
        }

        Keyboard.current.onTextInput += OnTextInput;
    }

    void Update()
    {
        #region Mouse Interaction

        //If a scene doesn't connect it's GraphicRaycaster (Canvas) to InputManager, we can't raycast for UI objects
        isGraphicRaycasterConnected = sceneCanvas != null;
        if (isGraphicRaycasterConnected)
        {
            mousePositionInScreen = Mouse.current.position.ReadValue();
        }

        //If a scene doesn't connect it's camera to InputManager, we can't raycast for 3d objects
        isCameraConnected = sceneCamera != null;
        if (isCameraConnected)
        {
            mousePositionInWorld = sceneCamera.ScreenToWorldPoint(mousePositionInScreen);
        }



        //UI
        List<RaycastResult> hoveredUiObjects = null;
        int hoveredUiObjectsCount = 0;
        if (isGraphicRaycasterConnected)
        {
            hoveredUiObjects = RaycastPosition(mousePositionInScreen);
            hoveredUiObjectsCount = hoveredUiObjects.Count;
        }

        RaycastHit[] hoveredObjects = null;
        int hoveredObjectsCount = 0;
        if (isCameraConnected)
        {
            //Collider objects
            hoveredObjects = Physics.RaycastAll(sceneCamera.ScreenPointToRay(mousePositionInScreen));
            hoveredObjectsCount = hoveredObjects.Length;
        }



        //All behaviours collectively
        List<MonoBehaviour> behaviours = new List<MonoBehaviour>();


        if (isGraphicRaycasterConnected)
        {
            //UI objects are handled first
            for (int i = 0; i < hoveredUiObjectsCount; i++)
            {
                MonoBehaviour[] attachedBehaviours = hoveredUiObjects[i].gameObject.GetComponents<MonoBehaviour>();
                behaviours.AddRange(attachedBehaviours);
            }
        }

        if (isCameraConnected)
        {
            for (int i = 0; i < hoveredObjectsCount; i++)
            {
                MonoBehaviour[] attachedBehaviours = hoveredObjects[i].collider.gameObject.GetComponents<MonoBehaviour>();
                behaviours.AddRange(attachedBehaviours);
            }
        }


        if (isGraphicRaycasterConnected || isCameraConnected)
        {
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
            bool left = Mouse.current.leftButton.wasPressedThisFrame;
            bool right = Mouse.current.rightButton.wasPressedThisFrame;
            if (left || right)
            {
                mousePositionWhenPressed = mousePositionInScreen;

                bool foundObject = false;
                foreach (MonoBehaviour mb in behaviours)
                {
                    bool lclk = mb is ILeftClickable;
                    bool rclk = mb is IRightClickable;

                    bool ldrg = mb is ILeftDraggable;
                    bool rdrg = mb is IRightDraggable;

                    if (lclk && left)
                    {
                        ILeftClickable c = (ILeftClickable)mb;
                        leftClickableElement = c;
                        c.OnClickPress();

                        l_clickable = true;
                        foundObject = true;
                    }
                    if (rclk && right)
                    {
                        IRightClickable c = (IRightClickable)mb;
                        rightClickableElement = c;
                        c.OnClickPress();

                        r_clickable = true;
                        foundObject = true;
                    }
                    if (ldrg && left)
                    {
                        ILeftDraggable d = (ILeftDraggable)mb;
                        leftDraggableElement = d;
                        d.OnDragPress();

                        l_draggable = true;
                        foundObject = true;
                    }
                    if (rdrg && right)
                    {
                        IRightDraggable d = (IRightDraggable)mb;
                        rightDraggableElement = d;
                        d.OnDragPress();

                        r_draggable = true;
                        foundObject = true;
                    }

                    if (foundObject)
                    {
                        break;
                    }
                }
            }
            //Hold
            left = Mouse.current.leftButton.isPressed;
            right = Mouse.current.rightButton.isPressed;
            if (left || right)
            {
                Vector2 mouseMovementSincePress = mousePositionInScreen - mousePositionWhenPressed;

                if (Mathf.Abs(mouseMovementSincePress.x) > pressMouseMovementThreshold
                       || Mathf.Abs(mouseMovementSincePress.y) > pressMouseMovementThreshold
                   )
                {
                    dragging = true;
                }

                if (dragging)
                {
                    if (l_draggable)
                    {
                        leftDraggableElement.OnDrag();
                    }
                    if (r_draggable)
                    {
                        rightDraggableElement.OnDrag();
                    }
                }
                else
                {
                    if (l_clickable)
                    {
                        leftClickableElement.OnClickHold();
                    }
                    if (r_clickable)
                    {
                        rightClickableElement.OnClickHold();
                    }
                }
            }
            else
            {
                dragging = false;
            }
            //Release
            left = Mouse.current.leftButton.wasReleasedThisFrame;
            right = Mouse.current.rightButton.wasReleasedThisFrame;
            if (left || right)
            {
                if (l_draggable && left)
                {
                    leftDraggableElement.OnDragRelease();
                    leftDraggableElement = null;
                    l_draggable = false;
                }
                if (r_draggable && right)
                {
                    rightDraggableElement.OnDragRelease();
                    rightDraggableElement = null;
                    r_draggable = false;
                }
                if (r_clickable && right)
                {
                    rightClickableElement.OnClickRelease();
                    rightClickableElement = null;
                    r_clickable = false;
                }
                if (l_clickable && left)
                {
                    leftClickableElement.OnClickRelease();
                    leftClickableElement = null;
                    l_clickable = false;
                }
            }
        }

        #endregion
    }

    // UI
    public List<RaycastResult> RaycastPosition(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = position;

        List<RaycastResult> results = new List<RaycastResult>();
        sceneCanvas.gr.Raycast(pointerData, results);

        return results;
    }

    // Game Input
    void GameInput.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (controlledPlayer != null)
        {
            controlledPlayer.PlayerMovement(context.ReadValue<Vector2>());
        }
    }

    void OnTextInput(char c)
    {
        KeyboardTextInput?.Invoke(c);
    }

    void GameInput.IPlayerActions.OnDropLeft(InputAction.CallbackContext context)
    {
        if (controlledPlayer != null)
        {
            controlledPlayer.Drop(controlledPlayer.leftHand);
        }
    }

    void GameInput.IPlayerActions.OnDropRight(InputAction.CallbackContext context)
    {
        if (controlledPlayer != null)
        {
            controlledPlayer.Drop(controlledPlayer.rightHand);
        }
    }

    void GameInput.IPlayerActions.OnPour(InputAction.CallbackContext context)
    {
        if (controlledPlayer != null)
        {
            controlledPlayer.Pour(context.ReadValueAsButton());
        }
    }

    void GameInput.IPlayerActions.OnOpenBartendingBook(InputAction.CallbackContext context)
    {
        if(bartendingBook != null)
        {
            bartendingBook.OpenBook();
        }
    }

    void GameInput.IPlayerActions.OnScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();
        MouseScroll?.Invoke(Mathf.Clamp(scroll, -1f, 1f));
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



interface ILeftDraggable
{
    void OnDragPress();
    void OnDrag();
    void OnDragRelease();
}

interface IRightDraggable
{
    void OnDragPress();
    void OnDrag();
    void OnDragRelease();
}



interface IHoverable
{
    void OnHoverEnter();
    void OnHoverStay();
    void OnHoverExit();
}
