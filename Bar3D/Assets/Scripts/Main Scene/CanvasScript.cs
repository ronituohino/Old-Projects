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

        // The 2d object itself
        public RectTransform rect;
        public GameObject gameObject;

        public Vector3 worldOffset;
        public Vector2 screenOffset;
    }

    public List<UIElement> uIElements = new List<UIElement>();

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.sceneCanvas = this;
    }

    UIElement CreateElement(Transform transform, GameObject gameObject, Transform parent, Vector3 worldOffset, Vector2 screenOffset)
    {
        UIElement element = new UIElement();
        element.transform = transform;

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

    public void RemoveElement(Transform t)
    {
        foreach (UIElement e in uIElements)
        {
            if (e.transform == t)
            {
                uIElements.Remove(e);
                Destroy(e.gameObject);

                return;
            }
        }
    }

    Vector2 CalculateElementPosition(UIElement e)
    {
        Vector3 screenPoint = InputManager.Instance.sceneCamera.WorldToScreenPoint(e.transform.position + e.worldOffset);

        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(annotationParent, screenPoint, InputManager.Instance.sceneCamera, out point);

        return point + e.screenOffset;
    }

    void LateUpdate()
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

    // A small dot on top of physics objects in range of the player
    [Header("Annotations")]

    [SerializeField] GameObject annotation;
    [SerializeField] RectTransform annotationParent;

    [Space]

    [SerializeField] float annotationHeight;

    public void AnnotatePhysicsObject(Transform transform)
    {
        CreateElement(transform, annotation, annotationParent, new Vector3(0, annotationHeight, 0), Vector2.zero);
    }

    // Shown on top of npc's
    [Header("Dialogue")]

    [SerializeField] GameObject dialogue;
    [SerializeField] RectTransform dialogueParent;

    [Space]

    [SerializeField] float dialogueHeight;
    [SerializeField] Vector2 dialogueOffsetInScreenSpace;

    public void ShowDialogue(Transform transform, string dialogueText)
    {
        UIElement bubble = CreateElement(transform, dialogue, dialogueParent, new Vector3(0, dialogueHeight, 0), dialogueOffsetInScreenSpace);
        DialogueBubble db = bubble.gameObject.GetComponent<DialogueBubble>();
        db.text.text = dialogueText;
    }

}
