using UnityEngine;

public class WindowTopDrag : MonoBehaviour, IDraggable
{
    Window window;

    Vector2 mouseRectOffset;
    RectTransform windowTransform;
    

    private void Awake()
    {
        windowTransform = transform.parent.GetComponent<RectTransform>();
        window = windowTransform.GetComponent<Window>();
    }

    void IDraggable.OnDrag(Vector2 movement)
    {
        Vector2 rectCenter = InputManager.Instance.mouseInScreen + mouseRectOffset;

        //Set dragged gameObject position
        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                ControlPanel.Instance.controlPanelRect, 
                rectCenter, 
                CameraScript.Instance.cam, 
                out point
            );


        window.WindowLocation = point;
    }

    void IDraggable.OnDragPress()
    {
        Vector2 rectMiddleScreen = RectTransformUtility.WorldToScreenPoint
            (
                CameraScript.Instance.cam, 
                windowTransform.TransformPoint(Vector3.zero)
            );

        mouseRectOffset = rectMiddleScreen - Input.mousePosition.ToVector2();

        window.MakeWindowOnTop();
    }

    void IDraggable.OnDragRelease() { }
}