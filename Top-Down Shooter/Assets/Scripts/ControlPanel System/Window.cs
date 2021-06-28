using TMPro;
using UnityEngine;

//Base for control windows
public class Window : MonoBehaviour
{
    public Vector2 defaultSize;
    public bool scalable;
    public Vector2 scaleStep;

    [Space]

    public Vector2 minSize;
    public Vector2 maxSize;

    [Space]

    public RectTransform windowTransform;
    public TextMeshProUGUI topText;

    [HideInInspector]
    public Tab associatedTab;

    [Space]

    public Sprite tabSprite;


    bool windowEnabled = false;
    public bool WindowEnabled
    {
        get { return windowEnabled; }
        set
        {
            windowEnabled = value;
            gameObject.SetActive(value);
        }
    }

    Vector2 windowLocation = Vector2.zero;
    public Vector2 WindowLocation
    {
        get { return windowLocation; }
        set
        {
            windowLocation = value;
            windowTransform.anchoredPosition = value;
        }
    }

    Vector2 windowScale = new Vector2(1, 1);
    public Vector2 WindowScale
    {
        get { return windowScale; }
        set
        {
            windowScale = value;
            windowTransform.sizeDelta = new Vector2(defaultSize.x * value.x, defaultSize.y * value.y);

            OnScale?.Invoke();
        }
    }


    //Events
    public delegate void WindowScaledEvent();
    public event WindowScaledEvent OnScale;

    protected void Awake()
    {
        windowTransform.sizeDelta = defaultSize;

        WindowEnabled = windowEnabled;
        WindowLocation = windowLocation;
        WindowScale = windowScale;
    }

    public void MakeWindowOnTop()
    {
        windowTransform.SetAsLastSibling();
        if(ControlPanel.Instance.carriedItem != null)
        {
            ControlPanel.Instance.carriedItem.rect.SetAsLastSibling();
        }
    }

    public void ToggleWindow()
    {
        WindowEnabled = !WindowEnabled;
        MakeWindowOnTop();
    }
}