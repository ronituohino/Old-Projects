using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles all Main.scene UI
public class CanvasScript : MonoBehaviour
{
    // GENERAL

    public GraphicRaycaster gr;

    [System.Serializable]
    public class UIElement
    {
        // Update ui elemnt according to this transform's position
        public Transform transform;
        public RectTransform parentRect;

        // The 2d object itself
        public RectTransform rect;
        public GameObject gameObject;

        public Vector2 worldOffset;
        public Vector2 screenOffset;
    }

    public List<UIElement> uIElements = new List<UIElement>();

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.sceneCanvas = this;
    }

    UIElement CreateElement(Transform transform, GameObject gameObject, RectTransform parent, Vector2 worldOffset, Vector2 screenOffset)
    {
        UIElement element = new UIElement();
        element.transform = transform;
        element.parentRect = parent;

        element.worldOffset = worldOffset;
        element.screenOffset = screenOffset;

        GameObject g = Instantiate(gameObject, parent);
        element.gameObject = g;

        RectTransform rect = g.GetComponent<RectTransform>();
        element.rect = rect;

        rect.anchoredPosition = CalculateElementPosition(element);

        uIElements.Add(element);
        return element;
    }

    public void RemoveElement(Transform transform)
    {
        foreach (UIElement e in uIElements)
        {
            if (e.transform == transform)
            {
                uIElements.Remove(e);
                Destroy(e.gameObject);

                return;
            }
        }
    }

    Vector2 CalculateElementPosition(UIElement element)
    {
        Vector3 screenPoint = InputManager.Instance.sceneCamera.WorldToScreenPoint
                                                (
                                            new Vector3
                                                    (
                                                        element.transform.position.x + element.worldOffset.x, 
                                                        element.transform.position.y + element.worldOffset.y, 
                                                        0
                                                    )
                                                );


        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(element.parentRect, screenPoint, InputManager.Instance.sceneCamera, out point);

        return point + element.screenOffset;
    }

    void Update()
    {
        UpdateElements();
    }

    public void UpdateElements()
    {
        foreach (UIElement e in uIElements)
        {
            e.rect.anchoredPosition = CalculateElementPosition(e);
        }
    }

    // SPECIFIC ELEMENTS

    // An info thingy that shows bottle name, type and fullness
    [Header("Bottle Info")]

    [SerializeField] GameObject bottleInfo;
    [SerializeField] RectTransform bottleInfoParent;

    public BottleInfo ShowBottleInfo(Transform transform, Vector2 worldOffset, BottlePhysics bp)
    {
        BottleInfo bi = CreateElement(transform, bottleInfo, bottleInfoParent, worldOffset, Vector2.zero).gameObject.GetComponent<BottleInfo>();
        bi.Initialize(bp);
        return bi;
    }

    // Shown on top of customers to indicate what they want
    [Header("Speech Bubble")]

    [SerializeField] GameObject bubble;
    [SerializeField] RectTransform bubbleParent;

    public DialogueBubble ShowBubble(Transform transform, Vector2 worldOffset, string text, Transform npcTransform)
    {
        DialogueBubble db = CreateElement(transform, bubble, bubbleParent, worldOffset, Vector2.zero).gameObject.GetComponent<DialogueBubble>();
        db.text.text = text;
        db.bubbleText = text;
        db.npcTransform = npcTransform;

        return db;
    }
}
