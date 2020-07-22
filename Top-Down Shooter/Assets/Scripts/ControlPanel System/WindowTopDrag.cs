using UnityEngine;

public class WindowTopDrag : MonoBehaviour, IDraggable
{
    [SerializeField] Window window;

    Vector2 mouseRectOffset;
    RectTransform windowTransform;
    

    private void Awake()
    {
        windowTransform = transform.parent.GetComponent<RectTransform>();
    }

    void IDraggable.OnDrag()
    {
        Vector2 rectCenter = Input.mousePosition.ToVector2() + mouseRectOffset;

        //Set dragged gameObject position
        Vector2 point = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlPanel.Instance.controlPanelRect, rectCenter, ControlPanel.Instance.cam, out point);
        windowTransform.anchoredPosition = point;

        Tab.WindowSettings settings = ControlPanelManager.Instance.FetchSettings(window.associatedTab);
        settings.location = point;

        ControlPanelManager.Instance.UpdateSettings(window.associatedTab, settings);
    }

    void IDraggable.OnPress()
    {
        Vector2 rectMiddleScreen = RectTransformUtility.WorldToScreenPoint(ControlPanel.Instance.cam, windowTransform.TransformPoint(new Vector3(0, 0, 0)));
        mouseRectOffset = rectMiddleScreen - Input.mousePosition.ToVector2();

        window.MakeWindowOnTop();
    }

    void IDraggable.OnRelease() { }
}