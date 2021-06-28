using UnityEngine;

public class Initiate : MonoBehaviour, ILeftClickable
{
    [SerializeField] MapWindow mapWindow;
    RectTransform rect;

    [Space]

    [SerializeField] Vector2 buttonPosClickable;
    [SerializeField] Vector2 buttonPosHidden;

    [SerializeField] float buttonLerpSpeed;

    bool clickable = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        clickable = mapWindow.markerHeading != null;

        if(clickable)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, buttonPosClickable, buttonLerpSpeed);
        } 
        else
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, buttonPosHidden, buttonLerpSpeed);
        }
    }

    void ILeftClickable.OnClickHold()
    {
        
    }

    void ILeftClickable.OnClickPress()
    {
        if(clickable)
        {
            mapWindow.StartTravel();
        }
    }

    void ILeftClickable.OnClickRelease()
    {
        
    }
}