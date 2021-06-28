using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
    [HideInInspector]
    public bool onPhone = false;

    Dictionary<int, List<Interactable>> touchInteractables = new Dictionary<int, List<Interactable>>();

    [Header("References")]

    [SerializeField] GraphicRaycaster gr;
    public Camera sceneCamera;
    public RectTransform sceneCanvas;
    public CanvasScaler sceneCanvasScaler;

    [Header("Settings")]

    [Tooltip("In centimeters on screen")]
    public float dragThresholdInCm;

    [Header("Items")]

    public float dragSmoothTime;

    public List<ItemContainer> visibleItemContainers = new List<ItemContainer>();
    
    
    public float containerHoverRange;

    private void Start()
    {
        onPhone = SystemInfo.deviceType == DeviceType.Handheld;
        print("On Phone: " + onPhone);
    }

    // Update is called once per frame
    void Update()
    {
        if(onPhone)
        {
            Touch[] touches = Input.touches;

            foreach(Touch t in touches)
            {
                switch(t.phase)
                {
                    case TouchPhase.Began:
                        if(!touchInteractables.ContainsKey(t.fingerId))
                        {
                            List<Interactable> interactablesBegan = FindInteractions(t.position);

                            if (interactablesBegan != null)
                            {
                                touchInteractables.Add(t.fingerId, interactablesBegan);
                                interactablesBegan.ForEach(x => x.Enter(t.fingerId));
                            }
                            else
                            {
                                touchInteractables.Add(t.fingerId, new List<Interactable>());
                            }
                        }

                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if(touchInteractables.ContainsKey(t.fingerId))
                        {
                            List<Interactable> fingerInteractables = touchInteractables[t.fingerId];

                            foreach (Interactable i in fingerInteractables)
                            {
                                i.Stay(t.deltaPosition);
                            }

                        }

                        break;

                    case TouchPhase.Ended:
                        if(touchInteractables.ContainsKey(t.fingerId))
                        {
                            List<Interactable> interactablesEnded = touchInteractables[t.fingerId];

                            if (interactablesEnded.Count > 0)
                            {
                                interactablesEnded.ForEach(x => x.Complete());

                                // If input was completed on another touch, interrupt all other inputs
                                foreach (List<Interactable> iList in touchInteractables.Values)
                                {
                                    foreach (Interactable i in iList)
                                    {
                                        i.Interrupt();
                                    }
                                }
                            }
                            touchInteractables.Remove(t.fingerId);
                        }
                        
                        break;
                }
            }
        }
    }

    List<Interactable> FindInteractions(Vector2 screenPosition)
    {
        // Cast against UI objects and Sprite colliders, find IInteractables
        bool isGraphicRaycasterConnected = gr != null;
        bool isCameraConnected = sceneCamera != null;

        //UI
        List<RaycastResult> hoveredUiObjects = null;
        int hoveredUiObjectsCount = 0;
        if (isGraphicRaycasterConnected)
        {
            hoveredUiObjects = RaycastPosition(screenPosition);
            hoveredUiObjectsCount = hoveredUiObjects.Count;
        }

        RaycastHit2D[] hoveredObjects = null;
        int hoveredObjectsCount = 0;
        if (isCameraConnected)
        {
            //Collider objects
            hoveredObjects = Physics2D.RaycastAll(sceneCamera.ScreenToWorldPoint(screenPosition), Vector2.zero);
            hoveredObjectsCount = hoveredObjects.Length;
        }


        //All behaviours collectively
        List<Interactable> interactables = new List<Interactable>();

        if (isGraphicRaycasterConnected)
        {
            //UI objects are handled first
            for (int i = 0; i < hoveredUiObjectsCount; i++)
            {
                interactables.AddRange(hoveredUiObjects[i].gameObject.GetComponents<Interactable>());
            }
        }

        if (isCameraConnected)
        {
            for (int i = 0; i < hoveredObjectsCount; i++)
            {
                interactables.AddRange(hoveredObjects[i].collider.gameObject.GetComponents<Interactable>());
            }
        }

        //Find the interactable with the highest priority
        int highestPriority = int.MinValue;
        foreach(Interactable i in interactables)
        {
            if(i.priority > highestPriority)
            {
                highestPriority = i.priority;
            }
        }

        // Return a list of interactables with the highest priority
        return interactables.FindAll(x => x.priority == highestPriority);
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
        gr.Raycast(pointerData, results);

        return results;
    }
}