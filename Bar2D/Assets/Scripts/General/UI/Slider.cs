using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour, ILeftDraggable
{
    [SerializeField] RectTransform canvas;
    [SerializeField] RectTransform rect;

    [Space]

    [SerializeField] Vector2 xBoundaries;
    [SerializeField] Vector2 yBoundaries;

    public float xVal = 0f;
    public float yVal = 0f;

    Vector2 startPos = Vector2.zero;
    Vector2 startOfDrag = Vector2.zero;

    Vector2 vel;

    void ILeftDraggable.OnDrag()
    {
        Vector2 pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                                        (
                                            canvas,
                                            InputManager.Instance.mousePositionInScreen,
                                            InputManager.Instance.sceneCamera,
                                            out pos
                                        );
        Vector2 delta = pos - startOfDrag;
        Vector2 newPos = startPos + delta;

        newPos = new Vector2(Mathf.Clamp(newPos.x, xBoundaries.x, xBoundaries.y), Mathf.Clamp(newPos.y, yBoundaries.x, yBoundaries.y));
        rect.anchoredPosition = newPos;

        xVal = newPos.x.Remap(xBoundaries.x, xBoundaries.y, 0f, 1f);
        yVal = newPos.y.Remap(yBoundaries.x, yBoundaries.y, 0f, 1f);
    }

    void ILeftDraggable.OnDragPress()
    {
        startPos = rect.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                                        (
                                            canvas,
                                            InputManager.Instance.mousePositionInScreen,
                                            InputManager.Instance.sceneCamera,
                                            out startOfDrag
                                        );

    }

    void ILeftDraggable.OnDragRelease() { }

    public IEnumerator SetSliderCoroutine(Vector2 values, float time)
    {
        Vector2 orignalPos = rect.anchoredPosition;
        Vector2 newPos = new Vector2(values.x.Remap(0f, 1f, xBoundaries.x, xBoundaries.y), values.y.Remap(0f, 1f, yBoundaries.x, yBoundaries.y));

        int fixedFramesToWait = Mathf.RoundToInt(time / Time.fixedDeltaTime);
        for(int i = 0; i <= fixedFramesToWait; i++)
        {
            Vector2 damp = Vector2.Lerp(orignalPos, newPos, (float)i / (float)fixedFramesToWait);

            rect.anchoredPosition = damp;
            xVal = damp.x.Remap(xBoundaries.x, xBoundaries.y, 0f, 1f);
            yVal = damp.y.Remap(yBoundaries.x, yBoundaries.y, 0f, 1f);

            yield return GlobalReferencesAndSettings.Instance.wait;
        }
    }
}
