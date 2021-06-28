using UnityEngine;

// Handles map movement
public class MapHandle : MonoBehaviour, IDraggable, IRightClickable
{
    RectTransform rect;
    Vector2 originalPos;

    Vector2 newPos = Vector2.zero;

    [SerializeField] float mapDampening;
    Vector2 mapVel = Vector2.zero;

    [SerializeField] RectTransform mapWindowRect;
    [SerializeField] MapWindow mapWindow;

    void Awake()
    {
        rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(WorldMapGenerator.Instance.mapDimensions.x, WorldMapGenerator.Instance.mapDimensions.y);

        originalPos = rect.anchoredPosition;
        newPos = rect.anchoredPosition;
    }

    void Update()
    {
        rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, newPos, ref mapVel, mapDampening);
    }

    public void SetTargetPosition(Vector2 target)
    {
        newPos = target;
    }

    void IRightClickable.OnClickHold()
    {
        
    }

    void IRightClickable.OnClickPress()
    {
        
            mapWindow.RemoveHeading();
        
    }

    void IRightClickable.OnClickRelease()
    {
        
    }

    void IDraggable.OnDrag(Vector2 totalMovement)
    {
        
            newPos = originalPos + totalMovement;

            float maxNewX = (rect.sizeDelta.x - mapWindowRect.sizeDelta.x) * 1.25f;
            float minNewX = maxNewX * -1;

            newPos.x = Mathf.Clamp(newPos.x, minNewX, maxNewX);

            float maxNewY = (rect.sizeDelta.y - mapWindowRect.sizeDelta.y) * 1.25f;
            float minNewY = maxNewY * -1;

            newPos.y = Mathf.Clamp(newPos.y, minNewY, maxNewY);
        
    }

    void IDraggable.OnDragPress()
    {
        
            originalPos = rect.anchoredPosition;
        
    }

    void IDraggable.OnDragRelease()
    {
        
    }
}