using TMPro;
using UnityEngine;

//Base for control windows
public class Window : MonoBehaviour
{
    public Vector2 defaultSize;
    public bool scalable;
    public Vector2 scaleStep;
    public Vector2 minSize;

    [Space]

    public RectTransform windowTransform;
    public TextMeshProUGUI topText;

    [HideInInspector]
    public Tab associatedTab;

    [Space]

    public Sprite tabSprite;

    private void Awake()
    {
        associatedTab = ControlPanelManager.Instance.AddTabSingle(this);
    }

    public void MakeWindowOnTop()
    {
        windowTransform.SetAsLastSibling();
        if(ControlPanel.Instance.carriedItem != null)
        {
            ControlPanel.Instance.carriedItem.rect.SetAsLastSibling();
        }
    }
}